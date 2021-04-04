namespace LazyECS
{
	internal interface IFeature
	{
		void Initialize();
		void Update();
		void Teardown();
		void Cleanup();
		Systems Systems { get; }
	}
}