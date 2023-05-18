using Assets.Scripts.InputSystem;
using Assets.Scripts.InventorySystem;
using Assets.Scripts.Utils;
using StarterAssets;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Initialization
{
    public class DefaultServicesInitializer : IInitializationStep
    {
        public void Run()
        {
            ServiceLocator.Register(new InventoryService());
            ServiceLocator.Register(new InputService());
            ServiceLocator.Register(new PlayerStatsService());
        }
        public void Dispose()
        {
            ServiceLocator.Unregister<InventoryService>();
            ServiceLocator.Unregister<InputService>();
            ServiceLocator.Unregister<PlayerStatsService>();
        }

    }
}