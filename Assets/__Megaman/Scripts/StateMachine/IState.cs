using System;
using UnityEngine;

namespace Megaman
{
    public interface IState
    {
        void Enter();
        void Exit();
        void Update();
        void FixedUpdate();

        bool IsMatchingCondition();

    }


    public enum ConditionType
    {
        Less,
        Greater,
        Has,
        AnimationFinished,
        TimerElapsed
    }

// [Serializable]
// public class Condition
// {
//     public ConditionType type;

//     public float value;
//     public string key;
// }




//     [Serializable]
// public class Transition
// {
//     public StateType TargetState;
//     public Condition Condition;
// }
}

