using System.Collections.Generic;

namespace MirrorECS.Entity
{
	public delegate void ComponentAdded(IEntity entity);
	
	public abstract class Entity : IEntity
	{
		public List<IComponent> Components { get; }

		public event ComponentAdded OnComponentAdded;
		
		public Entity()
		{
			Components = new List<IComponent>();
		}
		
		public void AddComponent<TComponent>() where TComponent : IComponent, new()
		{
			TComponent component = new TComponent();
			Components.Add(component);
			OnComponentAdded?.Invoke(this);
		}
	}
}