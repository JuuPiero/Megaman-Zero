using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Windows;

namespace Megaman
{
    public class BaseCharacter : MonoBehaviour, ICharacter
    {

        public bool isFacingRight = true;
        public LayerMask groundLayer;
        public bool isGrounded = false;
        public Transform groundCheckPoint;
        [field: SerializeField] public Rigidbody RB { get; protected set; }

        [SerializeField] protected CharacterDataSO _data;

        public GameObject visual;

        public StateMachine stateMachine;

        public Animancer.AnimancerComponent animancer;
        protected InputManager _input;



        public void Initialize(InputManager input)
        {
            _input = input;
            // animancer = visual.GetComponent<Animancer.AnimancerComponent>();
            // stateMachine?.Initialize(this, input);
        }

        public void Move(Vector2 direction)
        {
            // RB.linearVelocity = new Vector3(_data.speed * direction.x, RB.linearVelocity.y, 0);
            RB.linearVelocity = new Vector3(_data.speed * direction.x, RB.linearVelocity.y, direction.y * _data.speed);
        }

        public void Jump()
        {
            if(isGrounded)
            {
                RB.linearVelocity = new Vector3(RB.linearVelocity.x, _data.jumpForce, 0);
            }
        }
        protected virtual void FixedUpdate()
        {
            isGrounded = Physics.CheckSphere(
                groundCheckPoint.position,
                0.3f,
                groundLayer
            );
        }


        public void HandleFlip()
        {
            if (_input.Direction.sqrMagnitude <= 0.01f)
            {
                RB.angularVelocity = Vector3.zero;
                return;
            }

            if (_input.Direction.sqrMagnitude > 0.01f)
            {
                // Input is on XY, while character movement and yaw use the XZ plane.
                Vector3 moveDirection = new Vector3(_input.Direction.x, 0f, _input.Direction.y);
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                // Xoay dần cho mượt
                Quaternion smoothRotation = Quaternion.Slerp(
                    RB.rotation,
                    targetRotation,
                    Time.fixedDeltaTime * 10f);
                RB.MoveRotation(smoothRotation);
            }
        }


    }
}
