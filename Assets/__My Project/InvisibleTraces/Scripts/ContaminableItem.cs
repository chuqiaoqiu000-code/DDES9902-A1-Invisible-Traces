using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace InvisibleTraces
{
    public class ContaminableItem : MonoBehaviour
    {
        public string itemId;
        public string displayName;
        public bool startsContaminated;
        public bool IsContaminated { get; private set; }

        private readonly HashSet<string> contaminationSources = new HashSet<string>();
        private Renderer[] renderers;
        private Color[] normalColours;
        private bool traceMode;
        private TMP_Text traceLabel;

        private static readonly Color TraceColour = new Color(0.95f, 0.12f, 0.72f);

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            normalColours = new Color[renderers.Length];
            for (var i = 0; i < renderers.Length; i++)
                normalColours[i] = renderers[i].material.color;

            CreateTraceLabel();
            ResetContamination();
        }

        private void CreateTraceLabel()
        {
            var labelObject = new GameObject("Accessible Trace Label");
            labelObject.transform.SetParent(transform, false);

            var scale = transform.localScale;
            labelObject.transform.localPosition = new Vector3(
                0f,
                0.5f + (0.22f / Mathf.Max(scale.y, 0.01f)),
                -0.52f);
            labelObject.transform.localScale = new Vector3(
                1f / Mathf.Max(scale.x, 0.01f),
                1f / Mathf.Max(scale.y, 0.01f),
                1f / Mathf.Max(scale.z, 0.01f));

            traceLabel = labelObject.AddComponent<TextMeshPro>();
            traceLabel.text = "! ALLERGEN TRACE !";
            traceLabel.fontSize = 0.72f;
            traceLabel.fontStyle = FontStyles.Bold;
            traceLabel.alignment = TextAlignmentOptions.Center;
            traceLabel.color = Color.white;
            traceLabel.rectTransform.sizeDelta = new Vector2(2.6f, 0.35f);
            traceLabel.gameObject.SetActive(false);
        }

        public void ResetContamination()
        {
            contaminationSources.Clear();
            IsContaminated = startsContaminated;
            if (startsContaminated)
                contaminationSources.Add(displayName);
            RefreshVisuals();
        }

        public void SetTraceMode(bool enabled)
        {
            traceMode = enabled;
            RefreshVisuals();
        }

        public void ContaminateFrom(ContaminableItem source)
        {
            if (source == null || !source.IsContaminated || IsContaminated)
                return;

            IsContaminated = true;
            foreach (var entry in source.contaminationSources)
                contaminationSources.Add(entry);
            contaminationSources.Add(source.displayName);
            RefreshVisuals();

            InvisibleTracesManager.Instance?.ReportTransfer(source.displayName, displayName);
        }

        public string GetSourceSummary()
        {
            return contaminationSources.Count == 0
                ? "unknown contact"
                : string.Join(" -> ", contaminationSources);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var other = collision.collider.GetComponentInParent<ContaminableItem>();
            TransferOnContact(other);
        }

        private void OnTriggerEnter(Collider otherCollider)
        {
            // EZPZ Holdable temporarily changes colliders to triggers while objects
            // are carried, so trigger contact must propagate contamination too.
            var other = otherCollider.GetComponentInParent<ContaminableItem>();
            TransferOnContact(other);
        }

        private void TransferOnContact(ContaminableItem other)
        {
            if (other == null || other == this)
                return;

            if (other.IsContaminated)
                ContaminateFrom(other);
            else if (IsContaminated)
                other.ContaminateFrom(this);
        }

        private void RefreshVisuals()
        {
            if (renderers == null)
                return;

            for (var i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                    renderers[i].material.color = traceMode && IsContaminated
                        ? TraceColour
                        : normalColours[i];
            }

            if (traceLabel != null)
                traceLabel.gameObject.SetActive(traceMode && IsContaminated);
        }
    }
}
