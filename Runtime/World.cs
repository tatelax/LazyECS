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

		protected World()
		{
			Groups = new List<Group>();
			Entities = new Dictionary<int, Entity.Entity>();
		}

		public void Start()
		{
			for (int i = 0; i < Features.Length; i++)
			{
				Features[i].Initialize();
			}
		}

		public void Update()
		{
			for (int i = 0; i < Features.Length; i++)
			{
				Features[i].Update();
			}
		}
		
		public void Cleanup()
		{
			for (int i = 0; i < Features.Length; i++)
			{
				Features[i].Cleanup();
			}
		}

		public void Teardown()
		{
			for (int i = 0; i < Features.Length; i++)
			{
				Features[i].Teardown();
			}
		}

		public Entity.Entity CreateEntity(int id = default, bool entityCreatedFromNetworkMessage = false)
		{
			Entity.Entity newEntity = id != default ? new Entity.Entity(id) : new Entity.Entity();
			
			Debug.Log("Creating entity with id " + newEntity.id);
			newEntity.OnComponentAdded += OnComponentAddedToEntity;
			newEntity.OnComponentRemoved += OnComponentRemovedFromEntity;
			newEntity.OnComponentSet += OnComponentSetOnEntity;
			Entities.Add(newEntity.id, newEntity);
			OnEntityCreated(newEntity, entityCreatedFromNetworkMessage);
			
			return newEntity;
		}

		public bool DestroyEntity(int id, bool entityDestroyedFromNetworkMessage = false)
		{
			if (!Entities.ContainsKey(id))
			{
				Debug.LogWarning("Attempted to destroy entity but it doesn't exist!");
				return false;
			}

			return DestroyEntity(Entities[id], entityDestroyedFromNetworkMessage);
		}
		
		public bool DestroyEntity(Entity.Entity entity, bool entityDestroyedFromNetworkMessage = false)
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

		public void DestroyAllEntities()
		{
			foreach (KeyValuePair<int,Entity.Entity> entity in Entities.ToList())
			{
				DestroyEntity(entity.Value);
			}
		}

		public virtual void OnEntityCreated(Entity.Entity entity, bool entityCreatedFromNetworkMessage) { }

		public virtual void OnEntityDestroyed(Entity.Entity entity, bool entityDestroyedFromNetworkMessage = false)
		{
			for (int i = 0; i < Groups.Count; i++)
			{
				Groups[i].EntityDestroyed(entity);
			}
		}

		public virtual void OnComponentAddedToEntity(Entity.Entity entity, IComponent component)
		{
			Debug.Log($"{component.GetType()} was added to {entity.id}");
			for (int i = 0; i < Groups.Count; i++)
			{
				Groups[i].ComponentAddedToEntity(entity, component.GetType());
			}
		}

		public virtual void OnComponentRemovedFromEntity(Entity.Entity entity, IComponent component)
		{
			for (int i = 0; i < Groups.Count; i++)
			{
				Groups[i].ComponentRemovedFromEntity(entity, component.GetType());
			}
		}

		public virtual void OnComponentSetOnEntity(Entity.Entity entity, IComponent component, bool setFromNetworkMessage = false) {}

		public Group CreateGroup(GroupType groupType, HashSet<Type> filters)
		{
			Group newGroup = new Group(groupType, filters);
			Groups.Add(newGroup);
			return newGroup;
		}
	}
}