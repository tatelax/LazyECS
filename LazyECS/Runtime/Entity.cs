using System;
using System.Collections.Generic;
using LazyECS.Component;

namespace LazyECS.Entity
{
	public delegate void ComponentAdded(IEntity entity);
	public delegate void ComponentRemoved(IEntity entity);
	
	public abstract class Entity : IEntity
	{
		public Dictionary<Type, IComponent> Components { get; }

		public event ComponentAdded OnComponentAdded;
		public event ComponentRemoved OnComponentRemoved;
		
		public Entity()
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
			return (TComponent) Components[typeof(TComponent)];
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
	}
}