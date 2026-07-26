using UnityEngine;

namespace Megaman
{

    [CreateAssetMenu(fileName = "CharacterJumpAttackSO", menuName = "Scriptable Objects/States/CharacterJumpAttackSO")]
    public class CharacterJumpAttackSO : StateConfigSO
    {
        public override bool IsMatchingCondition()
        {
            return !_owner.isGrounded && _input.IsAttackPressed;
        }
        public override void Enter()
        {
            base.Enter();
            _state.Speed = 2;
            canExit = false;
        }
        public override void AnimationFinish()
        {
            _state.Speed = 1;
            canExit = true;
        }
    }
}