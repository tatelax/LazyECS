using System;
using System.Collections.Generic;
using LazyECS;

public class SimulationController : MonoBehaviourSingleton<SimulationController>
{
	public Dictionary<Type, IWorld> Worlds { get; protected set; }
}
