using System;
using System.Collections.Generic;
using LazyECS;

public class SimulationController : MonoBehaviourSingleton<SimulationController>
{
	public Dictionary<Type, IWorld> Worlds { get; private set; }

	public void InitializeWorlds(IWorld[] worlds)
	{
		Worlds = new Dictionary<Type, IWorld>();
		
		for (int i = 0; i < worlds.Length; i++)
		{
			Worlds.Add(worlds[i].GetType(), worlds[i]);
		}
	}
}
