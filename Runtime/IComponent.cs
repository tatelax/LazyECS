namespace LazyECS.Component
{
	public interface IComponent
	{
		bool Set(object value = null);
		object Get();
	}
}