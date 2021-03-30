using System;
using System.Collections.Generic;
using MirrorECS.Entity;

public interface IGroup
{
	HashSet<IEntity> Entities { get; }
	Type[] Filters { get; }
	void Update(IEntity entity);
}