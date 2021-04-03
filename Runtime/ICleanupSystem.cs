namespace LazyECS
{
	public interface ICleanupSystem : ISystem
	{
		void Cleanup();
	}
}