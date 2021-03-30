using System.Collections.Generic;
using LazyECS.Component;

namespace LazyECS.Entity
{
	public delegate void ComponentAdded(IEntity entity);
	
	public abstract class Entity : IEntity
	{
		public HashSet<IComponent> Components { get; }

		public event ComponentAdded OnComponentAdded;
		
		public Entity()
		{
			Components = new HashSet<IComponent>();
		}
		
		public void Add<TComponent>() where TComponent : IComponent, new()
		{
			TComponent component = new TComponent();
			Components.Add(component);
			OnComponentAdded?.Invoke(this);
		}

		public IComponent Get<TComponent>() where TComponent : IComponent
		{
			//TODO: Slow...maybe use dictionary for storing components
			foreach (IComponent component in Components)
			{
				if (component is TComponent)
				{
					return component;
				}
			}
			
			return null;
		}
	}
}