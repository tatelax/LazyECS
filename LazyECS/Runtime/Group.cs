using System;
using System.Collections.Generic;
using LazyECS.Component;
using LazyECS.Entity;

namespace LazyECS
{
	public class Group : IGroup
	{
		public HashSet<IEntity> Entities { get; }
		public HashSet<IComponent> Filters { get; }
		public GroupType GroupType { get; }

		public Group(GroupType groupType, HashSet<IComponent> filters)
		{
			Filters = filters;
			Entities = new HashSet<IEntity>();
			GroupType = groupType;
		}

		public void ComponentAddedToEntity(IEntity entity, IComponent component)
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
				foreach (IComponent filter in Filters)
				{
					if (cmp.Value == filter)
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

		public void ComponentRemovedFromEntity(IEntity entity, IComponent component)
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
				if (Filters.Contains(cmp.Value))
				{
					return;
				}
			}

			Entities.Remove(entity);
		}
	}
}