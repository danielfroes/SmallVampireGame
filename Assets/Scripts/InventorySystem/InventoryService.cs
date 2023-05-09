using System;

namespace Assets.Scripts.InventorySystem
{
    public class InventoryService
    {
        public int Coins { get; private set; }
        
        public void AddCoin(int amount)
        {
            Coins += amount;
        }
    }
}