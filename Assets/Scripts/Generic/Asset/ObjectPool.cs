using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GDM
{ 
	namespace Asset
	{
		public abstract class GameObjectPool : IPrefabPool
		{
			private readonly List<GameObject> _availableInstances;
			private readonly string _prefabName;
			private readonly int _growth;
			private GameObject _parent;

			public int UnrecycledPrefabCount { get; private set; }
			
			public int AvailablePrefabCount
			{
				get { return _availableInstances.Count; }
			}
			
			public int AvailablePrefabCountMaximum { get; private set; }


			protected GameObjectPool(string prefabName, GameObject parent)
				: this(prefabName, parent, 0)
			{
			}
			
			protected GameObjectPool(string prefabName, GameObject parent, int initialSize)
				: this(prefabName, parent, initialSize, 1)
			{
			}
			
			protected GameObjectPool(string prefabName, GameObject parent, int initialSize, int growth)
				: this(prefabName, parent, initialSize, growth, int.MaxValue)
			{
			}
			
			protected GameObjectPool(string prefabName, GameObject parent, int initialSize, int growth, int availableItemsMaximum)
			{
				if (growth <= 0){
					throw new ArgumentOutOfRangeException("growth must be greater than 0!");
				}
				if (availableItemsMaximum < 0){
					throw new ArgumentOutOfRangeException("availableItemsMaximum must be at least 0!");
				}

				_prefabName = prefabName;
				_growth = growth;
				_parent = parent;
				AvailablePrefabCountMaximum = availableItemsMaximum;
				_availableInstances = new List<GameObject>(initialSize);
				
				if (initialSize > 0){
					BatchAllocatePoolItems(initialSize);
				}
			}

			public GameObject ObtainPrefabInstance()
			{
		        GameObject prefabInstance;

				if (_availableInstances.Count > 0)
		        {
		            prefabInstance = RetrieveLastItemAndRemoveIt();
		        }
		        else
		        {
		            if (_growth == 1 || AvailablePrefabCountMaximum == 0)
		            {
		                prefabInstance = AllocatePoolItem();
		            }
		            else
		            {
		                BatchAllocatePoolItems(_growth);
		                prefabInstance = RetrieveLastItemAndRemoveIt();
		            }

		            Debug.Log(GetType().FullName + "<" + prefabInstance.GetType().Name + " " + prefabInstance.name + "> was exhausted, with " + UnrecycledPrefabCount +
		            " items not yet recycled.  " +
		            "Allocated " + _growth + " more.");
		        }
		        OnHandleObtainPrefab(prefabInstance);
		        UnrecycledPrefabCount++;
		        return prefabInstance;
			}

			public void RecyclePrefabInstance(GameObject prefab){
			    if (prefab == null) { throw new ArgumentNullException("Cannot recycle null item!"); }
			    OnHandleRecyclePrefab(prefab);
			    if (_availableInstances.Count < AvailablePrefabCountMaximum) { _availableInstances.Add(prefab); }
			    UnrecycledPrefabCount--;	
			    if (UnrecycledPrefabCount < 0) { Debug.Log("More items recycled than obtained"); }
			}

		    private GameObject AllocatePoolItem(){
				GameObject instance = new GameObject("");
		        if (_parent != null)
		        {            
		            instance.transform.parent = _parent.transform;
				}
		        OnHandleAllocatePrefab(instance);
			    return instance;
			}

		    private void BatchAllocatePoolItems(int count){
	            List<GameObject> availableItems = _availableInstances;
		
		        int allocationCount = AvailablePrefabCountMaximum - availableItems.Count;
		        if (count < allocationCount){
		            allocationCount = count;
		        }
		
		        for (int i = allocationCount - 1; i >= 0; i--){
		            availableItems.Add(AllocatePoolItem());
		        }
		    }

	        private GameObject RetrieveLastItemAndRemoveIt(){
		        int lastElementIndex = _availableInstances.Count - 1;
		        var prefab = _availableInstances[lastElementIndex];
		        _availableInstances.RemoveAt(lastElementIndex);
		        return prefab;
	        }

			protected abstract void OnHandleAllocatePrefab(GameObject prefabInstance);
			protected abstract void OnHandleObtainPrefab(GameObject prefabInstance);
			protected abstract void OnHandleRecyclePrefab(GameObject prefabInstance);
		}
	}
}