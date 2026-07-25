using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Megaman
{
    public class PlayerController : MonoBehaviour
    {
        public List<BaseCharacter> characters = null;
        public BaseCharacter currentCharacter = null;
        private InputManager _input;


        private void Start()
        {
            _input = ServiceLocator.Get<InputManager>();

            characters.ForEach(charater =>
            {
                charater.Initialize(_input);
            });
        }


        private void Update()
        {
            if(_input.JumpPressed)
            {
                currentCharacter?.Jump();
            }
            currentCharacter.HandleFlip();
        }

        private void FixedUpdate()
        {
            currentCharacter?.Move(_input.Direction);

        }
    }
}