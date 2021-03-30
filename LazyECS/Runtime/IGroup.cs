using System;
using System.Collections.Generic;
using LazyECS.Entity;

namespace LazyECS
{
	public interface IGroup
	{
		HashSet<IEntity> Entities { get; }
		Type[] Filters { get; }
		void Update(IEntity entity);
	}
}