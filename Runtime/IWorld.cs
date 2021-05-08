using System;
using System.Collections.Generic;
using LazyECS.Component;

namespace LazyECS
{
	public interface IWorld
	{
		Feature[] Features { get; }
		Dictionary<int, Entity.Entity> Entities { get; }
		List<Group> Groups { get; }
		
		event World.EntityCreatedEvent OnEntityCreatedEvent;
		event World.EntityDestroyedEvent OnEntityDestroyedEvent;
		event World.ComponentAddedToEntity OnComponentAddedToEntityEvent;
		event World.ComponentRemovedFromEntity OnComponentRemovedFromEntityEvent;
		event World.ComponentSetOnEntity OnComponentSetOnEntityEvent;
		
		Entity.Entity CreateEntity(int id = -1, bool entityCreatedFromNetworkMessage = false);
		bool DestroyEntity(int id, bool entityDestroyedFromNetworkMessage = false);
		bool DestroyEntity(Entity.Entity entity, bool entityDestroyedFromNetworkMessage = false);
		void DestroyAllEntities();

		void Initialize();
		void Start();
		void Update();
		void Teardown();
		void Cleanup();
		
		Group CreateGroup(GroupType groupType, HashSet<Type> filters, bool checkExisting = true);
		
		LazyECS.Entity.Entity GetEntity(int id);
		List<Entity.Entity> GetEntities<TComponent>() where TComponent : IComponent;
		List<Entity.Entity> GetEntities<TComponent>(object value) where TComponent : IComponent;
		
		void OnEntityCreated(Entity.Entity entity, bool entityCreatedFromNetworkMessage);
		void OnEntityDestroyed(Entity.Entity entity, bool entityDestroyedFromNetworkMessage = false);
		void OnComponentAddedToEntity(Entity.Entity entity, IComponent component);
		void OnComponentRemovedFromEntity(Entity.Entity entity, IComponent component);
		void OnComponentSetOnEntity(Entity.Entity entity, IComponent component, bool setFromNetworkMessage);
	}
}