using UnityEngine;

namespace Megaman
{
    public interface ICharacter
    {


        void Move(Vector2 direction);
        void Jump();
    }
}