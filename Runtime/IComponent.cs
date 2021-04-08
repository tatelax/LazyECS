namespace LazyECS.Component
{
	public delegate void ComponentChanged();
	
	public interface IComponent
	{
		void Set(object value);
		object Get();
	}
}