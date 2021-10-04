using System;
using System.Collections.Generic;
using LazyECS.Component;
using UnityEngine;

namespace LazyECS.Entity
{
	public delegate void ComponentAdded(Entity entity, IComponent component);
	public delegate void ComponentRemoved(Entity entity, IComponent component);
	public delegate void ComponentSet(Entity entity, IComponent component, bool setFromNetworkMessage);
	
	public class Entity : IEntity, IDisposable
	{
		public int id { get; }
		public Dictionary<Type, IComponent> Components { get; }

		public event ComponentAdded OnComponentAdded;
		public event ComponentRemoved OnComponentRemoved;
		public event ComponentSet OnComponentSet;

		public Entity(int _id = default)
		{
			//TODO: ID collision checking (proper id generation)
			id = _id;

			Components = new Dictionary<Type, IComponent>();
		}
		
		public void Dispose()
		{
			OnComponentAdded = null;
			OnComponentRemoved = null;
			OnComponentSet = null;
		}
		
		/// <summary>
		/// Add component by id
		/// </summary>
		/// <param name="componentId"></param>
		public IComponent Add(int componentId)
		{
			IComponent component = (IComponent)Activator.CreateInstance(ComponentLookup.Get(componentId));
			Components.Add(component.GetType(), component);
			
			OnComponentAdded?.Invoke(this, component);
			return component;
		}
		
		/// <summary>
		/// Add component by type
		/// </summary>
		/// <typeparam name="TComponent">Component type</typeparam>
		/// <returns></returns>
		public TComponent Add<TComponent>() where TComponent : IComponent, new()
		{
			TComponent component = new TComponent();
			Components.Add(component.GetType(), component);
			
			OnComponentAdded?.Invoke(this, component);
			return component;
		}

		public IComponent Get(int id)
		{
			Type compType = ComponentLookup.Get(id);
			if (!Components.ContainsKey(compType))
			{
				Debug.LogWarning($"Tried to access component {compType} but the entity didn't have it!");
				return default;
			}

			return Components[compType];
		}

		public IComponent Get(Type type)
		{
			if (!Components.ContainsKey(type))
			{
				Debug.LogWarning($"Tried to access component {type} but the entity didn't have it!");
				return default;
			}

			return Components[type];
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
		
		/// <summary>
		/// Check if entity has a component by type (param)
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal bool Has(Type type)
		{
			return Components.ContainsKey(type);
		}
		
		/// <summary>
		/// Check if entity has a component by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool Has(int id)
		{
			return Components.ContainsKey(ComponentLookup.Get(id));
		}
		
		/// <summary>
		/// Check if entity has a component by type
		/// </summary>
		/// <typeparam name="TComponent"></typeparam>
		/// <returns></returns>
		public bool Has<TComponent>() where TComponent : IComponent
		{
			return Components.ContainsKey(typeof(TComponent));
		}
		
		/// <summary>
		/// Remove component by id
		/// </summary>
		/// <param name="componentId"></param>
		public void Remove(int componentId)
		{
			Type component = ComponentLookup.Get(componentId);

			if (!Components.ContainsKey(component))
			{
				Debug.LogWarning($"Tried to remove component {componentId} but the entity didn't have it!");
				return;
			}

			IComponent cachedComponentBeforeRemoval = Components[component];
			Components.Remove(component);
			OnComponentRemoved?.Invoke(  this, cachedComponentBeforeRemoval);
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

		public void Set<TComponent>(object componentValue = null, bool setFromNetworkMessage = false) where TComponent : IComponent, new()
		{
			Type compType = typeof(TComponent);

			if (!Components.TryGetValue(compType, out IComponent component))
			{
				component = Add<TComponent>();
			}
			
			component.Set(componentValue);
			
			OnComponentSet?.Invoke(this, component, setFromNetworkMessage);
		}

		public void Set(int id, object componentValue = null, bool setFromNetworkMessage = false)
		{
			Type compType = ComponentLookup.Get(id);

			if (!Components.TryGetValue(compType, out IComponent component))
			{
				component = Add(id);
			}
			
			component.Set(componentValue);
			
			OnComponentSet?.Invoke(this, component, setFromNetworkMessage);
		}
	}
}