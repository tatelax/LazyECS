using System.Collections.Generic;

namespace LazyECS
{
	public class Systems
	{
		protected readonly List<IInitializeSystem> initializeSystems;
		protected readonly List<IUpdateSystem> updateSystems;
		protected readonly List<IEventSystem> eventSystems;
		protected readonly List<ITeardownSystem> teardownSystems;
		protected readonly List<ICleanupSystem> cleanupSystems;

		public Systems()
		{
			initializeSystems = new List<IInitializeSystem>();
			updateSystems = new List<IUpdateSystem>();
			eventSystems = new List<IEventSystem>();
			teardownSystems = new List<ITeardownSystem>();
			cleanupSystems = new List<ICleanupSystem>();
		}

		public virtual Systems Add(ISystem system)
		{
			if (system is IInitializeSystem initializeSystem)
				initializeSystems.Add(initializeSystem);

			if (system is IUpdateSystem updateSystem)
				updateSystems.Add(updateSystem);

			if (system is IEventSystem eventSystem)
				eventSystems.Add(eventSystem);

			if (system is ITeardownSystem teardownSystem)
				teardownSystems.Add(teardownSystem);

			if (system is ICleanupSystem cleanupSystem)
				cleanupSystems.Add(cleanupSystem);

			return this;
		}

		public virtual void Initialize()
		{
			for (int i = 0; i < initializeSystems.Count; i++)
			{
				initializeSystems[i].Initialize();
			}
		}

		public virtual void Update()
		{
			for (int i = 0; i < updateSystems.Count; i++)
			{
				updateSystems[i].Update();
			}
		}

		public virtual void Teardown()
		{
			for (int i = 0; i < teardownSystems.Count; i++)
			{
				teardownSystems[i].Teardown();
			}
		}

		public virtual void Cleanup()
		{
			for (var i = 0; i < cleanupSystems.Count; i++)
			{
				cleanupSystems[i].Cleanup();
			}
		}
	}
}