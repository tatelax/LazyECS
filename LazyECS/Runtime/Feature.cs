namespace LazyECS
{
	public abstract class Feature : IFeature
	{
		public Systems Systems { get; protected set; }

		protected Feature()
		{
			Setup();
		}
	
		public virtual void Setup()
		{
			throw new System.NotImplementedException();
		}

		public void Initialize()
		{
			Systems.Initialize();
		}

		public void Update()
		{
			Systems.Update();
		}
	}
}