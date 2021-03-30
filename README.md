# About

LazyECS is an ECS framework designed to be quick to learn and implement while still remaining performant and scalable.

# Features

* No codegen
* Simple
* Easy to learn (faster onboarding)
* No Unity dependency

# Systems

Systems are used to **Create** entities, **Delete** entities, and **Add/Remove** components to entities.

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
    public void Update() {
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
	public void Teardown() {
        Debug.Log("I tore down!");
    }
}
```

## Cleanup

Cleanup systems should be called after Update systems run.

```csharp
using LazyECS;
using UnityEngine;

public class FooCleanupSystem : ICleanupSystem
{   
    public void Cleanup() {
        Debug.Log("I cleaned up!");
    }
}
```

# Entities

Text

```csharp
public class FooEntity : Entity { }
```

# Components

Text

```csharp
using LazyECS;
using UnityEngine;

public class FooComponent : IComponent
{
    public string Value { get; }
}
```

# Important Info

* Lazy ECS is not production ready. There might be bugs.