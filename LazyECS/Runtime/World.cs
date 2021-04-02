using System.Collections.Generic;
using LazyECS.Component;
using LazyECS.Entity;

namespace LazyECS
{
	public abstract class World : IWorld
	{
		protected Feature[] features;
		public Dictionary<int, Entity.Entity> Entities { get; } //entity id, actual entity
		public List<Group> Groups { get; }

		protected World()
		{
			Groups = new List<Group>();
			Entities = new Dictionary<int, Entity.Entity>();
		}

		public virtual void Init()
		{
			throw new System.NotImplementedException();
		}

		public void Start()
		{
			foreach (IFeature feature in features)
			{
				feature.Initialize();
			}
		}

		public void Update()
		{
			foreach (IFeature feature in features)
			{
				feature.Update();
			}
		}

		public void Teardown()
		{
			foreach (Feature feature in features)
			{
				feature.Teardown();
			}
		}

		public void Cleanup()
		{
			foreach (Feature feature in features)
			{
				feature.Cleanup();
			}
		}

		public Entity.Entity CreateEntity(int id = default)
		{
			Entity.Entity newEntity = new Entity.Entity(id);
			
			newEntity.OnComponentAdded += OnComponentAddedToEntity;
			newEntity.OnComponentRemoved += OnComponentRemovedFromEntity;
			newEntity.OnComponentSet += OnComponentSetOnEntity;
			Entities.Add(newEntity.id, newEntity);
			OnEntityCreated(newEntity);
			
			return newEntity;
		}

		public bool DestroyEntity(Entity.Entity entity)
		{
			if (Entities.ContainsKey(entity.id))
			{
				Entities.Remove(entity.id);
				return true;
			}

			UnityEngine.Debug.LogWarning("Attempted to destroy entity but it doesn't exist!");
			return false;
		}

		public virtual void OnEntityCreated(Entity.Entity entity) { }

		public virtual void OnComponentAddedToEntity(Entity.Entity entity, IComponent component)
		{
			for (int i = 0; i < Groups.Count; i++)
			{
				Groups[i].ComponentAddedToEntity(entity, component);
			}
		}

		public virtual void OnComponentRemovedFromEntity(Entity.Entity entity, IComponent component)
		{
			for (int i = 0; i < Groups.Count; i++)
			{
				Groups[i].ComponentRemovedFromEntity(entity, component);
			}
		}

		public virtual void OnComponentSetOnEntity(Entity.Entity entity, IComponent component) {}

		public Group CreateGroup(GroupType groupType, HashSet<IComponent> filters)
		{
			Group newGroup = new Group(groupType, filters);
			Groups.Add(newGroup);
			return newGroup;
		}
	}
}