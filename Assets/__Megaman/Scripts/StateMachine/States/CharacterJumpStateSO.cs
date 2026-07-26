using UnityEngine;

namespace  Megaman
{
    
    [CreateAssetMenu(fileName = "CharacterJumpStateSO", menuName = "Scriptable Objects/States/CharacterJumpStateSO")]
    public class CharacterJumpStateSO : StateConfigSO
    {
        public override bool IsMatchingCondition()
        {
            return _owner.RB.linearVelocity.y > 0.1f && !_owner.isGrounded;
        }
    }
}