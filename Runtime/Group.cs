using System;
using System.Collections.Generic;
using LazyECS.Component;
using UnityEngine;

namespace LazyECS
{
	public class Group : IGroup
	{
		public HashSet<Entity.Entity> Entities { get; }
		public HashSet<Type> Filters { get; }
		public GroupType GroupType { get; }

		public Group(GroupType groupType, HashSet<Type> filters)
		{
			Filters = filters;
			Entities = new HashSet<Entity.Entity>();
			GroupType = groupType;
		}

		public void EntityDestroyed(Entity.Entity entity)
		{
			if (Entities.Contains(entity))
				Entities.Remove(entity);
		}

		public void ComponentAddedToEntity(Entity.Entity entity, Type component)
		{
			if (Entities.Contains(entity)) return;

			if (GroupType == GroupType.Any)
			{
				if (Filters.Contains(component))
				{
					Entities.Add(entity);
					return;
				}
			}

			int matches = 0;
			
			foreach (KeyValuePair<Type,IComponent> cmp in entity.Components)
			{
				foreach (Type filter in Filters)
				{
					if (cmp.Key == filter)
					{
						matches++;
					}
				}
			}

			if (matches == Filters.Count)
			{
				Entities.Add(entity);
			}
		}

		public void ComponentRemovedFromEntity(Entity.Entity entity, Type component)
		{
			if (GroupType == GroupType.All)
			{
				if (Filters.Contains(component))
				{
					Entities.Remove(entity);
					return;
				}
			}

			foreach (KeyValuePair<Type,IComponent> cmp in entity.Components)
			{
				if (Filters.Contains(cmp.Key))
				{
					return;
				}
			}

			Entities.Remove(entity);
		}
	}
}