using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


/// <summary>
/// Helper class for operate ui,  3 functions below:
/// 1、Batching rename.
/// 2、Batching destroy.
/// 3、Generate config.
/// </summary>
public class UIEditor : EditorWindow
{
    //GUI Function options
    private int function = 0;
    private int[] funcValueOptions = new int[] { 0, 1, 2 };
    private string[] funcDisplayOptions = new string[]
    { 
        "Batching Rename", 
        "Batching Destroy", 
        "Generate Config"
    };
   
    private List<string> consoleList = new List<string>();
    private List<Transform> selectionList = new List<Transform>();
    private XmlDocument xmlDoc = new XmlDocument();
    private string xmlPath = "";
    
    private GameObject portObject;
    private GameObject landObject;

    private List<GameObject> GameObjectList = new List<GameObject>();
    private string killComponentName = "";
    private Vector2 mScroll;


    [MenuItem("Development/UI Editor")]
    static void Init()
    {
        UIEditor editor = (UIEditor)EditorWindow.GetWindow(typeof(UIEditor), true, "UI Editor", true);
        editor.maxSize = new Vector2(550, 600);
        editor.minSize = new Vector2(550, 600);
    }

    void OnEnable()
    {
        GameObjectList.Add(null);
        GameObjectList.Add(null);
    }

    void OnGUI()
    {
        GUILayout.Space(3);      
        GUILayout.Label("Please Select One Function Below: ");
        function = EditorGUILayout.IntPopup(function, funcDisplayOptions, funcValueOptions, GUILayout.Width(120));

        GUILayout.Space(10);
        GUILayout.Label("**************************************************************");

        if (function == 0)
        {
            BatchingRename();
        }
        else if (function == 1)
        {
            BatchingDestroy();
        }
        else if (function == 2)
        {
            GenerateConfig();
        }
    }


    /// <summary>
    /// Batching rename the select transforms.
    /// </summary>   
    private void BatchingRename()
    {
        int index = 0;
        int count = Selection.transforms.Length;

        if (count == 0)
            EditorGUILayout.HelpBox("You must select at least one transform first!", MessageType.Warning);

        EditorGUI.BeginDisabledGroup(count == 0);
        bool rename = GUILayout.Button("Rename");
        EditorGUI.EndDisabledGroup();

        if(rename)
        {
            foreach (Transform item in Selection.transforms) Rename(item);
        }

        mScroll = GUILayout.BeginScrollView(mScroll);
        foreach(Transform item in Selection.transforms)
        {
            GUILayout.Label(string.Format("{0}\t\t{1}", index, item.name));
            index++;
        }
        GUILayout.EndScrollView();
    }


    /// <summary>
    /// Rename the child which has the same name by add suffix "_".
    /// </summary>
    private void Rename(Transform parent)
    {
        if (parent == null) return;
        List<Transform> childList = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            childList.Add(child);
            if (child.childCount > 0)
            {
                Rename(child);
            }
        }
        foreach (Transform t in childList.Uniquify("_")) { }
    }


    /// <summary>
    /// Batching destroy specify component of the seleced transforms.
    /// </summary>
    void BatchingDestroy()
    {
        int index = 0;
        int count = Selection.transforms.Length;

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Component Name: ", GUILayout.Width(100));
        killComponentName = GUILayout.TextField(killComponentName, GUILayout.Width(150));
        GUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("You can enter the name of component which will be destroyed.", MessageType.Info);
        if (count == 0)
            EditorGUILayout.HelpBox("You must select at least one transform first!", MessageType.Warning);

        EditorGUI.BeginDisabledGroup(count == 0);
        bool destroy = GUILayout.Button("Destroy");
        EditorGUI.EndDisabledGroup();

        if(destroy)
        {
            foreach (GameObject item in Selection.gameObjects) DestroyComponent(item);
        }

        mScroll = GUILayout.BeginScrollView(mScroll);
        foreach (Transform item in Selection.transforms)
        {
            GUILayout.Label(string.Format("{0}\t\t{1}", index, item.name));
            index++;
        }
        GUILayout.EndScrollView();
    }

    
    /// <summary>
    /// Destroy the specify component of gameobject.
    /// </summary>
    void DestroyComponent(GameObject go)
    {
        Component cpt = go.GetComponent(killComponentName);
        if (cpt != null) DestroyImmediate(cpt);
        for (int i = 0; i < go.transform.childCount; i++)
        {
            DestroyComponent(go.transform.GetChild(i).gameObject);
        }
    }


    private void GenerateConfig()
    {
        GUILayout.Label("XML File: " + xmlPath);
        bool open = GUILayout.Button("Open");
        bool export =GUILayout.Button("Export");

        if (open)
        {
            xmlPath = EditorUtility.OpenFilePanel("", "", "xml");
        }

        if(export)
        {
            Export();
        }

        GUILayout.Space(10.0f);
        EditorGUIUtility.labelWidth = 80.0f;

        mScroll = EditorGUILayout.BeginScrollView(mScroll);
        for (int i = 0; i+1 < GameObjectList.Count; i=i+2)
        {
            GUILayout.BeginHorizontal();
            GameObjectList[i] = (GameObject)EditorGUILayout.ObjectField("Portrait", GameObjectList[i], typeof(UnityEngine.GameObject), true, GUILayout.Width(230));
            GUILayout.Space(10.0f);
            GameObjectList[i+1] = (GameObject)EditorGUILayout.ObjectField("Landscape", GameObjectList[i+1], typeof(UnityEngine.GameObject), true, GUILayout.Width(230));
            if(GUILayout.Button("-"))
            {
                GameObjectList.RemoveRange(i, 2);
            }
            if(GUILayout.Button("+"))
            {
                GameObjectList.Add(null);
                GameObjectList.Add(null);
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    private void Export()
    {
        if (string.IsNullOrEmpty(xmlPath))
        {
            consoleList.Add("error: file path of xml is incorrect!");
            return;
        }
        xmlDoc.Load(xmlPath);
        ExportXML(portObject, landObject);
    }

    private void ExportXML(GameObject portrait, GameObject landscape)
    {
        XmlElement docElement = xmlDoc.DocumentElement;
        XmlNodeList portraitNodeList = docElement.GetElementsByTagName(portrait.name);
        foreach (XmlNode node in portraitNodeList)
        {
            docElement.RemoveChild(node);
        }

        XmlNodeList landscapeNodeList = docElement.GetElementsByTagName(landscape.name);
        foreach (XmlNode node in landscapeNodeList)
        {
            docElement.RemoveChild(node);
        }

        XmlElement portraitElement = xmlDoc.CreateElement(portrait.name);
        portraitElement.SetAttribute("pos", portrait.transform.localPosition.ToString());
        XmlElement landscapeElement = xmlDoc.CreateElement(landscape.name);
        landscapeElement.SetAttribute("pos", landscape.transform.localPosition.ToString());

        docElement.AppendChild(portraitElement);
        docElement.AppendChild(landscapeElement);
        ExportXMLRecursivelyComparative(portrait.transform, landscape.transform, portraitElement, landscapeElement);
        xmlDoc.Save(xmlPath);
    }

    private void ExportXMLRecursively(GameObject go, XmlNode parent)
    {
        for (int i = 0; i < go.transform.childCount; i++)
        {
            Transform t = go.transform.GetChild(i);
            XmlElement child = xmlDoc.CreateElement(t.name);
            if(t.localPosition != Vector3.zero) child.SetAttribute("pos", t.localPosition.ToString());
            if (t.localEulerAngles != Vector3.zero) child.SetAttribute("rot", t.localEulerAngles.ToString());
            if (t.localScale != Vector3.one) child.SetAttribute("sca", t.localScale.ToString());
            parent.AppendChild(child);
            if (t.childCount > 0) ExportXMLRecursively(t.gameObject, child);
        }
    }

    private void ExportXMLRecursivelyComparative(Transform porTrans, Transform landTrans, XmlNode porParentNode, XmlNode landParentNode)
    {
        for(int i =0; i < porTrans.transform.childCount; i++)
        {
            Transform t1 = porTrans.transform.GetChild(i);
            Transform t2 = landTrans.transform.GetChild(i);
            XmlElement porChildNode = xmlDoc.CreateElement(t1.name);
            XmlElement landChildNode = xmlDoc.CreateElement(t2.name);
            
            if (t1.localPosition != t2.localPosition) {
                porChildNode.SetAttribute("pos", t1.localPosition.ToString());
                landChildNode.SetAttribute("pos", t2.localPosition.ToString());
            }

            if (t1.localEulerAngles != t2.localEulerAngles) {
                porChildNode.SetAttribute("rot", t1.localEulerAngles.ToString());
                landChildNode.SetAttribute("rot", t2.localEulerAngles.ToString());
            }

            if (t1.localScale != t2.localScale) {
                porChildNode.SetAttribute("sca", t1.localScale.ToString());
                landChildNode.SetAttribute("sca", t2.localScale.ToString());
            }


            UISprite sprite1 = t1.GetComponent<UISprite>();
            UISprite sprite2 = t2.GetComponent<UISprite>();
            if (sprite1 != null && sprite2 != null)
            {
                if (sprite1.width != sprite2.width)
                {
                    porChildNode.SetAttribute("sprite_size_width", sprite1.width.ToString());
                    landChildNode.SetAttribute("sprite_size_width", sprite2.width.ToString());
                }

                if (sprite1.height != sprite2.height)
                {
                    porChildNode.SetAttribute("sprite_size_height", sprite1.height.ToString());
                    landChildNode.SetAttribute("sprite_size_height", sprite2.height.ToString());
                }

            }

            if (t1.GetComponent<UIWidget>() != null && t2.GetComponent<UIWidget>() != null)
            {
                if (t1.GetComponent<UIWidget>().depth != t2.GetComponent<UIWidget>().depth)
                {
                    porChildNode.SetAttribute("widget_depth", t1.GetComponent<UIWidget>().depth.ToString());
                    landChildNode.SetAttribute("widget_depth", t2.GetComponent<UIWidget>().depth.ToString());
                }
            }

            porParentNode.AppendChild(porChildNode);
            landParentNode.AppendChild(landChildNode);
            if (t1.childCount > 0) ExportXMLRecursivelyComparative(t1, t2, porChildNode, landChildNode);
        }
    }

    private void Compare()
    {
        consoleList.Clear();
        CompareBasePortrait(portObject.transform, "");
    }

    private void CompareBasePortrait(Transform portrait, string path)
    {
        for(int i=0, imax=portrait.childCount; i < imax; i++)
        {   
            Transform portChild = portrait.GetChild(i);
            string relativePath = path + portChild.name;
            Transform landChild = landObject.transform.Find(relativePath);

            if(landChild == null)
            {
                consoleList.Add(string.Format("child is null: {0}", relativePath));
                continue;
            }

            if (portChild.name != landChild.name)
            {
                consoleList.Add(string.Format("name not equal: {0}", relativePath));
                continue;
            }

            UISprite portSprite = portChild.GetComponent<UISprite>();
            UISprite landSprite = landChild.GetComponent<UISprite>();
            if(portSprite != null)
            {
                if (landSprite == null)
                {
                    consoleList.Add(string.Format("sprite is null: {0}", relativePath));
                    continue;
                }
                
                if (portSprite.atlas != landSprite.atlas)
                {
                    consoleList.Add(string.Format("atlas not equal: {0}", relativePath));
                    continue;
                }

                if (portSprite.spriteName != landSprite.spriteName)
                {
                    consoleList.Add(string.Format("spriteName not equal: {0}", relativePath));
                    continue;
                }

                if(portSprite.rawPivot != landSprite.rawPivot)
                {
                    consoleList.Add(string.Format("Pivot not equal: {0}", relativePath));
                    continue;
                }
            }

            if(portChild.childCount > 0)
            {
                CompareBasePortrait(portChild, relativePath + "/");
            }
        }
    }


    void OnSelectionChange()
    {
        Repaint();
    }
}



public static class EnumerableExtensions
{
    public static IEnumerable<Transform> Uniquify(this IEnumerable<Transform> enumerable, string suffix)
    {
        HashSet<string> prevItems = new HashSet<string>();
        foreach (var item in enumerable)
        {
            var temp = item;
            while (prevItems.Contains(temp.name))
            {
                temp.name += suffix;
            }
            prevItems.Add(temp.name);
            yield return temp;
        }
    }
}