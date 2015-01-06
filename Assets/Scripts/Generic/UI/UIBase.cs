using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace GDM
{
	namespace UI
	{
        public abstract class UIBase : MonoBehaviour
        {
            protected Dictionary<string, BoxCollider> dicBtns = new Dictionary<string, BoxCollider>();
            protected Dictionary<string, UILabel> dicLabels = new Dictionary<string, UILabel>();
            protected Dictionary<string, UISprite> dicSprite = new Dictionary<string, UISprite>();

            protected string buttonFilter = "Button_";
            protected string labelFilter = "Label_";
            protected string spriteFilter = "Sprite_";

            protected virtual void Awake()
            {
                GetAllReference();
                OnAwake();
            }

            protected virtual void Start()
            {
                RegisterEvent();
                OnStart();
            }

            protected virtual void OnAwake()
            {

            }

            protected virtual void OnStart()
            {

            }

            public virtual void OnCreate(object param)
            {

            }

            public virtual void OnDestroy()
            {

            }

            public virtual void OnVisible(object param)
            {

            }

            public virtual void OnInvisible(object param)
            {

            }

            protected virtual void OnClick(GameObject param)
            {

            }

            protected virtual void RegisterEvent()
            {
                foreach (KeyValuePair<string, BoxCollider> item in dicBtns)
                {
                    UIEventListener.Get(item.Value.gameObject).onClick += this.OnClick;
                }
            }

            /// <summary>
            /// Get references of ui controls.
            /// </summary>
            protected void GetAllReference()
            {
                GetReference<BoxCollider>(buttonFilter, dicBtns);
                GetReference<UILabel>(labelFilter, dicLabels);
                GetReference<UISprite>(spriteFilter, dicSprite);
            }


            /// <summary>
            /// Generic functions for get references of all ui components.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="filter"></param>
            /// <param name="dic"></param>
            protected void GetReference<T>(string filter, IDictionary dic) where T : Component
            {
                T[] array = GetComponentsInChildren<T>(true);
                foreach (T item in array)
                {
                    if (item.name.Contains(filter))
                    {
                        if (dic.Contains(item.name))
                        {
                            Debug.LogError("");
                            continue;
                        }
                        dic.Add(item.name, item);
                    }
                }
            }
        }
	}
}