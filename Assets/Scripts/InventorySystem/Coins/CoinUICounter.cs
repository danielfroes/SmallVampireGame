using Assets.Scripts.Utils;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.InventorySystem.Coins
{
    public class CoinUICounter : MonoBehaviour
    {
        [SerializeField] TMP_Text _coinCounterText;

        // Update is called once per frame
        void Update()
        {
            int coins = ServiceLocator.Get<InventoryService>().Coins;
            _coinCounterText.text = $"Coins: {coins}";
        }
    }
}