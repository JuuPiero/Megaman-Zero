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
        
      
        public virtual void Initialize(Animancer.AnimancerComponent animancer, InputManager input)
        {
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