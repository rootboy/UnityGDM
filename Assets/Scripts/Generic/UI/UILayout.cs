using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using SmartWar.Global;

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

    public bool isTween;

    private XmlDocument xmlDoc = new XmlDocument();
    private List<UIParts> partsList = new List<UIParts>();
    private DeviceOrientation deviceOrientation = DeviceOrientation.Portrait;
    public GameObject target;
    private float time = 1.0f;
    private iTween.EaseType easeType = iTween.EaseType.easeInOutBack;

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
        xmlDoc.Load(Application.streamingAssetsPath + "/" + "UIConfig.xml");
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
        //if (GUILayout.Button("Test"))
        //{
        //    UISprite sprite = target.GetComponent<UISprite>();
        //    Vector2 to = new Vector2(5 * sprite.width, sprite.height);

        //    iTween.SpriteScaleTo(target, iTween.Hash
        //        ("time", 2.0f,
        //        "to", to
        //        ));
        //}  
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

<<<<<<< HEAD
    //竖屏
=======
>>>>>>> 732146ae6eb4af09ae91079101326f025de11d19
    private void UsePortraitLayout()
    {
        XmlNode parent = xmlDoc.DocumentElement.SelectSingleNode("LandPanel3D");
        partsList.Clear();
        partsList.Add(new UIParts("", parent.Attributes));
        GetUIPartsInfo(parent, string.Empty);
        AdjustUI(target);
    }

    private void UseLandscapeLayout()
    {
        XmlNode parent = xmlDoc.DocumentElement.SelectSingleNode("LandPanel3DHS");
        partsList.Clear();
        partsList.Add(new UIParts("", parent.Attributes));
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
    /// Adjust UI for landscape or portrait style.
    /// </summary>
    private void AdjustUI(GameObject ui)
    {
        foreach (UIParts item in partsList)
        {
            XmlAttribute attr = null;

            if (item.xpath == string.Empty){
                attr = (XmlAttribute)item.attrColl.GetNamedItem("pos");
                if (attr != null)
                {

                    ui.transform.localPosition = StringToVector3(attr.Value);

                    //if (!isTween)
                    //    ui.transform.localPosition = StringToVector3(attr.Value);
                    //else
                    //    iTween.MoveTo(ui, iTween.Hash(
                    //    "position", StringToVector3(attr.Value),
                    //    "time", time,
                    //    "islocal", true)
                    //    );
                }
                continue;
            }

            Transform t = ui.transform.Find(item.xpath);
            if (t == null) continue;

            attr = (XmlAttribute)item.attrColl.GetNamedItem("pos");
            if (attr != null){
                if(!isTween)
                    t.localPosition = StringToVector3(attr.Value);
                else
                    iTween.MoveTo(t.gameObject,  iTween.Hash(
                    "position", StringToVector3(attr.Value),
                    "time", time,
                    "islocal", true,
                    "easetype", easeType)
                    );
            } 

            attr = (XmlAttribute)item.attrColl.GetNamedItem("rot");
            if (attr != null) {
                if(!isTween)
                    t.localEulerAngles = StringToVector3(attr.Value);
                else
                    iTween.RotateTo(t.gameObject, iTween.Hash(
                    "rotation", StringToVector3(attr.Value),
                    "time", time,
                    "islocal", true)
                    );
            } 

            attr = (XmlAttribute)item.attrColl.GetNamedItem("sca");
            if (attr != null) {
               if(!isTween) 
                   t.localScale = StringToVector3(attr.Value);
               else
                   iTween.ScaleTo(t.gameObject, iTween.Hash(
                   "scale", StringToVector3(attr.Value),
                   "time", time,
                   "islocal", true)
                   );
            } 

            attr = (XmlAttribute)item.attrColl.GetNamedItem("sprite_size");
            if (attr != null) {
                if (!isTween)
                    t.GetComponent<UISprite>().width = int.Parse(attr.Value);
                else
                    iTween.SpriteScaleTo(t.gameObject, iTween.Hash(
                    "to", StringToVector2(attr.Value),
                    "time", time,
                    "easetype", easeType)
                     );
            }

            attr = (XmlAttribute)item.attrColl.GetNamedItem("widget_depth");
            if (attr != null) 
                t.GetComponent<UIWidget>().depth = int.Parse(attr.Value);
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

    private Vector2 StringToVector2(string str)
    {
        string[] array = str.Trim('(', ')').Split(',');
        return new Vector2(float.Parse(array[0]), float.Parse(array[1]));
    }
}