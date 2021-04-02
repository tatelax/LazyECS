using System;
using System.Collections.Generic;
using LazyECS.Component;
using LazyECS.Entity;

namespace LazyECS
{
	public interface IWorld
	{
		Dictionary<int, IEntity> Entities { get; }
		TEntity CreateEntity<TEntity>() where TEntity : IEntity, new();
		bool DestroyEntity(Entity.Entity entity);
		List<Group> Groups { get; }
		void Start();
		void Update();
		void Teardown();
		void Cleanup();
		Group CreateGroup(GroupType groupType, HashSet<IComponent> filters);
	}
}