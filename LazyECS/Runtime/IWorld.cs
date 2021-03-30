using System;
using System.Collections.Generic;
using MirrorECS.Entity;

namespace MirrorECS
{
	public interface IWorld
	{
		TEntity CreateEntity<TEntity>() where TEntity : IEntity, new();
		List<Group> Groups { get; }
		void Start();
		void Update();
		Group CreateGroup(Type[] filters);
	}
}