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
		HashSet<Entity.Entity> Entities { get; }
		HashSet<Type> Filters { get; }
		void ComponentAddedToEntity(Entity.Entity entity, Type component);
		void ComponentRemovedFromEntity(Entity.Entity entity, Type component);
	}
}