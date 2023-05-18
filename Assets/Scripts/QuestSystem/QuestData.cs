using Assets.Game.Scripts.Utils;
using Assets.Scripts.InventorySystem;
using Assets.Scripts.Utils;
using StarterAssets;
using UnityEngine;

namespace Assets.Scripts.QuestSystem
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest System/Quest", order = 0)]
    public class QuestData : ScriptableObject
    {
        [SerializeReference, SerializeReferenceMenu] IQuest _quest;
        public IQuest Quest => _quest;
    }

    public interface IQuest
    {
        bool IsCompleted { get; }
        bool CanBeCompleted();
        void Complete();
    }

    public class CollectCoinsQuest : IQuest
    {
        [SerializeField] int _coinsToCollect;

        public bool IsCompleted { get; private set; }
        public bool CanBeCompleted()
        {
            return ServiceLocator.Get<InventoryService>().Coins >= _coinsToCollect;
        }

        public void Complete()
        {
            ServiceLocator.Get<InventoryService>().RemoveCoins(_coinsToCollect);
            ServiceLocator.Get<PlayerStatsService>().IncreaseBatTime();
            IsCompleted = true;
        }
    }
}