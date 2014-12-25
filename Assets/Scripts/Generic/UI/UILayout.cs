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
                public Transform t;
                public string key;
                public object param;

                public UIPartsParam(Transform _t, string _key, object _param)
                {
                    t = _t;
                    key = _key;
                    param = _param;
                }
            }

            public bool isTween;
            public float tweenTime = 1.0f;
            public iTween.EaseType easeType = iTween.EaseType.easeInOutBack;

            XmlDocument xmlDoc = new XmlDocument();
            List<UIParts> partsList = new List<UIParts>();
            List<UIPartsParam> partsParamList = new List<UIPartsParam>();
            bool layout = false;

            void Awake()
            {
                xmlDoc.Load(Application.streamingAssetsPath + "/" + "UIConfig.xml");
            }

            void Update()
            {
                if (layout){

                    //TODO: real layout functions here.
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

                    attr = (XmlAttribute)item.attrColl.GetNamedItem("sprite_size");
                    if (attr != null) partsParamList.Add(new UIPartsParam(t, "sprite_size", StringUtil.StringToVector2(attr.Value)));

                    attr = (XmlAttribute)item.attrColl.GetNamedItem("widget_depth");
                    if (attr != null) partsParamList.Add(new UIPartsParam(t, "widget_depth", int.Parse(attr.Value)));

                    layout = true;
 
                    //Add code here...
                }
            }
        }
    }
}