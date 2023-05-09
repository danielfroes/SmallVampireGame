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
        }
        public void Dispose()
        {
            ServiceLocator.Unregister<InventoryService>();
        }

    }
}