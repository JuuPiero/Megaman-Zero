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
}