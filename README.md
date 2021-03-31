# About

LazyECS is an ECS framework designed to be quick to learn and implement while still remaining performant and scalable.

# Features

* No codegen
* Simple
* Easy to learn (faster onboarding)
* No Unity dependency

# How to Install
LazyECS can be installed via the Unity Package Manager.

# Example Project

[Here's an example project in Unity 2019.](https://github.com/tatelax/LazyECSExample)

# Worlds

Entities, Systems, and Features exist inside of a world.
```csharp
public class MainWorld : World
{
    public override void Init()
    {
        features = new Feature[]
        {
            new TestFeature()
        };
    }
}
```
# Features
Systems are organized inside of features.
```csharp
public class FooFeature : Feature
{
    public override void Setup()
    {
        Systems = new LazyECS.Systems()
            .Add(new TestInitializeSystem())
            .Add(new TestUpdateSystem())
            .Add(new TestTeardownSystem())
            .Add(new TestCleanupSystem());
    }
}
```

# Systems

Systems are used to **Create** entities, **Delete** entities, and **Add/Remove** components to entities. They also can contain your applications business logic.

## Initialize

Initialize systems should be called at the start of the program (i.e. Start()/Awake()).

```csharp
using LazyECS;
using UnityEngine;

public class FooInitializeSystem : IInitializeSystem
{
    public void Initialize()
    {
        Debug.Log("Initialized!");
    }
}
```

## Update

Update systems should be called every frame (i.e. Update()).

```csharp
using LazyECS;
using UnityEngine;

public class FooUpdateSystem : IUpdateSystem
{    
    public void Update()
    {
        Debug.Log("Update!");
    }
}
```
## Teardown

Teardown systems should be called when your program quits (i.e. OnDisable()).

```csharp
using LazyECS;
using UnityEngine;

public class FooTeardownSystem : ITeardownSystem
{
    public void Teardown()
    {
        Debug.Log("I tore down!");
    }
}
```

## Cleanup

Cleanup systems should be called after Update systems run. These systems can be used to remove entities you no longer need. It's better to remove entities during the Cleanup phase so that loops are not manipulated while being iterated.

```csharp
using LazyECS;
using UnityEngine;

public class FooCleanupSystem : ICleanupSystem
{   
    public void Cleanup()
    {
        Debug.Log("I cleaned up!");
    }
}
```

# Entities

Entities are bits of memory used to store components.

```csharp
public class FooEntity : Entity { }
```

# Components

Components are used to store data.

```csharp
using LazyECS;

public class FooComponent : IComponent
{
    public string Value { get; private set; }
    
    public void Set(Vector3 pos) {
        Value = pos;
    }
}
```


# How To

## Create an Entity

Entities are created as part of a world. Access the world you want to create the entity in by any means you choose, such as **Dependency Injection**

```csharp
GameEntity newEntity = mainWorld.CreateEntity<GameEntity>
```
## Add a Component to an Entity

```csharp
newEntity.Add<PositionComponent>();
```
## Remove a Component from an Entity

```csharp
newEntity.Remove<PositionComponent>();
```
## Create a Group

```csharp
Group fooGroup = mainWorld.CreateGroup(GroupType.All, new []
{
	typeof(PositionComponent),
	typeof(HelloComponent)
});
```
## See if an Entity has a Particular Component

```csharp
gameEntity.Has<PositionComponent>();
```

# Important Info

* Lazy ECS is not production ready. There might be bugs.

# Support

You can contact me on Discord: ```tatelax#0001```