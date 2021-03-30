namespace LazyECS
{
	public interface ITeardownSystem : ISystem
	{
		void Teardown();
	}
}