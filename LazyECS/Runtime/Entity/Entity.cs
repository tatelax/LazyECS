using System;
using System.Collections.Generic;
using LazyECS.Component;

namespace LazyECS.Entity
{
	public delegate void ComponentAdded(IEntity entity);
	
	public abstract class Entity : IEntity
	{
		public Dictionary<Type, IComponent> Components { get; }

		public event ComponentAdded OnComponentAdded;
		
		public Entity()
		{
			Components = new Dictionary<Type, IComponent>();
		}
		
		public void Add<TComponent>() where TComponent : IComponent, new()
		{
			TComponent component = new TComponent();
			Components.Add(component.GetType(), component);
			OnComponentAdded?.Invoke(this);
		}

		public IComponent Get<TComponent>() where TComponent : IComponent
		{
			return Components[typeof(TComponent)];
		}

		public bool Has<TComponent>() where TComponent : IComponent
		{
			return Components.ContainsKey(typeof(TComponent));
		}
	}
}