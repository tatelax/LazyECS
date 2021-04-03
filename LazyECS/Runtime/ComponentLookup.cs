using System;

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
}