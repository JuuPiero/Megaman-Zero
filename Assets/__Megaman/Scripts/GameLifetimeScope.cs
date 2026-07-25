using Megaman;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {

        //builder.Register<InputManager, InputManager>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<InputManager>();
        builder.RegisterEntryPoint<GameManager>();
    }
}
