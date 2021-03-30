using System;
using System.Collections.Generic;
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
		Type[] Filters { get; }
		void ComponentAddedOrRemovedFromSomeEntity(IEntity entity);
	}
}