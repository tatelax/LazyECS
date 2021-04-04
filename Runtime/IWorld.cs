using System;
using System.Collections.Generic;
using LazyECS.Component;
using LazyECS.Entity;

namespace LazyECS
{
	public interface IWorld
	{
		Feature[] Features { get; }
		Dictionary<int, Entity.Entity> Entities { get; }
		List<Group> Groups { get; }
		Entity.Entity CreateEntity(int id = default);
		bool DestroyEntity(int id);
		bool DestroyEntity(Entity.Entity entity);
		void DestroyAllEntities();
		void Start();
		void Update();
		void Teardown();
		void Cleanup();
		void OnEntityCreated(Entity.Entity entity);
		void OnComponentAddedToEntity(Entity.Entity entity, IComponent component);
		void OnComponentRemovedFromEntity(Entity.Entity entity, IComponent component);
		void OnComponentSetOnEntity(Entity.Entity entity, IComponent component);
		Group CreateGroup(GroupType groupType, HashSet<Type> filters);
	}
}