using System.Collections.Generic;
using LazyECS;

public class SimulationController : MonoBehaviourSingleton<SimulationController>
{
	public Dictionary<int, IWorld> Worlds { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		
		ComponentLookup.Init();
	}
	
	public void InitializeWorlds(IWorld[] worlds)
	{
		Worlds = new Dictionary<int, IWorld>();
		
		for (int i = 0; i < worlds.Length; i++)
		{
			Worlds.Add(i, worlds[i]);
		}
	}

	public IWorld GetWorld(int id)
	{
		return Worlds[id];
	}
}
