using System;
using System.Collections.Generic;
using LazyECS.Component;
using UnityEngine;

namespace LazyECS.Entity
{
	public delegate void ComponentAdded(IEntity entity);
	public delegate void ComponentRemoved(IEntity entity);
	public delegate void ComponentSet(IEntity entity);
	
	public abstract class Entity : IEntity
	{
		public Dictionary<Type, IComponent> Components { get; }

		public event ComponentAdded OnComponentAdded;
		public event ComponentRemoved OnComponentRemoved;
		public event ComponentSet OnComponentSet;

		protected Entity()
		{
			Components = new Dictionary<Type, IComponent>();
		}
		
		public TComponent Add<TComponent>() where TComponent : IComponent, new()
		{
			TComponent component = new TComponent();
			Components.Add(component.GetType(), component);
			OnComponentAdded?.Invoke(this);
			return component;
		}

		public TComponent Get<TComponent>() where TComponent : IComponent
		{
			Type compType = typeof(TComponent);
			if (!Components.ContainsKey(compType))
			{
				Debug.LogWarning($"Tried to access component {compType} but the entity didn't have it!");
				return default;
			}
			
			return (TComponent) Components[compType];
		}
		
		public bool Has<TComponent>() where TComponent : IComponent
		{
			return Components.ContainsKey(typeof(TComponent));
		}
		
		public void Remove<TComponent>() where TComponent : IComponent
		{
			Components.Remove(typeof(TComponent));
			OnComponentRemoved?.Invoke(this);
		}
		
		public void Replace<TComponent>() where TComponent : IComponent, new()
		{
			Remove<TComponent>();
			Add<TComponent>();
		}

		public void Set<TComponent>(object value) where TComponent : IComponent, new()
		{
			Type compType = typeof(TComponent);

			if (!Components.ContainsKey(compType))
			{
				Add<TComponent>();
			}
			
			Components[compType].Set(value);
			OnComponentSet?.Invoke(this);
		}
	}
}