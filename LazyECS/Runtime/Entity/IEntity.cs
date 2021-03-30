using System.Collections.Generic;
using LazyECS.Component;

namespace LazyECS.Entity
{
	public interface IEntity
	{
		HashSet<IComponent> Components { get; }
		event ComponentAdded OnComponentAdded;
		void Add<TComponent>() where TComponent : IComponent, new();
		IComponent Get<TComponent>() where TComponent : IComponent;
	}
}