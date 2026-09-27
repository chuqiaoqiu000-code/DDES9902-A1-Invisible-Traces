using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace InvisibleTraces
{
    public class InvisibleTracesManager : MonoBehaviour
    {
        public static InvisibleTracesManager Instance { get; private set; }

        public TMP_Text objectiveText;
        public TMP_Text feedbackText;
        public TMP_Text progressText;
        public TMP_Text traceLegendText;

        private readonly HashSet<string> preparedItems = new HashSet<string>();
        private ContaminableItem[] contaminableItems;
        private bool workstationClean;
        private bool handsWashed;
        private bool inspectionCompleted;
        private bool traceMode;
        private bool completed;
        private static readonly Color NeutralFeedback = new Color(0.85f, 0.92f, 0.95f);
        private static readonly Color PositiveFeedback = new Color(0.35f, 1f, 0.58f);
        private static readonly Color WarningFeedback = new Color(1f, 0.42f, 0.24f);

        private void Awake()
        {
            Instance = this;
            contaminableItems = FindObjectsByType<ContaminableItem>(FindObjectsSortMode.None);
        }

        private void Start()
        {
            ResetTraining();
        }

        public void CleanWorkstation()
        {
            if (completed) return;
            workstationClean = true;
            var mustRewash = handsWashed;
            handsWashed = false;
            feedbackText.color = mustRewash ? WarningFeedback : PositiveFeedback;
            feedbackText.text = mustRewash
                ? "WORKTOP CLEANED - WASH HANDS AGAIN\nCleaning controlled the surface, but created new hand contact. Sequence matters."
                : "WORKTOP CLEANED\nSurface residue removed. Wash hands before touching clean equipment.";
            UpdateProgress();
        }

        public void WashHands()
        {
            if (completed) return;
            handsWashed = true;
            feedbackText.color = workstationClean ? PositiveFeedback : WarningFeedback;
            feedbackText.text = workstationClean
                ? "HANDS WASHED\nYou are ready to handle clean equipment."
                : "HANDS WASHED - BUT THE WORKTOP IS STILL A HAZARD\nClean the shared surface before preparing the order.";
            UpdateProgress();
        }

        public void ToggleTraceMode()
        {
            traceMode = !traceMode;
            if (traceMode)
                inspectionCompleted = true;
            foreach (var item in contaminableItems)
                item.SetTraceMode(traceMode);

            traceLegendText.gameObject.SetActive(traceMode);
            feedbackText.color = traceMode ? new Color(1f, 0.35f, 0.82f) : NeutralFeedback;
            feedbackText.text = traceMode
                ? "TRACE MODE ON\nMagenta reveals otherwise invisible allergen contamination."
                : "TRACE MODE OFF\nThe hazard is invisible again, as it is in a real kitchen.";
            UpdateProgress();
        }

        public void RegisterPreparedItem(ContaminableItem item)
        {
            if (completed || item == null || preparedItems.Contains(item.itemId))
                return;

            if (item.IsContaminated)
            {
                completed = true;
                objectiveText.color = WarningFeedback;
                objectiveText.text = "ORDER UNSAFE - TRACE THE CAUSE";
                feedbackText.color = WarningFeedback;
                feedbackText.text = "CONTACT CHAIN\n" + item.GetSourceSummary() +
                                    " -> " + item.displayName +
                                    " -> peanut-free order. Reveal the trace, then reset and break the chain.";
                progressText.text = "DEBRIEF: CONTACT, NOT APPEARANCE, DETERMINED THE OUTCOME";
                return;
            }

            if (!workstationClean || !handsWashed)
            {
                feedbackText.color = WarningFeedback;
                feedbackText.text = "PROCESS RISK\nPrepare the workspace and wash hands before placing food or tools.";
                return;
            }

            preparedItems.Add(item.itemId);
            feedbackText.color = PositiveFeedback;
            feedbackText.text = item.displayName + " added safely to the preparation zone.";
            UpdateProgress();

            if (preparedItems.Contains("clean_knife") &&
                preparedItems.Contains("bread") &&
                preparedItems.Contains("vegetables"))
            {
                completed = true;
                objectiveText.color = PositiveFeedback;
                objectiveText.text = "SAFE ORDER COMPLETE";
                feedbackText.text = "CONTROLLED CONTACT CHAIN\nInspect -> clean surface -> wash hands -> use clean tools and ingredients.";
                progressText.text = "DEBRIEF: THE SEQUENCE BROKE EVERY ALLERGEN TRANSFER PATH";
            }
        }

        public void ReportTransfer(string source, string destination)
        {
            if (!completed)
            {
                feedbackText.color = WarningFeedback;
                feedbackText.text = "CONTACT RECORDED\nA hidden trace moved from " + source + " to " + destination + ".";
            }
        }

        public void ResetTraining()
        {
            workstationClean = false;
            handsWashed = false;
            inspectionCompleted = false;
            traceMode = false;
            completed = false;
            preparedItems.Clear();

            contaminableItems = FindObjectsByType<ContaminableItem>(FindObjectsSortMode.None);
            foreach (var item in contaminableItems)
            {
                item.ResetContamination();
                item.SetTraceMode(false);
                var holdable = item.GetComponent<Holdable>();
                if (holdable != null)
                    holdable.ResetOrientation();
            }

            objectiveText.text = "PEANUT-FREE ORDER\nInspect. Clean. Wash. Prepare safely.";
            objectiveText.color = Color.white;
            feedbackText.color = NeutralFeedback;
            feedbackText.text = "A peanut-butter order was prepared here moments ago.\nThe contamination is invisible. Inspect before you act.";
            traceLegendText.gameObject.SetActive(false);
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            progressText.text =
                "1 INSPECT " + Mark(inspectionCompleted) +
                "   2 WORKTOP " + Mark(workstationClean) +
                "   3 HANDS " + Mark(handsWashed) +
                "\n4 KNIFE " + Mark(preparedItems.Contains("clean_knife")) +
                "   BREAD " + Mark(preparedItems.Contains("bread")) +
                "   VEG " + Mark(preparedItems.Contains("vegetables"));
        }

        private static string Mark(bool value) => value ? "[SAFE]" : "[TODO]";
    }
}
