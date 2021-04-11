using System.Collections.Generic;
using System.Linq;
using LazyECS;

public class SimulationController : MonoBehaviourSingleton<SimulationController>
{
	public Dictionary<int, IWorld> Worlds { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		
		ComponentLookup.Init();
	}

	protected void Start()
	{
		foreach (KeyValuePair<int,IWorld> world in Worlds)
		{
			world.Value.Start();
		}
	}

	protected virtual void Update()
	{
		foreach (KeyValuePair<int,IWorld> world in Worlds)
		{
			world.Value.Update();
			world.Value.Cleanup();
		}
	}

	protected void OnDisable()
	{
		foreach (KeyValuePair<int,IWorld> world in Worlds)
		{
			world.Value.Teardown();
		}
	}

	protected void InitializeWorlds(IWorld[] worlds)
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

	public int GetWorldId(IWorld world)
	{
		return Worlds.First(x => x.Value == world).Key;
	}
}
