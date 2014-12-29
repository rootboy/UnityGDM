using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using GDM.Util;


namespace GDM
{
    namespace UI
    {
        /// <summary>
        /// UILayout used to adjust ui for different aspect radio.
        /// </summary>
        public sealed class UILayout : MonoBehaviour
        {
            class UIParts
            {
                public string xpath;
                public XmlAttributeCollection attrColl;

                public UIParts(string _xpath, XmlAttributeCollection _attrColl)
                {
                    xpath = _xpath;
                    attrColl = _attrColl;
                }
            }

            class UIPartsParam
            {
                public object target;
                public string key;
                public object param;
                public object start;
                public object end;


                public UIPartsParam(object _target, string _key, object _param)
                {
                    target = _target;
                    key = _key;
                    param = _param;
                }
            }

            public bool isTween;
            public float tweenTime = 1.0f;
            
            XmlDocument xmlDoc = new XmlDocument();
            List<UIParts> partsList = new List<UIParts>();
            List<UIPartsParam> partsParamList = new List<UIPartsParam>();
            bool layout = false;
            float runningTime, percentage, time = 1.0f;
            TweenUtil.EasingFunction ease;
            TweenUtil.EaseType easeType = TweenUtil.EaseType.linear;

            void Awake()
            {
                xmlDoc.Load(Application.streamingAssetsPath + "/" + "UIConfig.xml");
                ease = TweenUtil.GetEasingFunction(easeType);
            }

            void Update()
            {
                //TODO: Need test
                if (layout){
                    runningTime += Time.deltaTime;
                    percentage = runningTime / time;
                    Apply();

                    if (!isTween || percentage > 1){
                        percentage = 1.0f;
                        Apply();
                        layout = false;
                    }
                }
            }

            private void Apply()
            {
                foreach (UIPartsParam item in partsParamList)
                {
                    if (item.key == "pos")
                    {
                        ((Transform)item.target).localPosition = new Vector3(
                            ease(((Vector3)item.start).x, ((Vector3)item.end).x, percentage),
                            ease(((Vector3)item.start).y, ((Vector3)item.end).y, percentage),
                            ease(((Vector3)item.start).z, ((Vector3)item.end).z, percentage));
                    }
                    if (item.key == "rot")
                    {
                        ((Transform)item.target).localEulerAngles = new Vector3(
                            ease(((Vector3)item.start).x, ((Vector3)item.end).x, percentage),
                            ease(((Vector3)item.start).y, ((Vector3)item.end).y, percentage),
                            ease(((Vector3)item.start).z, ((Vector3)item.end).z, percentage));
                    }
                    if (item.key == "sca")
                    {
                        ((Transform)item.target).localScale = new Vector3(
                            ease(((Vector3)item.start).x, ((Vector3)item.end).x, percentage),
                            ease(((Vector3)item.start).y, ((Vector3)item.end).y, percentage),
                            ease(((Vector3)item.start).z, ((Vector3)item.end).z, percentage));
                    }
                    if (item.key == "sprite_size_width")
                    {
                        ((UISprite)item.target).width = (int)ease(((int)item.start), ((int)item.end), percentage);
                    }
                    if (item.key == "sprite_size_height")
                    {
                        ((UISprite)item.target).height = (int)ease(((int)item.start), ((int)item.end), percentage);
                    }
                    if (item.key == "widget_depth")
                    {
                        ((UIWidget)item.target).depth = (int)ease(((int)item.start), ((int)item.end), percentage);
                    }
                }
            }

            private void OnDeviceOrientationChange(DeviceOrientation orientation)
            {
                if (orientation == DeviceOrientation.LandscapeLeft || orientation == DeviceOrientation.LandscapeRight)
                    UseLandscapeLayout();
                else if (orientation == DeviceOrientation.Portrait || orientation == DeviceOrientation.PortraitUpsideDown)
                    UsePortraitLayout();
            }

            private void UsePortraitLayout()
            {
                List<GameObject> list = UIManager.Instance.GetAllVisibleUI();
                foreach(GameObject item in list)
                {
                    XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(item.name + "_Portrait");
                    GetUIConfig(node, string.Empty);
                    AdjustUI(item.transform);
                }
            }

            private void UseLandscapeLayout()
            {
                List<GameObject> list = UIManager.Instance.GetAllVisibleUI();
                foreach (GameObject item in list)
                {
                    XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(item.name + "_Landscape");
                    GetUIConfig(node, string.Empty);
                    AdjustUI(item.transform);
                }
            }

            private void GetUIConfig(XmlNode node, string parentPath)
            {
                foreach(XmlNode child in node.ChildNodes)
                {
                    XmlAttributeCollection attrColl = child.Attributes;
                    string xpath = parentPath + "/" + child.Name;
                    partsList.Add(new UIParts(xpath, attrColl));
                    if (child.HasChildNodes) GetUIConfig(child, xpath);
                }
            }

            private void AdjustUI(Transform ui)
            {
                foreach(UIParts item in partsList)
                {
                    Transform t = ui.Find(item.xpath);
                    if (t == null) continue;

                    XmlAttribute attr = null;
                    attr = (XmlAttribute)item.attrColl.GetNamedItem("pos");
                    if (attr != null) partsParamList.Add(new UIPartsParam(t, "pos", StringUtil.StringToVector3(attr.Value)));

                    attr = (XmlAttribute)item.attrColl.GetNamedItem("rot");
                    if (attr != null) partsParamList.Add(new UIPartsParam(t, "rot", StringUtil.StringToVector3(attr.Value)));

                    attr = (XmlAttribute)item.attrColl.GetNamedItem("sca");
                    if (attr != null) partsParamList.Add(new UIPartsParam(t, "sca", StringUtil.StringToVector3(attr.Value)));

                    attr = (XmlAttribute)item.attrColl.GetNamedItem("sprite_size_width");
                    if (attr != null) partsParamList.Add(new UIPartsParam(t.GetComponent<UISprite>(), "sprite_size_width", int.Parse(attr.Value)));

                    attr = (XmlAttribute)item.attrColl.GetNamedItem("sprite_size_height");
                    if (attr != null) partsParamList.Add(new UIPartsParam(t.GetComponent<UISprite>(), "sprite_size_height", int.Parse(attr.Value)));

                    attr = (XmlAttribute)item.attrColl.GetNamedItem("widget_depth");
                    if (attr != null) partsParamList.Add(new UIPartsParam(t.GetComponent<UISprite>(), "widget_depth", int.Parse(attr.Value)));

                    layout = true;
 
                    //Add code here...
                }
            }
        }
    }
}