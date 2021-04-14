﻿namespace LazyECS.Component
{
	public delegate void ComponentChanged();
	
	public interface IComponent
	{
		bool Set(object value = null);
		object Get();
	}
}