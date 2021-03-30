using System;
using System.Collections.Generic;
using MirrorECS.Entity;

namespace MirrorECS
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
			newEntity.OnComponentAdded += GroupUpdate;
			return newEntity;
		}

		// Used to notify groups that a component changed
		private void GroupUpdate(IEntity entity)
		{
			for (var i = 0; i < Groups.Count; i++)
			{
				Groups[i].Update(entity);
			}
		}

		public Group CreateGroup(Type[] filters)
		{
			Group newGroup = new Group(filters);
			Groups.Add(newGroup);
			return newGroup;
		}
	}
}