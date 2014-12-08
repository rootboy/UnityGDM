using UnityEngine;
using System.Collections;
using GDM.Global;

namespace GDM
{
	namespace Asset
	{
		public class AssetManager :  Singleton<AssetManager>
		{

			public Object Load(string path)
			{
				return null;
			}

			public T Load<T>(string path)
			{
				return default(T);
			}
		}
	}
}