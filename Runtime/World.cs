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

			Debug.Log("Creating entity with id " + newEntity.id);
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
			Debug.Log("Destroy!");
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
		
		public virtual Group CreateGroup(GroupType groupType, HashSet<Type> filters)
		{
			Group newGroup = new Group(groupType, filters);
			Groups.Add(newGroup);
			return newGroup;
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
			OnEntityCreatedEvent?.Invoke(entity, entityCreatedFromNetworkMessage);
		}

		public virtual void OnEntityDestroyed(Entity.Entity entity, bool entityDestroyedFromNetworkMessage = false)
		{
			for (int i = 0; i < Groups.Count; i++)
			{
				Groups[i].EntityDestroyed(entity);
			}

			OnEntityDestroyedEvent?.Invoke(entity, entityDestroyedFromNetworkMessage);
		}

		public virtual void OnComponentAddedToEntity(Entity.Entity entity, IComponent component)
		{
			Debug.Log($"{component.GetType()} was added to {entity.id}");
			for (int i = 0; i < Groups.Count; i++)
			{
				Groups[i].ComponentAddedToEntity(entity, component.GetType());
			}
			
			OnComponentAddedToEntityEvent?.Invoke(entity, component);
		}

		public virtual void OnComponentRemovedFromEntity(Entity.Entity entity, IComponent component)
		{
			for (int i = 0; i < Groups.Count; i++)
			{
				Groups[i].ComponentRemovedFromEntity(entity, component.GetType());
			}
			
			OnComponentRemovedFromEntityEvent?.Invoke(entity, component);
		}

		public virtual void OnComponentSetOnEntity(Entity.Entity entity, IComponent component, bool setFromNetworkMessage = false)
		{
			for (int i = 0; i < Groups.Count; i++)
			{
				Groups[i].EntitySet(entity, component.GetType());
			}
			
			OnComponentSetOnEntityEvent?.Invoke(entity, component, setFromNetworkMessage);
		}
	}
}