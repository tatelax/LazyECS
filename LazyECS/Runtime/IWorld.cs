using System;
using System.Collections.Generic;
using LazyECS.Component;
using LazyECS.Entity;

namespace LazyECS
{
	public interface IWorld
	{
		Dictionary<int, Entity.Entity> Entities { get; }
		void Init();
		Entity.Entity CreateEntity(int id = default);
		bool DestroyEntity(Entity.Entity entity);
		List<Group> Groups { get; }
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