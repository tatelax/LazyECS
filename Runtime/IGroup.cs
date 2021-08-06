using System;
using System.Collections.Generic;

namespace LazyECS
{
	public enum GroupType
	{
		Any,
		All
	}
	
	public enum EventType
	{
		Added,
		Set,
		Removed,
		All
	}
	
	public interface IGroup
	{
		GroupType GroupType { get; }
		HashSet<Entity.Entity> Entities { get; }
		HashSet<Type> Filters { get; }
		EventType EventType { get; }
		
		event Group.OnEntityUpdate OnEntityUpdateEvent;
		
		void EntitySet(Entity.Entity entity, Type component);
		void EntityDestroyed(Entity.Entity entity);
		void ComponentAddedToEntity(Entity.Entity entity, Type component);
		void ComponentRemovedFromEntity(Entity.Entity entity, Type component);
	}
}