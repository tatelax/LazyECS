using System;
using System.Collections.Generic;
using LazyECS.Component;
using LazyECS.Entity;

namespace LazyECS
{
	public enum GroupType
	{
		Any,
		All
	}
	
	public interface IGroup
	{
		GroupType GroupType { get; }
		HashSet<IEntity> Entities { get; }
		HashSet<IComponent> Filters { get; }
		void ComponentAddedToEntity(IEntity entity, IComponent component);
		void ComponentRemovedFromEntity(IEntity entity, IComponent component);
	}
}