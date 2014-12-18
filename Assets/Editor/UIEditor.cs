using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/// <summary>
/// Functions:
/// 1、Rename childs
/// 2、Generate config file 
/// </summary>
public sealed class UIEditor : EditorWindow
{
    class TransformInfo
    {
        public Transform trans;
        public string name;
        public TransformInfo(Transform t, string n)
        {
            trans = t;
            name = n;
        }
    }


    private string xmlPath = "";
    private List<GameObject> list = new List<GameObject>();
    private XmlDocument xmlDoc = new XmlDocument();
    private string console = "";
    private Color consoleColor = Color.green;


    private int function = 0;
    private string[] funcDisplayedOptions = new string[]{
        "Batching Rename",
        "Generate Config",
    };
    private int[] funcOptionValues = new int[]{
        0,
        1,
    };

    [MenuItem("Development/UI Editor")]
    static void Init()
    {
        UIEditor editor = (UIEditor)EditorWindow.GetWindow(typeof(UIEditor), true, "UI Editor", true);
        editor.maxSize = new Vector2(500, 300);
        editor.minSize = new Vector2(500, 300);
    }

    void OnGUI()
    {
        GUILayout.Space(5);
        GUILayout.Label("Please select one function below.");
        function = EditorGUILayout.IntPopup(function, funcDisplayedOptions, funcOptionValues, GUILayout.Width(120));

        GUILayout.Space(5);
        if (function == 0){
            BatchingRename();
        }
        else if (function == 1){
            GenerateConfig();
        }
    }

    void BatchingRename()
    {
        GUILayout.Label("-------------------------------------" + funcDisplayedOptions[function] + "-------------------------------------");
    }

    void Rename(Transform parent)
    {
        List<Transform> childList = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            childList.Add(parent.GetChild(i));
        }
    }

   


    void GenerateConfig()
    {
        GUILayout.Label("-------------------------------------" + funcDisplayedOptions[function] + "-------------------------------------");
        GUILayout.Label("File Path: " + xmlPath);
        if (GUILayout.Button("Open", GUILayout.Width(60)))
        {
            xmlPath = EditorUtility.OpenFilePanel("", "", "xml");
        }
        GUILayout.Space(5);
        if (GUILayout.Button("Export", GUILayout.Width(60)))
        {
            consoleColor = Color.green;
            console = "Exporting...";
            Export();
        }
        GUI.contentColor = consoleColor;
        GUILayout.Label(console);
    }

    void Export()
    {
        if (string.IsNullOrEmpty(xmlPath)){
            consoleColor = Color.red;
            console = "The file path is incorrent!";
            return;
        }
        xmlDoc.Load(xmlPath);
        
        foreach (GameObject go in Selection.gameObjects){
            ExportXML(go);
        }

        consoleColor = Color.green;
        console = "Export finish!";
    }

    void ExportXML(GameObject go)
    {
        XmlElement docElement = xmlDoc.DocumentElement;

        XmlNodeList nodeList = docElement.GetElementsByTagName(go.name);
        foreach (XmlNode node in nodeList)
        {
            docElement.RemoveChild(node);
        }

        XmlElement element = xmlDoc.CreateElement(go.name);
        if (go.transform.localPosition != Vector3.zero) element.SetAttribute("pos", go.transform.localPosition.ToString());
        if (go.transform.localEulerAngles != Vector3.zero) element.SetAttribute("rot", go.transform.localEulerAngles.ToString());
        if (go.transform.localScale != Vector3.one) element.SetAttribute("sca", go.transform.localScale.ToString());

        docElement.AppendChild(element);
        ExportXMLRecursively(go, element);
        xmlDoc.Save(xmlPath);
    }

    void ExportXMLRecursively(GameObject go, XmlNode parent)
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
}