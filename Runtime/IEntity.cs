﻿using System;
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
		
		TComponent Add<TComponent>() where TComponent : IComponent, new();
		TComponent Get<TComponent>() where TComponent : IComponent;
		bool Has<TComponent>() where TComponent : IComponent;
		void Remove<TComponent>() where TComponent : IComponent;
		void Replace<TComponent>() where TComponent : IComponent, new();
		void Set<TComponent>(object value, bool setFromNetworkMessage = false) where TComponent : IComponent, new();
	}
}