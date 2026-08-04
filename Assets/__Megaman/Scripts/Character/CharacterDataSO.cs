using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "Scriptable Objects/CharacterDataSO")]
public class CharacterDataSO : ScriptableObject
{
    public float speed = 10f;
    public float jumpForce = 12f;

    public float attackCoolDown = 0.5f;
}
