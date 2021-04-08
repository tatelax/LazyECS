using System;
using System.Collections.Generic;
using LazyECS.Component;

namespace LazyECS.Entity
{
	internal interface IEntity
	{
		int id { get; }
		Dictionary<Type, IComponent> Components { get; }
		
		event ComponentAdded OnComponentAdded;
		event ComponentRemoved OnComponentRemoved;
		event ComponentSet OnComponentSet;

		IComponent Add(int componentId);
		TComponent Add<TComponent>() where TComponent : IComponent, new();
		IComponent Get(int id);
		IComponent Get(Type type);
		TComponent Get<TComponent>() where TComponent : IComponent;
		bool Has(int id);
		bool Has<TComponent>() where TComponent : IComponent;
		void Remove(int componentId);
		void Remove<TComponent>() where TComponent : IComponent;
		void Replace<TComponent>() where TComponent : IComponent, new();
		void Set<TComponent>(object value, bool setFromNetworkMessage = false) where TComponent : IComponent, new();
		void Set(int id, object value, bool setFromNetworkMessage = false);
	}
}