using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LazyECS.Component;
using UnityEngine;

public static class ComponentLookup
{
	private static List<Type> Components { get; set; }

	public static void Init()
	{
		Components = new List<Type>();
		
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (type.IsInterface || !type.GetInterfaces().Contains(typeof(IComponent))) continue;
				
				Components.Add(type);
			}
		}
	}
	
	public static Type Get(int id)
	{
		return Components[id];
	}

	public static int Get(Type type)
	{
		//TODO: Maybe if Components was a Dictionary this could be faster
		for (int i = 0; i < Components.Count; i++)
		{
			if (Components[i] == type)
			{
				return i;
			}
		}
		
		Debug.LogError($"Unable to find component {type.Name}");
		return -1;
	}
}