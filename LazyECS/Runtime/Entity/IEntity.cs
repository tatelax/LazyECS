using System.Collections.Generic;
using LazyECS.Component;

namespace LazyECS.Entity
{
	public interface IEntity
	{
		HashSet<IComponent> Components { get; }
		event ComponentAdded OnComponentAdded;
	}
}