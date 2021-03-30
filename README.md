# About

LazyECS is an ECS framework 



# Features

* No codegen
* Simple
* Easy to learn (faster onboarding)
* No Unity dependency



# Systems

Systems are used to **Create** entities, **Delete** entities, and **Add/Remove** components to entities.

## Initialize

Text

```csharp
using LazyECS;
using UnityEngine;

public class TestInitializeSystem : IInitializeSystem
{
    public TestInitializeSystem(MainWorld world)
    {
        mainWorld = world;
    }

    public void Initialize()
    {
        Debug.Log("Initialized!");
    }
}
```

## Update

Text

```Text```

## Teardown

Text

```Text```



# Entities

```Text```



# Components

Text

```Text```



# Important Info

* Lazy ECS is not production ready. There might be bugs.