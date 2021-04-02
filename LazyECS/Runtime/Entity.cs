using System;
using System.Collections.Generic;
using LazyECS.Component;
using UnityEngine;

namespace LazyECS.Entity
{
	public delegate void ComponentAdded(IEntity entity, IComponent component);
	public delegate void ComponentRemoved(IEntity entity, IComponent component);
	public delegate void ComponentSet(IEntity entity, IComponent component);
	
	public abstract class Entity : IEntity
	{
		public int id { get; }
		public Dictionary<Type, IComponent> Components { get; }

		public event ComponentAdded OnComponentAdded;
		public event ComponentRemoved OnComponentRemoved;
		public event ComponentSet OnComponentSet;

		protected Entity()
		{
			id = new System.Random().Next(0,9999999);
			Components = new Dictionary<Type, IComponent>();
		}
		
		public TComponent Add<TComponent>() where TComponent : IComponent, new()
		{
			TComponent component = new TComponent();
			Components.Add(component.GetType(), component);
			OnComponentAdded?.Invoke(this, component);
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
			IComponent component = Components[typeof(TComponent)];
			Components.Remove(component.GetType());
			OnComponentRemoved?.Invoke(this, component);
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
			OnComponentSet?.Invoke(this, Components[compType]);
		}
	}
}