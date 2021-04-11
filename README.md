# About

LazyECS is an ECS framework designed to be quick to learn and implement while still remaining performant and scalable.

# Features

* No codegen
* Simple
* Easy to learn (faster onboarding)
* No Unity dependency (Excluding Debug.Log and WorldDebugger)

# How to Install
LazyECS can be installed via the Unity Package Manager.

# Usage

See [Wiki](https://github.com/tatelax/LazyECS/wiki/)

# Addons

### [LazyECS.Networking](https://github.com/tatelax/LazyECS.Networking)
This addon provides the ability to synchronize entities and their component data across the networking using [Mirror](https://github.com/vis2k/Mirror)

# Example Project

[Here's an example project in Unity 2019.](https://github.com/tatelax/LazyECSExample)

# Known Issues

* No multi-threading (still plenty of room for optimization without affecting API).
* No way to access components with a specific values easily. It can be done by creating a group and looping through the entities in that group.
* No "unique" components like in Entitas.
* Not much error handling.
* No unit tests.


# Important Info

Lazy ECS is not production ready. There might be bugs.

# Support

You can contact me on Discord: ```tatelax#0001```