using UnityEngine;

namespace Megaman
{

    
    public abstract class StateConfigSO : ScriptableObject, IState
    {
        public string stateName;
         public StateLayer stateLayer = StateLayer.Base; // Layer mặc định
        public AnimationClip animationClip;
        public AnimationClip endAnimationClip;

        public float crossFadeDuration = 0.25f;
         [Header("Transition Priority")]
        [Range(0, 100)] public int priority = 0; // Độ ưu tiên, cao hơn sẽ override
        protected InputManager _input;
        protected Animancer.AnimancerComponent _animancer;
        protected BaseCharacter _owner;

        public virtual void Initialize(BaseCharacter owner, Animancer.AnimancerComponent animancer, InputManager input)
        {
            _owner = owner;
            _animancer = animancer;
            _input = input;
        }


        public void Enter()
        {
            _animancer.Play(animationClip, crossFadeDuration);
        }

        public void Exit()
        {
            if (endAnimationClip != null) 
            {
                _animancer.Play(endAnimationClip);
            }
        }

        public void FixedUpdate()
        {
        }

        public void Update()
        {
        }

        public virtual bool IsMatchingCondition()
        {
            return false;
        }
    }
}