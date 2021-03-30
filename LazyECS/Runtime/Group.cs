using System;
using System.Collections.Generic;
using LazyECS.Entity;

namespace LazyECS
{
	public class Group : IGroup
	{
		public HashSet<IEntity> Entities { get; }
		public Type[] Filters { get; }

		public Group(Type[] filters)
		{
			Filters = filters;
			Entities = new HashSet<IEntity>();
		}
		
		public void Update(IEntity entity)
		{
			if (Entities.Contains(entity))
				return;
			
			for (int i = 0; i < entity.Components.Count; i++)
			{
				Type type = entity.Components[i].GetType();
				
				for (int i1 = 0; i1 < Filters.Length; i1++)
				{
					if(type == Filters[i1])
					{
						Entities.Add(entity);
						break;
					}
				}
			}
		}
	}
}