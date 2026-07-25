using Megaman;
using UnityEngine;

public class BaseManager : MonoBehaviour, IManager
{
    public int priority = 0;

    public void Initialize(params object[] parameters)
    {
    }
}
