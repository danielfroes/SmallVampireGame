using Assets.Scripts.Utils;

using UnityEngine;

namespace Assets.Scripts.InventorySystem
{
    public class CoinCollector : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.IsPlayer())
                return;

            ServiceLocator.Get<InventoryService>().AddCoin(1);
            Destroy(gameObject);
        }
    }
}