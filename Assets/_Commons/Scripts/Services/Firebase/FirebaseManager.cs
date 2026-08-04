using UnityEngine;

namespace Megaman
{
    public enum FirebaseState { NotReady, Initializing, Ready, Failed }
    public class FirebaseManager : MonoBehaviour, IManager
    {
        void Awake()
        {
            Initialize();
        }
        public static FirebaseState State { get; private set; }
        public async void Initialize(params object[] parameters)
        {
            // State = FirebaseState.Initializing;
            // try
            // {
            //     var status = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
            //     if (status == Firebase.DependencyStatus.Available)
            //     {
            //         // await InitializeModules();
            //         State = FirebaseState.Ready;
            //         Debug.Log("Firebase Ready");
            //     }
            // }
            // catch
            // {
            //     State = FirebaseState.Failed;
            // }
        }
    }
}