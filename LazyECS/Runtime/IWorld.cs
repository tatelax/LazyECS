using System;
using System.Collections.Generic;
using LazyECS.Entity;

namespace LazyECS
{
	public interface IWorld
	{
		TEntity CreateEntity<TEntity>() where TEntity : IEntity, new();
		List<Group> Groups { get; }
		void Start();
		void Update();
		void Teardown();
		void Cleanup();
		Group CreateGroup(GroupType groupType, Type[] filters);
	}
}