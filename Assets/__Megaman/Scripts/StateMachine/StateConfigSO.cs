using Animancer;
using UnityEngine;

namespace Megaman
{

    
    public abstract class StateConfigSO : ScriptableObject, IState
    {
        public string stateName;
        public StateLayer stateLayer = StateLayer.Base; // Layer mặc định
        public AnimationClip animationClip;
        public AnimationClip endAnimationClip;

        public bool canExit = true;
        public float speed = 1;

        public float crossFadeDuration = 0.25f;
        [Header("Transition Priority")]
        [Range(0, 100)] public int priority = 0; // Độ ưu tiên, cao hơn sẽ override
        protected InputManager _input;
        protected Animancer.AnimancerComponent _animancer;
        protected BaseCharacter _owner;

        protected AnimancerState _state;

        public virtual void Initialize(BaseCharacter owner, Animancer.AnimancerComponent animancer, InputManager input)
        {
            _owner = owner;
            _animancer = animancer;
            _input = input;
        }


        public virtual void Enter()
        {
            _state = _animancer.Play(animationClip, crossFadeDuration);
            _state.Speed = speed;
            _state.Events.OnEnd += AnimationFinish;    
        }

        public virtual void Exit()
        {
            _state.Events.OnEnd -= AnimationFinish;         
            if (endAnimationClip != null) 
            {
                _animancer.Play(endAnimationClip);
            }
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void Update()
        {
        }

        public virtual bool IsMatchingCondition()
        {
            return false;
        }

        public virtual void AnimationFinish()
        {
        }
    }
}