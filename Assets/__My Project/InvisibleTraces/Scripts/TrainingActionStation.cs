using UnityEngine;

namespace InvisibleTraces
{
    [RequireComponent(typeof(InteractableGeneral))]
    public class TrainingActionStation : MonoBehaviour
    {
        public enum ActionType
        {
            CleanWorkstation,
            WashHands,
            ToggleTraceMode,
            ResetTraining
        }

        public ActionType action;

        private void Start()
        {
            GetComponent<InteractableGeneral>().onPrimaryInteract.AddListener(Activate);
        }

        public void Activate()
        {
            var manager = InvisibleTracesManager.Instance;
            if (manager == null) return;

            switch (action)
            {
                case ActionType.CleanWorkstation: manager.CleanWorkstation(); break;
                case ActionType.WashHands: manager.WashHands(); break;
                case ActionType.ToggleTraceMode: manager.ToggleTraceMode(); break;
                case ActionType.ResetTraining: manager.ResetTraining(); break;
            }
        }
    }
}
