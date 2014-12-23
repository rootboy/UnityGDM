using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/// <summary>
/// UILayout used to adjust ui between Portrait and Landscape.
/// </summary>

public class UILayout : MonoBehaviour 
{
    class UIParts
    {
        public string xpath;
        public XmlAttributeCollection attrColl;
        public UIParts(string _path, XmlAttributeCollection _attrColl)
        {
            xpath = _path;
            attrColl = _attrColl;
        }
    }

    private XmlDocument xmlDoc = new XmlDocument();
    private List<UIParts> partsList = new List<UIParts>();
    private DeviceOrientation deviceOrientation = DeviceOrientation.Portrait;
    public GameObject target;


    public DeviceOrientation Orientation
    {
        get { return deviceOrientation; }
        set 
        {
            if (deviceOrientation != value) {
                if (deviceOrientation == DeviceOrientation.LandscapeLeft) UseLandscapeLayout();
                if (deviceOrientation == DeviceOrientation.Portrait) UsePortraitLayout();
                AdjustUI(target);
            }
        }
    }

    void Awake()
    {
        xmlDoc.Load("C:\\Project_403\\SmartWAR\\Assets\\UIConfig.xml");
        Orientation = Input.deviceOrientation;
    }

#if UNITY_EDITOR

	void OnGUI () 
    {
        if (GUILayout.Button("Portrait"))
        {
            UsePortraitLayout();
        }
        if (GUILayout.Button("Landscape"))
        {
            UseLandscapeLayout();
        }  
	}
#endif

#if !UNITY_EDITOR

    void Update()
    {
        if (Input.deviceOrientation != deviceOrientation)
        {
            if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft ||
                Input.deviceOrientation == DeviceOrientation.LandscapeRight)
            {
                UseLandscapeLayout();
            }
            else if(Input.deviceOrientation == DeviceOrientation.Portrait)
            {
                UsePortraitLayout();
            }
        }
    }

#endif


    //竖屏
    private void UsePortraitLayout()
    {
        XmlNode parent = xmlDoc.DocumentElement.SelectSingleNode("LandPanel3D");
        GetUIPartsInfo(parent, string.Empty);
        AdjustUI(target);
    }

    //横屏
    private void UseLandscapeLayout()
    {
        XmlNode parent = xmlDoc.DocumentElement.SelectSingleNode("LandPanel3DHS");
        GetUIPartsInfo(parent, string.Empty);
        AdjustUI(target);
    }

    private void GetUIPartsInfo(XmlNode parent, string parentPath)
    {
        foreach (XmlNode child in parent.ChildNodes)
        {
            XmlAttributeCollection attrColl = child.Attributes;
            string path = parentPath + child.Name;
            UIParts uiParts = new UIParts(path, attrColl);
            partsList.Add(uiParts);
            if (child.HasChildNodes){
                GetUIPartsInfo(child, path + "/");
            }
        }
    }

    /// <summary>
    /// 适配UI.
    /// </summary>
    private void AdjustUI(GameObject ui)
    {
        foreach (UIParts item in partsList)
        {
            Transform t = ui.transform.Find(item.xpath);
            if (t == null) continue;

            XmlAttribute attr = null;

            attr = (XmlAttribute)item.attrColl.GetNamedItem("pos");
            if (attr != null) t.localPosition = StringToVector3(attr.Value);
            else t.localPosition = Vector3.zero;

            attr = (XmlAttribute)item.attrColl.GetNamedItem("rot");
            if (attr != null) t.localEulerAngles = StringToVector3(attr.Value);
            else t.localEulerAngles = Vector3.zero;

            attr = (XmlAttribute)item.attrColl.GetNamedItem("sca");
            if (attr != null) t.localScale = StringToVector3(attr.Value);
            else t.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 将(x,y,z)形式的字符串转换为Vector3.
    /// </summary>
    private Vector3 StringToVector3(string str)
    {
        string[] array = str.Trim('(', ')').Split(',');
        return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
    }
}