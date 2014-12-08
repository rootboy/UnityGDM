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
							GameObject go = new GameObject();
							instance = go.AddComponent<T>();
						}
					}
					return instance;
				}
			}
		}
	}
}