using UnityEngine;

namespace InvisibleTraces
{
    public class PreparationZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var item = other.GetComponentInParent<ContaminableItem>();
            if (item != null)
                InvisibleTracesManager.Instance?.RegisterPreparedItem(item);
        }
    }
}
