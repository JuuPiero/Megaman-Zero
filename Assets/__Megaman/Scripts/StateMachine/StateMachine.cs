using System.Collections.Generic;
using Animancer;
using UnityEngine;

namespace Megaman
{
    public class StateMachine : MonoBehaviour
    {
        public List<StateConfigSO> allStates = new List<StateConfigSO>();
        protected Dictionary<string, StateConfigSO> _stateDict = new Dictionary<string, StateConfigSO>();
        public AnimancerComponent animancer;

        public StateConfigSO currentState;
        
        public BaseCharacter _owner;
      
        public virtual void Initialize(BaseCharacter owner, InputManager input)
        {
            _owner = owner;
            animancer = _owner.visual.GetComponent<AnimancerComponent>();
            allStates.ForEach(state =>
            {
                state.Initialize(GetComponent<BaseCharacter>() , animancer, input);
            });

            ChangeState(allStates[0]);
        }

        public virtual void ChangeState(StateConfigSO newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }

        protected virtual void Update()
        {
            if(!currentState.canExit) return;
            foreach (var state in allStates)
            {
                if(state != currentState && state.IsMatchingCondition())
                {
                    ChangeState(state);
                }
            }
        }
    }
}