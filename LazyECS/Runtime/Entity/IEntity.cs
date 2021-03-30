using System;
using System.Collections.Generic;

namespace MirrorECS.Entity
{
	public interface IEntity
	{
		List<IComponent> Components { get; }
		event ComponentAdded OnComponentAdded;
	}
}