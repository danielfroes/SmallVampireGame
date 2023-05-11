using Assets.Scripts.InputSystem;
using Assets.Scripts.InventorySystem;
using Assets.Scripts.Utils;
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
        }
        public void Dispose()
        {
            ServiceLocator.Unregister<InventoryService>();
            ServiceLocator.Unregister<InputService>();
        }

    }
}