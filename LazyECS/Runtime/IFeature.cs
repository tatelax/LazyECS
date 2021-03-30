namespace LazyECS
{
	internal interface IFeature
	{
		void Setup();
		void Initialize();
		void Update();
		Systems Systems { get; }
	}
}