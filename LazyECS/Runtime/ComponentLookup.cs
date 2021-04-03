using System;
using UnityEngine;

public static class ComponentLookup
{
	private static Type[] Components { get; set; }

	public static void Init(Type[] components)
	{
		Components = components;
	}
	
	public static Type Get(int id)
	{
		return Components[id];
	}

	public static int Get(Type type)
	{
		//TODO: Maybe if Components was a Dictionary this could be faster
		for (int i = 0; i < Components.Length; i++)
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