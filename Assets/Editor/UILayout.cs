using UnityEngine;
using System.Collections;
using System.Xml;

public class UILayout : MonoBehaviour 
{
    private XmlDocument xmlDoc = new XmlDocument();

    class UIPartsInfo
    {
        public string path;
        public string attribute;
    }


    void Awake()
    {
        xmlDoc.Load("C:\\Project_403\\SmartWAR\\Assets\\UIConfig.xml");
    }

        
	void OnGUI () 
    {
        if (GUILayout.Button("Portrait")) UsePortraitLayout();
        if (GUILayout.Button("Landscape")) UseLandscapeLeftLayout();
	}


    //竖屏
    void UsePortraitLayout()
    {
        XmlNodeList list = xmlDoc.GetElementsByTagName("LandPanel3D");


        XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("LandPanel3D/PanelDown/Button_More");


    }


    void GetAllUIPartsInfo(XmlNode parent)
    {
        string path = parent.Name;

        foreach(XmlNode child in parent.ChildNodes)
        {


        }
    }



    string GetXmlNodePath(XmlNode node)
    {
        string path = node.Name;

        XmlNode parent = node.ParentNode;
        while (parent.Name != "root")
        {
            path = parent.Name + "/" + path;
            parent = parent.ParentNode;
        }
        return path;
    }


    //横屏
    void UseLandscapeLeftLayout()
    {

    }

    //横屏
    void UseLandscapeRightLayout()
    {

    }
}
