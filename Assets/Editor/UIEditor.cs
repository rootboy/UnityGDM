using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/// <summary>
/// Helper class for operate ui.
/// </summary>
public class UIEditor : EditorWindow
{
    private string xmlPath = "";
    private List<GameObject> list = new List<GameObject>();
    private XmlDocument xmlDoc = new XmlDocument();
    private string console = "";
    private Color consoleColor = Color.green;

    private int function = 0;
    private string[] funcDisplayedOptions = new string[]{
        "Batching Rename",
        "Generate Config",
        "Destroy Component",
    };
    private int[] funcOptionValues = new int[]{
        0,
        1,
        2,
    };


    private GameObject portrait;
    private GameObject landscape;
    private string destroyComponent = string.Empty;

    [MenuItem("Development/UI Editor")]
    static void Init()
    {
        UIEditor editor = (UIEditor)EditorWindow.GetWindow(typeof(UIEditor), true, "UI Editor", true);
        editor.maxSize = new Vector2(500, 300);
        editor.minSize = new Vector2(500, 300);
    }

    void OnGUI()
    {
        GUILayout.Space(3);      
        GUILayout.BeginHorizontal();
        GUILayout.Label("Select one function: ", GUILayout.Width(120));
        function = EditorGUILayout.IntPopup(function, funcDisplayedOptions, funcOptionValues, GUILayout.Width(120));
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        if (function == 0){
            BatchingRename();
        }
        else if (function == 1){
            GenerateConfig();
        }
        else if (function == 2){
            DestroyComponent();
        }
    }

    #region Batching Rename

    private void BatchingRename()
    {
        GUILayout.Label("-------------------------------------" + funcDisplayedOptions[function] + "-------------------------------------");
        GUILayout.Space(5);

        if (GUILayout.Button("Rename", GUILayout.Width(60))){
            Rename(Selection.activeGameObject.transform);
        }
    }

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

    #endregion


    #region Generate Config

    private void GenerateConfig()
    {
        GUILayout.Label("-------------------------------------" + funcDisplayedOptions[function] + "-------------------------------------");
        portrait =  (GameObject)EditorGUILayout.ObjectField("portrait", portrait, typeof(GameObject), true);
        landscape = (GameObject)EditorGUILayout.ObjectField("landscape", landscape, typeof(GameObject), true);
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

    private void Export()
    {
        if (string.IsNullOrEmpty(xmlPath)){
            consoleColor = Color.red;
            console = "The file xpath is incorrent!";
            return;
        }
        xmlDoc.Load(xmlPath);
        ExportXML(portrait, landscape);

        consoleColor = Color.green;
        console = "Export finish!";
    }

    private void ExportXML(GameObject portrait, GameObject landscape)
    {
        XmlElement docElement = xmlDoc.DocumentElement;
        XmlNodeList portraitNodeList = docElement.GetElementsByTagName(portrait.name);
        foreach (XmlNode node in portraitNodeList){
            docElement.RemoveChild(node);
        }

        XmlNodeList landscapeNodeList = docElement.GetElementsByTagName(landscape.name);
        foreach (XmlNode node in landscapeNodeList){
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
            if (sprite1 != null && sprite2 != null){
                if (sprite1.width != sprite2.width ||
                    sprite1.height != sprite2.height)
                {
                    porChildNode.SetAttribute("sprite_size", new Vector2(sprite1.width, sprite1.height).ToString());
                    landChildNode.SetAttribute("sprite_size", new Vector2(sprite2.width, sprite2.height).ToString());
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
    #endregion


    #region DestroyComponent

    void DestroyComponent()
    {
        GUILayout.Label("-------------------------------------" + funcDisplayedOptions[function] + "-------------------------------------");
        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Component: ", GUILayout.Width(100));
        destroyComponent = GUILayout.TextField(destroyComponent, GUILayout.Width(150));
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Destroy", GUILayout.Width(60)))
        {
            foreach (GameObject item in Selection.gameObjects)
            {
                DestroyComponent(item);
            }
        }
    }

    void DestroyComponent(GameObject go)
    {
        Component cpt = go.GetComponent(destroyComponent);
        if (cpt != null) DestroyImmediate(cpt);
        for(int i=0; i<go.transform.childCount;i++)
        {
            DestroyComponent(go.transform.GetChild(i).gameObject);
        }
    }

    #endregion
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