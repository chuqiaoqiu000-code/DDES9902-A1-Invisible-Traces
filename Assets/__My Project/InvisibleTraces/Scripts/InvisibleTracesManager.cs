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
        private bool traceMode;
        private bool completed;

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
            feedbackText.text = "WORKTOP CLEANED\nGood: remove residue before handling safe ingredients.";
            UpdateProgress();
        }

        public void WashHands()
        {
            if (completed) return;
            handsWashed = true;
            feedbackText.text = workstationClean
                ? "HANDS WASHED\nYou are ready to handle clean equipment."
                : "HANDS WASHED - BUT THE WORKTOP IS STILL A HAZARD\nClean the shared surface before preparing the order.";
            UpdateProgress();
        }

        public void ToggleTraceMode()
        {
            traceMode = !traceMode;
            foreach (var item in contaminableItems)
                item.SetTraceMode(traceMode);

            traceLegendText.gameObject.SetActive(traceMode);
            feedbackText.text = traceMode
                ? "TRACE MODE ON\nMagenta reveals otherwise invisible allergen contamination."
                : "TRACE MODE OFF\nThe hazard is invisible again, as it is in a real kitchen.";
        }

        public void RegisterPreparedItem(ContaminableItem item)
        {
            if (completed || item == null || preparedItems.Contains(item.itemId))
                return;

            if (item.IsContaminated)
            {
                completed = true;
                feedbackText.text = "ORDER UNSAFE\n" + item.displayName +
                                    " carries peanut contamination. Use Trace Mode to investigate, then reset.";
                progressText.text = "OUTCOME: CROSS-CONTAMINATION DETECTED";
                return;
            }

            if (!workstationClean || !handsWashed)
            {
                feedbackText.text = "PROCESS RISK\nPrepare the workspace and wash hands before placing food or tools.";
                return;
            }

            preparedItems.Add(item.itemId);
            feedbackText.text = item.displayName + " added safely to the preparation zone.";
            UpdateProgress();

            if (preparedItems.Contains("clean_knife") &&
                preparedItems.Contains("bread") &&
                preparedItems.Contains("vegetables"))
            {
                completed = true;
                objectiveText.text = "ORDER COMPLETE";
                feedbackText.text = "SAFE ORDER COMPLETED\nYou controlled the surface, hand, tool and ingredient pathway.";
                progressText.text = "OUTCOME: SAFE PROCEDURE ACHIEVED";
            }
        }

        public void ReportTransfer(string source, string destination)
        {
            if (!completed)
                feedbackText.text = "CONTACT RECORDED\nA hidden trace moved from " + source + " to " + destination + ".";
        }

        public void ResetTraining()
        {
            workstationClean = false;
            handsWashed = false;
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

            objectiveText.text = "PEANUT-FREE ORDER\nInspect risk, clean the worktop, wash hands, then prepare safely.";
            feedbackText.text = "A peanut-butter order was prepared here moments ago.\nThe contamination is invisible. What should you do first?";
            traceLegendText.gameObject.SetActive(false);
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            progressText.text =
                "WORKTOP " + Mark(workstationClean) +
                "   HANDS " + Mark(handsWashed) +
                "\nCLEAN KNIFE " + Mark(preparedItems.Contains("clean_knife")) +
                "   BREAD " + Mark(preparedItems.Contains("bread")) +
                "   VEG " + Mark(preparedItems.Contains("vegetables"));
        }

        private static string Mark(bool value) => value ? "[SAFE]" : "[TODO]";
    }
}
