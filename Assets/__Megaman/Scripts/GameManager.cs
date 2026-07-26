using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Megaman
{
    public class GameManager : MonoBehaviour
    {


        [ShowInInspector] public List<IManager> managers;


        private void Awake()
        {

            managers = new(GetComponentsInChildren<IManager>(true));
            // Firebase.Analytics.FirebaseAnalytics.LogEvent("new_game", "time", DateTime.Now.ToBinary());
        }
        private void OnEnable()
        {

        }
        private void OnDisable()
        {

        }

        void OnNewGame()
        {

        }
    }
}