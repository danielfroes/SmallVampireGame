using Assets.Scripts.InputSystem;
using Assets.Scripts.Utils;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Interactable
{
    public class ButtonInteractable : AInteractable
    {
        PlayerMovementInput _input;

        public void Start()
        {
            _input = ServiceLocator.Get<InputService>().PlayerInput;

        }

        public override void Enable()
        {
            _input.Player.Interact.performed += ExecuteInteraction;
        }

        public override void Disable()
        {
            _input.Player.Interact.performed -= ExecuteInteraction;
        }

        void ExecuteInteraction(InputAction.CallbackContext obj)
        {
            TriggerInteraction();
        }

    }
}