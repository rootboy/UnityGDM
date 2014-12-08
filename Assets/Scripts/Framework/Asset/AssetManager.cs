using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GDM.Global;

namespace GDM
{
	namespace Asset
	{
		public class AssetManager :  Singleton<AssetManager>
		{
			protected Dictionary<int, Object> dicAssets = new Dictionary<int, Object>();
	


			public Object Load(string path)
			{
#if UNITY_EDITOR



#else



#endif
				return null;
			}


			public T Load<T>(string path)
			{
				return default(T);
			}






			private Object LoadAssetAtPath(string path, System.Type type)
			{
				return Resources.LoadAssetAtPath(path, type);
			}


			private T LoadAssetAtPath<T>(string path) where T : Object
			{
				return Resources.LoadAssetAtPath<T>(path);
			}
		}
	}
}