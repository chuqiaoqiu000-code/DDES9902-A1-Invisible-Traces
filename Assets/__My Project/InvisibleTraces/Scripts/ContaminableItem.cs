using System.Collections.Generic;
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

        private static readonly Color TraceColour = new Color(0.95f, 0.12f, 0.72f);

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            normalColours = new Color[renderers.Length];
            for (var i = 0; i < renderers.Length; i++)
                normalColours[i] = renderers[i].material.color;

            ResetContamination();
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

        private void OnCollisionEnter(Collision collision)
        {
            var other = collision.collider.GetComponentInParent<ContaminableItem>();
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
        }
    }
}
