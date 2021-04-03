// MonoBehaviourSingleton: this is a base mechanism for singleton-style MonoBehaviour classes, used 
//   when you want only one instance of an object.
//
// The singleton instance can come in as part of a loaded scene or instantiated 
//   prefab, or can be created at runtime using GameObject's component-creation 
//   functions (such as AddComponent()).  
//
// Instance setup comes through the Awake() function, which we've made virtual. This was
//   necessary to support singletons that come in as part of a prefab, due to the fact that
//   the class will actually have TWO instances created (one inside the prefab resource and
//   one when the prefab is instantiated). Only the instantiated copy will be used in the 
//   running game and receive Awake().
//
// Also note that cleanup happens automatically when the singleton object is slated for
//   destruction, such as when it is removed from its owner GameObject or if that GameObject
//   is marked to be destroyed. This happens at the end of the Unity's Update loop for that
//   frame, when OnDestroy is called, without waiting for the the garbage collector to run.
//
// How to derive your own:
//
//      public class MySingletonManager : MonoBehaviourSingleton<MySingletonManager>
//      { ... }
//
//      // If you override Awake() in your class, you must call base.Awake() inside it.
//      protected override void Awake()
//      {
//          base.Awake();
//          ...
//      }
//
// How to check for validity:  if (MySingletonManager.IsInstanceValid()) { ... do something ... }
// How to access:               MySingletonManager.Instance
//
// Originally based on http://wiki.unity3d.com/index.php?title=Singleton

using UnityEngine;


public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	/// <summary>
	/// Safely checks whether the singleton instance is set.
	/// </summary>
	public static bool IsInstanceValid()
	{
		return (_instance != null);
	}

	/// <summary>
	/// Get access to the singleton instance of the class. Note that this does not currently perform lazy instantiation if there is no instance set.
	/// </summary>
	public static T Instance
	{
		get
		{
			// NOTE: this null check exists so we can catch the case when the instance is tagged for destruction under-the-hood. 
			//       Unity system objects (including MonoBehaviours) have their == and != operators overloaded so that when they
			//       are tagged for destruction, these operators return true when the reference is compared with null. So if
			//       _instance is actually null OR tagged for destruction, we don't want anyone accessing it or calling functions
			//       on it.
			if(_instance == null)
				return null;

			return _instance;
		}

		// NOTE: the 'set' is private because setting a new instance happens internally in Awake(), and nulling 
		//       happens automatically when the object is slated for destruction, by Unity. Think very carefully 
		//       whether you really need to change it or not.
		private set
		{
			Debug.Assert((value == null || _instance == null || ReferenceEquals(_instance, value)), "Multiple singleton-style Components of type " + typeof(T).Name + " detected. There should only be one.");
			_instance = value;
		}
	}

	private static T _instance = null;

	// NOTE: If you override Awake() in your class, you must call base.Awake() inside it.
	protected virtual void Awake()
	{
		Instance = this as T;
	}

	protected virtual void OnDestroy()
	{

	}
}
