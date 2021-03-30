using System;
using System.Collections.Generic;
using LazyECS.Component;

namespace LazyECS.Entity
{
	public interface IEntity
	{
		Dictionary<Type, IComponent> Components { get; }
		event ComponentAdded OnComponentAdded;
		event ComponentRemoved OnComponentRemoved;
		void Add<TComponent>() where TComponent : IComponent, new();
		IComponent Get<TComponent>() where TComponent : IComponent;
		bool Has<TComponent>() where TComponent : IComponent;
		void Remove<TComponent>() where TComponent : IComponent;
		void Replace<TComponent>() where TComponent : IComponent, new();
	}
}