using System.Collections.Generic;
using MirrorECS;

internal interface IFeature
{
	void Setup();
	void Initialize();
	void Update();
	Systems Systems { get; }
}