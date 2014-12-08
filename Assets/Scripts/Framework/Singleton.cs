using UnityEngine;

namespace GDM
{
	namespace Global
	{
		public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour 
		{
			protected static T instance;

			/// <summary>
			/// Return the instance of this singleton.
			/// </summary>
			public static T Instance
			{
				get
				{
					if(instance == null)
					{
						instance  = (T)Object.FindObjectOfType(typeof(T));
						if(instance == null)
						{
							Debug.LogError("An instance of " + typeof(T) + 
							               " is needed in the scene, but there is none.");
						}
					}
					return instance;
				}
			}
		}
	}
}