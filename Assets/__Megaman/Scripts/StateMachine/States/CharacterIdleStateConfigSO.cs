using UnityEngine;

namespace Megaman
{
    [CreateAssetMenu(fileName = "CharacterIdleStateConfig", menuName = "Scriptable Objects/States/CharacterIdleStateConfigSO")]
    public class CharacterIdleStateConfigSO : StateConfigSO
    {
        public override bool IsMatchingCondition()
        {
            return _input.Direction == Vector2.zero;
        }
    }
}