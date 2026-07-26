using UnityEngine;

namespace Megaman
{
    [CreateAssetMenu(fileName = "CharacterFallStateSO", menuName = "Scriptable Objects/States/CharacterFallState")]
    public class CharacterFallStateSO : StateConfigSO
    {
        public override bool IsMatchingCondition()
        {
            return _owner.RB.linearVelocity.y < 0.1f && !_owner.isGrounded;
        }
    }
}