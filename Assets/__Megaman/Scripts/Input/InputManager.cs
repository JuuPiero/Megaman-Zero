using UnityEngine;
using UnityEngine.InputSystem;
namespace Megaman
{
    public class InputManager: MonoBehaviour, IManager
    {
        //public PlayerControls _input;

        private InputSystem_Actions _input;

        public bool JumpPressed { get; protected set; }

        public Vector2 Direction { get; protected set; }

        public void Initialize(params object[] parameters) 
        {
        }

        private void Awake()
        {
            _input = new InputSystem_Actions();
            ServiceLocator.Register<InputManager>(this);
        }

        private void OnEnable()
        {
            _input.Enable();
            _input.Player.Move.performed += ctx => Direction = ctx.ReadValue<Vector2>();
            _input.Player.Move.canceled += ctx => Direction = Vector2.zero;

        }
        private void OnDisable()
        {
            _input.Disable();
        }

        private void Update()
        {
            JumpPressed = _input.Player.Jump.WasPressedThisFrame();
        }
    }


}