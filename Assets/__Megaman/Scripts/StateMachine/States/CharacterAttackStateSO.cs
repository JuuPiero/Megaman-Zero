using Animancer;
using UnityEngine;

namespace Megaman
{
    [CreateAssetMenu(fileName = "CharacterAttackStateSO", menuName = "Scriptable Objects/States/CharacterAttackStateSO")]
    public class CharacterAttackStateSO : StateConfigSO
    {
        public override bool IsMatchingCondition()
        {
            return _input.IsAttackPressed && _owner.isGrounded;
        }

      
        public override void AnimationFinish()
        {
            canExit = true;
        }
    }
}