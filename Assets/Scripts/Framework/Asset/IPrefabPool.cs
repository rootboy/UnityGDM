using UnityEngine;
using System.Collections;

namespace GDM
{
	namespace Asset
	{
		public interface IPrefabPool
		{	
			int UnrecycledPrefabCount { get; }
			
			int AvailablePrefabCount { get; }
			
			int AvailablePrefabCountMaximum { get; }
			
			GameObject ObtainPrefabInstance();
			
			void RecyclePrefabInstance(GameObject prefab);
		}

	}
}