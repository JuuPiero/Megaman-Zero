using UnityEngine;
using UnityEngine.Windows;

namespace Megaman
{
    public abstract class BaseCharacter : MonoBehaviour, ICharacter
    {

        public bool isFacingRight = true;
        public bool isGrounded = false;

        [field: SerializeField] public Rigidbody RB { get; protected set; }

        [SerializeField] protected CharacterDataSO _data;

        public GameObject visual;

        public CharacterStateMachine stateMachine;

        public Animancer.AnimancerComponent animancer;

        protected InputManager _input;




        public void Initialize(InputManager input)
        {
            _input = input;
            animancer = visual.GetComponent<Animancer.AnimancerComponent>();
            stateMachine.Initialize(this, input);
        }

        public void Move(Vector2 direction)
        {
            RB.linearVelocity = new Vector3(_data.speed * direction.x, RB.linearVelocity.y, 0);
        }


        public void Jump()
        {
            RB.linearVelocity = new Vector3(RB.linearVelocity.x, _data.jumpForce, 0);
        }


        public void HandleFlip()
        {
            if(_input.Direction.x > 0 && !isFacingRight)
            {
                visual.transform.eulerAngles = new Vector3(0, 100f, 0);
                isFacingRight = true;

            }
            else if(_input.Direction.x < 0 && isFacingRight)
            {
                visual.transform.eulerAngles = new Vector3(0, -100f, 0);
                isFacingRight = false;
            }
        }

        
    }
}