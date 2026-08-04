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
            Debug.Log("hello world 123");
        }


        private void Update()
        {
            if(_input.JumpPressed)
            {
                currentCharacter?.Jump();
            }

            if(Input.GetKeyDown(KeyCode.Alpha0))
            {
                ChangeCharacter(0);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeCharacter(1);
            }
        }

        private void FixedUpdate()
        {
            currentCharacter?.Move(_input.Direction);
            currentCharacter?.HandleFlip();

        }


        void ChangeCharacter(int index)
        {
            try
            {
                currentCharacter.gameObject.SetActive(false);
                Vector3 pos = currentCharacter.transform.position;
                currentCharacter = characters[index];
                currentCharacter.transform.position = pos;
                currentCharacter.gameObject.SetActive(true);
            }
            catch (System.Exception)
            {
                
                return;
            }
           
        }
    }
}
