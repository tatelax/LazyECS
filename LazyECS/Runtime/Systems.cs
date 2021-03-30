using System.Collections.Generic;

namespace MirrorECS
{
	public class Systems
	{
		protected readonly List<IInitializeSystem> initializeSystems;
		protected readonly List<IUpdateSystem> updateSystems;

		public Systems()
		{
			initializeSystems = new List<IInitializeSystem>();
			updateSystems = new List<IUpdateSystem>();
		}

		public virtual Systems Add(ISystem system)
		{
			if(system is IInitializeSystem initializeSystem)
				initializeSystems.Add(initializeSystem);
			
			if(system is IUpdateSystem updateSystem)
				updateSystems.Add(updateSystem);

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
	}
}