namespace LazyECS
{
	public abstract class Feature : IFeature
	{
		public Systems Systems { get; protected set; }

		public void Initialize()
		{
			Systems.Initialize();
		}

		public void Update()
		{
			Systems.Update();
		}

		public void Teardown()
		{
			Systems.Teardown();
		}
		
		public void Cleanup()
		{
			Systems.Cleanup();
		}
	}
}