using System.Collections.Generic;
using LazyECS.Component;

namespace LazyECS.Entity
{
	public interface IEntity
	{
		List<IComponent> Components { get; }
		event ComponentAdded OnComponentAdded;
	}
}