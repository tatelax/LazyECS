using System;
using System.Collections.Generic;
using System.Linq;
using LazyECS;

public class SimulationController : MonoBehaviourSingleton<SimulationController>
{
	public Dictionary<int, IWorld> Worlds { get; private set; }
	public event EventHandler OnWorldsInitialized;

	protected override void Awake()
	{
		base.Awake();
		
		Worlds = new Dictionary<int, IWorld>();
		ComponentLookup.Init();
	}

	private void StartWorlds()
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
		
		foreach (KeyValuePair<int,IWorld> world in Worlds)
		{
			world.Value.Initialize();
		}
		
		OnWorldsInitialized?.Invoke(this, EventArgs.Empty);
		
		StartWorlds();
	}

	public IWorld GetWorld(int id)
	{
		return Worlds[id];
	}

	public IWorld GetWorld<T>() where T : IWorld
	{
		return Worlds.First(x => x.Value.GetType() == typeof(T)).Value;
	}

	public int GetWorldId(IWorld world)
	{
		return Worlds.First(x => x.Value == world).Key;
	}
}
