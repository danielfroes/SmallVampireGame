using Assets.Scripts.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.Interactable
{
    public abstract class AInteractable : MonoBehaviour
    {
        public event Action OnInteract;

        void OnTriggerExit(Collider other)
        {
            if (!other.IsPlayer())
                return;

            Disable();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.IsPlayer())
                return;

            Enable();
        }

        protected void TriggerInteraction() => OnInteract?.Invoke();

        public abstract void Enable();

        public abstract void Disable();

    }
}