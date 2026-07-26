using UnityEngine;
namespace Megaman
{
    public class InputManager : MonoBehaviour, IManager
    {
        //public PlayerControls _input;
        private const float HOLD_THRESHOLD = 0.25f;
        private InputSystem_Actions _input;

        public bool JumpPressed { get; protected set; }

        public Vector2 Direction { get; protected set; }

        [SerializeField] private float _attackPressTime;
        public bool IsHoldingAttack { get; protected set; }
        public bool IsAttackPressed { get; protected set; }

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

            _input.Player.Attack.started += ctx => StartAttack();
            _input.Player.Attack.canceled += ctx => ReleaseAttack();

        }
        private void OnDisable()
        {
            _input.Disable();
        }

        private void Update()
        {
            JumpPressed = _input.Player.Jump.WasPressedThisFrame();

            if (IsAttackPressed && (Time.time - _attackPressTime) >= HOLD_THRESHOLD)
            {
                IsHoldingAttack = true; // Chuyển sang heavy attack nếu giữ lâu
                IsAttackPressed = false;
            }
        }


        private void StartAttack()
        {
            _attackPressTime = Time.time;
            IsAttackPressed = true;
            IsHoldingAttack = false;
        }

        private void ReleaseAttack()
        {
            float holdDuration = Time.time - _attackPressTime;

            if (holdDuration >= HOLD_THRESHOLD)
            {
                IsHoldingAttack = false;
            }
            IsAttackPressed = false;
            IsHoldingAttack = false;
        }
    }


}