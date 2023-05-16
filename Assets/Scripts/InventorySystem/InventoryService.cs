using System;

namespace Assets.Scripts.InventorySystem
{
    public class InventoryService
    {
        public int Coins { get; private set; }
        
        public void AddCoin(int amount)
        {
            if (amount < 0)
                return;

            Coins += amount;
        }

        public void RemoveCoins(int coins)
        {
            if (coins < 0)
                return;

            Coins -= coins;
        }
    }
}