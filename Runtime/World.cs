using System;
using System.Collections.Generic;
using System.Linq;
using LazyECS.Component;
using UnityEngine;

namespace LazyECS
{
	public abstract class World : IWorld
	{
		public Feature[] Features { get; protected set; }
		public Dictionary<int, Entity.Entity> Entities { get; } //entity id, actual entity
		public List<Group> Groups { get; }

		public delegate void EntityCreatedEvent(Entity.Entity entity, bool entityCreatedFromNetworkMessage);
		public delegate void EntityDestroyedEvent(Entity.Entity entity, bool entityDestroyedFromNetworkMessage);
		public delegate void ComponentAddedToEntity(Entity.Entity entity, IComponent component);
		public delegate void ComponentRemovedFromEntity(Entity.Entity entity, IComponent component);
		public delegate void ComponentSetOnEntity(Entity.Entity entity, IComponent component, bool setFromNetworkMessage);
		
		public event EntityCreatedEvent OnEntityCreatedEvent;
		public event EntityDestroyedEvent OnEntityDestroyedEvent;
		public event ComponentAddedToEntity OnComponentAddedToEntityEvent;
		public event ComponentRemovedFromEntity OnComponentRemovedFromEntityEvent;
		public event ComponentSetOnEntity OnComponentSetOnEntityEvent;

		protected World()
		{
			Groups = new List<Group>();
			Entities = new Dictionary<int, Entity.Entity>();
			Features = new Feature[]{};
		}

		public virtual void Initialize()
		{
			// Add features here in override
		}
		
		public virtual void Start()
		{
			for (int i = 0; i < Features.Length; i++)
			{
				Features[i].Initialize();
			}
		}

		public virtual void Update()
		{
			for (int i = 0; i < Features.Length; i++)
			{
				Features[i].Update();
			}
		}
		
		public virtual void Cleanup()
		{
			for (int i = 0; i < Features.Length; i++)
			{
				Features[i].Cleanup();
			}
		}

		public virtual void Teardown()
		{
			for (int i = 0; i < Features.Length; i++)
			{
				Features[i].Teardown();
			}
		}

		public virtual Entity.Entity CreateEntity(int id = -1, bool entityCreatedFromNetworkMessage = false)
		{
			Entity.Entity newEntity = id != -1 ? new Entity.Entity(id) : new Entity.Entity(Entities.Count);

			newEntity.OnComponentAdded += OnComponentAddedToEntity;
			newEntity.OnComponentRemoved += OnComponentRemovedFromEntity;
			newEntity.OnComponentSet += OnComponentSetOnEntity;
			Entities.Add(newEntity.id, newEntity);
			OnEntityCreated(newEntity, entityCreatedFromNetworkMessage);
			
			return newEntity;
		}

		public virtual bool DestroyEntity(int id, bool entityDestroyedFromNetworkMessage = false)
		{
			if (!Entities.ContainsKey(id))
			{
				Debug.LogWarning("Attempted to destroy entity but it doesn't exist!");
				return false;
			}

			return DestroyEntity(Entities[id], entityDestroyedFromNetworkMessage);
		}
		
		public virtual bool DestroyEntity(Entity.Entity entity, bool entityDestroyedFromNetworkMessage = false)
		{
			if (Entities.ContainsKey(entity.id))
			{
				Entities.Remove(entity.id);
				OnEntityDestroyed(entity, entityDestroyedFromNetworkMessage);
				return true;
			}

			Debug.LogWarning("Attempted to destroy entity but it doesn't exist!");
			return false;
		}

		public virtual void DestroyAllEntities()
		{
			foreach (KeyValuePair<int,Entity.Entity> entity in Entities.ToList())
			{
				DestroyEntity(entity.Value);
			}
		}
		
		public virtual Group CreateGroup(GroupType groupType, HashSet<Type> filters, EventType _eventType, Group.OnEntityUpdate OnEntityUpdate, bool checkExisting = true)
		{
			Group newGroup = new Group(groupType, filters, _eventType, OnEntityUpdate);
			Groups.Add(newGroup);

			if (checkExisting)
			{
				foreach (KeyValuePair<int,Entity.Entity> entity in Entities)
				{
					foreach (Type filter in filters)
					{
						if (entity.Value.Has(filter))
						{
							newGroup.ComponentAddedToEntity(entity.Value, filter);
						}
					}
				}
			}
			
			return newGroup;
		}

		/// <summary>
		/// Returns an entity with a specific ID
		/// </summary>
		/// <param name="id">The ID of the entity you want</param>
		/// <returns></returns>
		public Entity.Entity GetEntity(int id)
		{
			return Entities[id];
		}

		public List<Entity.Entity> GetEntities()
		{
			List<Entity.Entity> entities = new List<Entity.Entity>();
			
			foreach (KeyValuePair<int,Entity.Entity> entity in Entities)
			{
				entities.Add(entity.Value);	
			}
			
			return entities;
		}

		/// <summary>
		/// Get a list of entities that have a given component
		/// </summary>
		/// <typeparam name="TComponent"></typeparam>
		/// <returns></returns>
		public virtual List<Entity.Entity> GetEntities<TComponent>() where TComponent : IComponent
		{
			List<Entity.Entity> entitiesWithComponent = new List<Entity.Entity>();

			foreach (KeyValuePair<int,Entity.Entity> entity in Entities)
			{
				if(entity.Value.Has<TComponent>())
					entitiesWithComponent.Add(entity.Value);
			}
			
			return entitiesWithComponent;
		}

		/// <summary>
		/// Get a list of entities that have a given component with a specific value
		/// </summary>
		/// <param name="value"></param>
		/// <typeparam name="TComponent"></typeparam>
		/// <returns></returns>
		public virtual List<Entity.Entity> GetEntities<TComponent>(object value) where TComponent : IComponent
		{
			List<Entity.Entity> entitiesWithComponent = GetEntities<TComponent>();
			List<Entity.Entity> entitiesWithComponentValue = new List<Entity.Entity>();
			
			foreach (Entity.Entity entity in entitiesWithComponent)
			{
				if (entity.Get<TComponent>().Get().Equals(value))
					entitiesWithComponentValue.Add(entity);
			}
			
			return entitiesWithComponentValue;
		}

		public virtual void OnEntityCreated(Entity.Entity entity, bool entityCreatedFromNetworkMessage)
		{
			if (SimulationController.Instance.LogLevel == LogLevel.Verbose)
				Debug.Log("<color=#00ff00>[LazyECS] Creating entity with id " + entity.id + " in world " + GetType().Name + "</color>");
			
			OnEntityCreatedEvent?.Invoke(entity, entityCreatedFromNetworkMessage);
		}

		public virtual void OnEntityDestroyed(Entity.Entity entity, bool entityDestroyedFromNetworkMessage = false)
		{
			if (SimulationController.Instance.LogLevel == LogLevel.Verbose)
			{
				Debug.Log("<color=#00ff00>[LazyECS] " + $"{entity.id} was <b>DESTROYED</b> in world {GetType().Name} </color>");
			}
			
			for (int i = 0; i < Groups.Count; i++)
			{
				Groups[i].EntityDestroyed(entity);
			}

			OnEntityDestroyedEvent?.Invoke(entity, entityDestroyedFromNetworkMessage);
		}

		public virtual void OnComponentAddedToEntity(Entity.Entity entity, IComponent component)
		{
			if (SimulationController.Instance.LogLevel == LogLevel.Verbose)
			{
				Debug.Log("<color=#00ff00>[LazyECS] " + $"{component.GetType()} was <b>ADDED</b> to entity {entity.id} in world {GetType().Name} </color>");
			}
			
			for (int i = 0; i < Groups.Count; i++)
			{
				Groups[i].ComponentAddedToEntity(entity, component.GetType());
			}
			
			OnComponentAddedToEntityEvent?.Invoke(entity, component);
		}

		public virtual void OnComponentRemovedFromEntity(Entity.Entity entity, IComponent component)
		{
			if (SimulationController.Instance.LogLevel == LogLevel.Verbose)
				Debug.Log("<color=#00ff00>[LazyECS] " + $"{component.GetType()} was <b>REMOVED</b> from entity {entity.id} in world {GetType().Name} </color>");
			
			for (int i = 0; i < Groups.Count; i++)
			{
				Groups[i].ComponentRemovedFromEntity(entity, component.GetType());
			}
			
			OnComponentRemovedFromEntityEvent?.Invoke(entity, component);
		}

		public virtual void OnComponentSetOnEntity(Entity.Entity entity, IComponent component, bool setFromNetworkMessage = false)
		{
			if (SimulationController.Instance.LogLevel == LogLevel.Verbose)
				Debug.Log("<color=#00ff00>[LazyECS] " + $"{component.GetType()} was <b>SET</b> on entity {entity.id} in world {GetType().Name} </color>");
			
			for (int i = 0; i < Groups.Count; i++)
			{
				Groups[i].EntitySet(entity, component.GetType());
			}
			
			OnComponentSetOnEntityEvent?.Invoke(entity, component, setFromNetworkMessage);
		}
	}
}