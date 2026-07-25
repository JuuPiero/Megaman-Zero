using UnityEngine;

namespace Megaman
{
    
    [CreateAssetMenu(fileName = "CharacterRunStateSO", menuName = "Scriptable Objects/States/CharacterRunStateSO")]
    public class CharacterRunStateSO : StateConfigSO
    {
        public override bool IsMatchingCondition()
        {
            return _input.Direction != Vector2.zero;
        }
    }
}