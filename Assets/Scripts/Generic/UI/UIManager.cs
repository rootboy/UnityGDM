using UnityEngine;
using System.Collections.Generic;
using GDM.Global;

namespace GDM
{ 
    namespace UI
    {
        public class UIManager : Singleton<UIManager>
        {
            protected Dictionary<string, GameObject> mDicUis = new Dictionary<string, GameObject>();
            protected Stack<string> stackUis = new Stack<string>();
            public GameObject root;


            /// <summary>
            /// Create a new ui, convenience function.
            /// </summary>
            /// <targetParam name="name"></targetParam>
            /// <returns></returns>
            public bool CreateUI(string name)
            {
                return CreateUI(name, null, true);
            }


            /// <summary>
            /// Create a new ui, convenience function.
            /// </summary>
            /// <targetParam name="name"></targetParam>
            /// <targetParam name="targetParam"></targetParam>
            /// <returns></returns>
            public bool CreateUI(string name, object param)
            {
                return CreateUI(name, param, true);
            }


            /// <summary>
            /// Create a new ui.
            /// </summary>
            /// <targetParam name="name"></targetParam>
            /// <targetParam name="targetParam"></targetParam>
            /// <returns></returns>
            public GameObject CreateUI(string name, object param, bool isVisible)
            {
                if (mDicUis.ContainsKey(name)) return mDicUis[name];

                //TODO: asset loading may be replaced future.
                Object asset = Resources.Load(name);
                if (asset == null)
                {
                    Debug.LogError(string.Format("ui creating failed, file not exits! {0}", name));
                    return null;
                }

                GameObject ui = UnityEngine.Object.Instantiate(asset) as GameObject;
                UIBase uiBase = ui.GetComponent<UIBase>();
                if (uiBase != null) uiBase.OnCreate(param);
                ui.SetActive(isVisible);
                AddToUIRoot(ui);

                mDicUis.Add(name, ui);
                return ui;
            }


            /// <summary>
            /// Destroy a ui.
            /// </summary>
            /// <targetParam name="name"></targetParam>
            public void DestroyUI(string name)
            {
                if (mDicUis.ContainsKey(name))
                {
                    GameObject ui = mDicUis[name];
                    if (ui != null) UnityEngine.Object.Destroy(ui);
                    mDicUis.Remove(name);
                }
            }


            /// <summary>
            /// Destroy all ui.
            /// </summary>
            public void DestroyAllUI()
            {
                foreach (KeyValuePair<string, GameObject> item in mDicUis)
                {
                    GameObject ui = item.Value;
                    if (ui != null) UnityEngine.Object.Destroy(ui);
                }
                mDicUis.Clear();
            }


            /// <summary>
            /// Display a ui and pass a parameter.
            /// </summary>
            /// <targetParam name="name"></targetParam>
            /// <targetParam name="targetParam"></targetParam>
            /// <returns></returns>
            public void Show(string name, object param)
            {
                if (!mDicUis.ContainsKey(name))
                    CreateUI(name, param);

                if (mDicUis.ContainsKey(name))
                {
                    GameObject go = mDicUis[name];
                    if (go != null)
                    {
                        go.SetActive(true);
                        UIBase ui = go.GetComponent<UIBase>();
                        if (ui != null) ui.OnVisible(param);
                    }
                }
            }


            /// <summary>
            /// Hide a ui and pass a parameter.
            /// </summary>
            /// <targetParam name="name"></targetParam>
            /// <targetParam name="targetParam"></targetParam>
            /// <returns></returns>
            public bool Hide(string name, object param)
            {
                if (mDicUis.ContainsKey(name))
                {
                    GameObject go = mDicUis[name];
                    if (go != null)
                    {
                        go.SetActive(false);
                        UIBase ui = go.GetComponent<UIBase>();
                        if (ui != null) ui.OnInvisible(param);
                    }
                }
                return false;
            }


            /// <summary>
            /// Add a new ui to the child of UIRoot2D/UIRoot3D.
            /// </summary>
            public GameObject AddToUIRoot(GameObject go)
            {
                if (root == null)
                {
                    Debug.LogError("UIRoot is null, please have a check!");
                    return go;
                }

                Vector3 pos = go.transform.localPosition;
                Vector3 rot = go.transform.localEulerAngles;
                Vector3 sca = go.transform.localScale;
                go.transform.parent = root.transform;
                go.transform.localPosition = pos;
                go.transform.localEulerAngles = rot;
                go.transform.localScale = sca;
                return go;
            }


            /// <summary>
            /// Is the ui where on the relativePath visible?
            /// </summary>
            /// <targetParam name="name"></targetParam>
            /// <returns></returns>
            public bool IsVisible(string relativePath)
            {
                //TODO: consider the alpha value
                Transform child = root.transform.Find(name);
                if (child == null || !child.gameObject.activeInHierarchy)
                    return false;
                return true;
            }

            /// <summary>
            /// Is the ui where on the relative static(No tween component running)?
            /// </summary>
            /// <targetParam name="relativePath"></targetParam>
            /// <returns></returns>
            public bool IsStatic(string relativePath)
            {
                return true;
            }


            /// <summary>
            /// Retrieve all visible ui currently.
            /// </summary>
            /// <returns></returns>
            public List<GameObject> GetAllVisibleUI()
            {
                List<GameObject> list = new List<GameObject>();
                foreach (KeyValuePair<string, GameObject> item in mDicUis)
                {
                    if (item.Value.activeSelf) list.Add(item.Value);
                }
                return list;
            }
        }
    } 
} 
