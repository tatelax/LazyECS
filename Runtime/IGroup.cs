using System;
using System.Collections.Generic;

namespace LazyECS
{
	public enum GroupType
	{
		Any,
		All
	}
	
	public interface IGroup
	{
		event Group.OnEntityAdded OnEntityAddedEvent;
		event Group.OnEntityRemoved OnEntityRemovedEvent;
		event Group.OnEntitySet OnEntitySetEvent;
			
		GroupType GroupType { get; }
		HashSet<Entity.Entity> Entities { get; }
		HashSet<Type> Filters { get; }
		
		void EntitySet(Entity.Entity entity, Type component);
		void EntityDestroyed(Entity.Entity entity);
		void ComponentAddedToEntity(Entity.Entity entity, Type component);
		void ComponentRemovedFromEntity(Entity.Entity entity, Type component);
	}
}