using System;
using System.Collections.Generic;
using LazyECS.Entity;

namespace LazyECS
{
	public abstract class World : IWorld
	{
		protected Feature[] features;
		private List<Entity.Entity> entities;
		public List<Group> Groups { get; }

		protected World()
		{
			Groups = new List<Group>();
			entities = new List<Entity.Entity>();
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

		public TEntity CreateEntity<TEntity>() where TEntity : IEntity, new()
		{
			TEntity newEntity = new TEntity();
			newEntity.OnComponentAdded += ComponentChangedOnAnEntity;
			newEntity.OnComponentRemoved += ComponentChangedOnAnEntity;
			newEntity.OnComponentSet += ComponentChangedOnAnEntity;
			return newEntity;
		}

		// Used to notify groups that a component was added/removed/changed
		public void ComponentChangedOnAnEntity(IEntity entity)
		{
			for (int i = 0; i < Groups.Count; i++)
			{
				Groups[i].ComponentAddedOrRemovedFromSomeEntity(entity);
			}
		}

		public Group CreateGroup(GroupType groupType, Type[] filters)
		{
			Group newGroup = new Group(groupType, filters);
			Groups.Add(newGroup);
			return newGroup;
		}
	}
}