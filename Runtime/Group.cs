using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LazyECS
{
	public class Group : IGroup, IDisposable
	{
		public delegate void OnEntityUpdate(EventType _eventType, Entity.Entity entity, Type component);

		public HashSet<Entity.Entity> Entities { get; }
		public HashSet<Type> Filters { get; } // We have to use Type because we aren't storing instances of IComponents, only their type
		public GroupType GroupType { get; }
		public EventType EventType { get; }

		public event OnEntityUpdate OnEntityUpdateEvent;

		public Group(GroupType groupType, HashSet<Type> filters, EventType _eventType, OnEntityUpdate onEntityUpdate)
		{
			Filters = filters;
			EventType = _eventType;
			OnEntityUpdateEvent += onEntityUpdate;
			Entities = new HashSet<Entity.Entity>();
			GroupType = groupType;
		}

		public void Dispose()
		{
			OnEntityUpdateEvent = null;
		}

		public void EntitySet(Entity.Entity entity, Type component)
		{
			if (Entities.Contains(entity) && Filters.Contains(component))
			{
				if(EventType == EventType.Set || EventType == EventType.All)
					OnEntityUpdateEvent?.Invoke(EventType.Set, entity, component);
			}
		}

		public void EntityDestroyed(Entity.Entity entity)
		{
			if (Entities.Contains(entity))
				Entities.Remove(entity);
		}

		public void ComponentAddedToEntity(Entity.Entity entity, Type component)
		{
			if (Entities.Contains(entity)) return;

			if (GroupType == GroupType.Any && Filters.Contains(component))
			{
				Entities.Add(entity);
				
				if(EventType == EventType.Added || EventType == EventType.All)
					OnEntityUpdateEvent?.Invoke(EventType.Added, entity, component);
				
				return;
			}

			int matches = entity.Components.Sum(cmp => Filters.Count(filter => cmp.Key == filter));

			if (matches != Filters.Count) return;
			
			Entities.Add(entity);
			
			if(EventType == EventType.Added || EventType == EventType.All)
				OnEntityUpdateEvent?.Invoke(EventType.Added, entity, component);
		}

		public void ComponentRemovedFromEntity(Entity.Entity entity, Type component)
		{
			if (GroupType == GroupType.All && Filters.Contains(component))
			{
				Entities.Remove(entity);
				
				if(EventType == EventType.Removed || EventType == EventType.All)
					OnEntityUpdateEvent?.Invoke(EventType.Removed, entity, component);
				
				return;
			}

			if (entity.Components.Any(cmp => Filters.Contains(cmp.Key)))
				return;

			Entities.Remove(entity);
			
			if(EventType == EventType.Removed || EventType == EventType.All)
				OnEntityUpdateEvent?.Invoke(EventType.Removed, entity, component);
		}
	}
}