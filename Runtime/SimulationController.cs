using System;
using System.Collections.Generic;
using System.Linq;
using LazyECS;
using UnityEngine;

public enum LogLevel
{
	None,
	Verbose
}

public class SimulationController : MonoBehaviourSingleton<SimulationController>
{
	[SerializeField] private LogLevel logLevel;
	public LogLevel LogLevel => logLevel;

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
		if (worlds.Length > 0)
			Reset();
		
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

	protected void Reset()
	{
		foreach (KeyValuePair<int,IWorld> world in Worlds)
		{
			world.Value.Teardown();
			IDisposable disposableWorld = (IDisposable)world.Value;
			disposableWorld.Dispose();
		}

		Worlds = new Dictionary<int, IWorld>();
	}

	protected void DestroyAllEntitiesInAllWorlds()
	{
		foreach (KeyValuePair<int,IWorld> world in Worlds)
		{
			world.Value.DestroyAllEntities();
		}
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
