using System;
using System.Collections.Generic;
using LazyECS.Entity;

namespace LazyECS
{
	public abstract class World : IWorld
	{
		protected Feature[] features;
		private List<Entity.Entity> entities;
		public List<Group> Groups { get; private set; }

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

		public TEntity CreateEntity<TEntity>() where TEntity : IEntity, new()
		{
			TEntity newEntity = new TEntity();
			newEntity.OnComponentAdded += ComponentAddedOrRemovedFromSomeEntity;
			newEntity.OnComponentRemoved += ComponentAddedOrRemovedFromSomeEntity;
			return newEntity;
		}

		// Used to notify groups that a component was added or removed
		private void ComponentAddedOrRemovedFromSomeEntity(IEntity entity)
		{
			for (var i = 0; i < Groups.Count; i++)
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