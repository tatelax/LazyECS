using System;
using System.Collections.Generic;
using LazyECS.Component;
using LazyECS.Entity;

namespace LazyECS
{
	public class Group : IGroup
	{
		public HashSet<IEntity> Entities { get; }
		public Type[] Filters { get; }
		public GroupType GroupType { get; }

		public Group(GroupType groupType, Type[] filters)
		{
			Filters = filters;
			Entities = new HashSet<IEntity>();
			GroupType = groupType;
		}
		
		public void ComponentAddedOrRemovedFromSomeEntity(IEntity entity)
		{
			int matches = 0;
			
			foreach (KeyValuePair<Type,IComponent> component in entity.Components)
			{
				for (int i1 = 0; i1 < Filters.Length; i1++)
				{
					if(component.Key == Filters[i1])
					{
						if (GroupType == GroupType.Any)
						{
							Entities.Add(entity);
							return;
						}
						
						matches++;
					}
				}
			}

			if (GroupType == GroupType.All)
			{
				if (matches != Filters.Length)
				{
					if (Entities.Contains(entity))
					{
						// The entity didnt contain any components we were looking for so remove it
						Entities.Remove(entity);
					}
				}
				else
				{
					Entities.Add(entity);
				}
			}
		}
	}
}