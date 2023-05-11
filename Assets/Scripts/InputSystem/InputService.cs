

namespace Assets.Scripts.InputSystem
{
    public class InputService
    {
        public PlayerMovementInput PlayerInput { get; }

        public InputService()
        {
            PlayerInput = new PlayerMovementInput();
            PlayerInput.Enable();
        }
    }
}