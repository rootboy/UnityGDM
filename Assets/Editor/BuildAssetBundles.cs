using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Threading;

public class BuildAssetBundles
{

#if UNITY_IPHONE
	    const BuildTarget target=BuildTarget.iPhone;
#elif UNITY_ANDROID
	    const BuildTarget target=BuildTarget.Android;
#elif UNITY_WP8
	    const BuildTarget target=BuildTarget.WP8Player;
#else
    const BuildTarget target = BuildTarget.StandaloneWindows;
#endif

    private const BuildAssetBundleOptions optionsDefault = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.CollectDependencies
        | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.UncompressedAssetBundle;

    private const string wwwPrefix = "file://";
    private const string assetPrefix = "Assets/AssetBundles";
    //private static string path = Application.persistentDataPath + "/" + outPath + "/" + target.ToString() + "/";
	private static string assetPath = assetPrefix + "/" + target.ToString();
    private static string inLuaPath = "Assets/Lua";
    private static string outLuaPath = "Assets/LuaTmp";
    private static string inLocalizationPath = "Assets/Data";
    private static string outLocalizationPath = "Assets/DataTmp";
    //res types need to be updated
    private static ArrayList resTypes = new ArrayList() { typeof(Texture2D), typeof(AudioClip), typeof(AnimationClip)};

    private const string META_EXTENSION = ".meta";
    private const string MANIFEST_FILE = "manifest.txt";
    private const string CONFIG_FILE = "config.txt";
    private const string RESOURCE_PATH = "Assets/Resource";

    private static DirectoryInfo inputAssetPath = new DirectoryInfo(assetPath);
    //private static DirectoryInfo outputPath = new DirectoryInfo(Application.streamingAssetsPath);
    private static DirectoryInfo inputLuaPath = new DirectoryInfo(inLuaPath);
    //need change to your own local server path
    private static DirectoryInfo serverPath = new DirectoryInfo(@"C:\inetpub\wwwroot\autoupdate");
    private static DirectoryInfo outputPath = serverPath;
    private static DirectoryInfo resourcePath = new DirectoryInfo(RESOURCE_PATH);
    #region menuitem

    [MenuItem("Assets/[SmartWar] Build selected Assets with dependent prefabs and resources")]
    static void BuildAssetBundleWithResourceMenu()
    {
        BuildAssetBundleWithResource();
    }

    [MenuItem("Assets/[SmartWar] Build selected Assets with dependent prefabs and resources packaged ")]
    static void BuildAssetBundleWithPackagedResourceMenu()
    {
        BuildAssetBundleWithPackagedResource();
    }

    [MenuItem("Assets/[SmartWar] Build selected Assets with dependencies all packaged")]
    static void BuildAssetBundlePackagedMenu()
    {
        BuildAssetBundlePackaged();
    }

    [MenuItem("Assets/[SmartWar] Package selected Assets into a single AssetBundle")]
    static void BuildAllAssetBundleMenu()
    {
        BuildAllAssetBundle();
    }

    [MenuItem("Assets/[SmartWar] AssetBundle Auto-Package")]
    static void AutoBuildAllAssetBundleMenu()
    {
        AutoBuildAllAssetBundle();
    }

    [MenuItem("Assets/[SmartWar] Check Reduplicate Resource Name")]
    static void CheckReduplicateResMenu()
    {
        if (!CheckReduplicateRes2())
            Debug.Log("no reduplicate filename");
    }


    [MenuItem("Assets/[SmartWar] BuildTest")]
    static void BuildTestMenu()
    {
        BuildTest();
    }

    #endregion

    #region protected

    private static bool CheckReduplicateFolder()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resource");
        List<string> dirNames = new List<string>();
        bool reduplicate = false;
        foreach (DirectoryInfo di in dir.GetDirectories("*", SearchOption.AllDirectories))
        {
            FileInfo[] fis = di.GetFiles("*.prefab", SearchOption.AllDirectories);
            if(fis != null && fis.Length > 0)
            {
                if (dirNames.Contains(di.Name))
                {
                    Debug.LogError("reduplicate folder name: " + di.Name);
                    reduplicate = true;
                }
                else
                {
                    dirNames.Add(di.Name);
                }
            }
        }

        return reduplicate;
    }

    private static bool CheckReduplicateLuaFile()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Lua");
        List<string> fileNames = new List<string>();
        bool reduplicate = false;
        foreach (FileInfo fi in dir.GetFiles("*.lua", SearchOption.AllDirectories))
        {
            if (fileNames.Contains(fi.Name))
            {
                Debug.LogError("reduplicate file name: " + fi.Name);
                reduplicate = true;
            }
            else
            {
                fileNames.Add(fi.Name);
            }
        }

        return reduplicate;
    }

    //private static bool Check

    //TODO: 文件夹名字也要check, 重构，当前check效率太低，改成directory和file存入一个不可重复的数据结构中，dictionary
    private static bool CheckReduplicateRes2()
    {
        bool reduplicate = false;
        DirectoryInfo prefabDir = new DirectoryInfo(Application.dataPath + "/Prefab");
        DirectoryInfo ResDir = new DirectoryInfo(Application.dataPath + "/Resources");

        FileInfo[] fis;
        foreach (FileInfo fi in prefabDir.GetFiles("*.*", SearchOption.AllDirectories))
        {
            if (fi.Extension == META_EXTENSION)
                continue;
            fis = prefabDir.GetFiles(fi.Name.Substring(0, fi.Name.IndexOf(".")) + ".*", SearchOption.AllDirectories);

            if (fis != null)
            {
                foreach (FileInfo f in fis)
                {
                    //Debug.LogError(f.FullName);
                    if (f.FullName != fi.FullName && f.Extension != META_EXTENSION)
                    {
                        Debug.LogError("reduplicate filename: " + fi.FullName + ", " + f.FullName);
                        reduplicate = true;
                    }
                }
            }

            fis = ResDir.GetFiles(fi.Name.Substring(0, fi.Name.IndexOf(".")) + ".*", SearchOption.AllDirectories);
            if (fis != null)
            {
                foreach (FileInfo f in fis)
                {
                    if (f.Extension != META_EXTENSION)
                    {
                        Debug.LogError("reduplicate filename: " + fi.FullName + ", " + f.FullName);
                        reduplicate = true;
                    }
                }
            }
        }

        foreach (FileInfo fi in ResDir.GetFiles("*.*", SearchOption.AllDirectories))
        {
            if (fi.Extension == META_EXTENSION)
                continue;
            fis = ResDir.GetFiles(fi.Name.Substring(0, fi.Name.IndexOf(".")) + ".*", SearchOption.AllDirectories);
            if (fis != null)
            {
                foreach (FileInfo f in fis)
                {
                    if (f.FullName != fi.FullName && f.Extension != META_EXTENSION)
                    {
                        Debug.LogError("reduplicate filename: " + fi.FullName + ", " + f.FullName);
                        reduplicate = true;
                    }
                }
            }

            fis = prefabDir.GetFiles(fi.Name.Substring(0, fi.Name.IndexOf(".")) + ".*", SearchOption.AllDirectories);
            if (fis != null)
            {
                foreach (FileInfo f in fis)
                {
                    if (f.Extension != META_EXTENSION)
                    {
                        Debug.LogError("reduplicate filename: " + fi.FullName + ", " + f.FullName);
                        reduplicate = true;
                    }
                }
            }
        }
        return reduplicate;
    }

    private static void CheckAssetsBundlePath()
    {
        if (!Directory.Exists(assetPath))
        {
            Directory.CreateDirectory(assetPath);
        }

    }

    private static void BuildTest()
    {
        /*
        GameObject cube = AssetDatabase.LoadAssetAtPath("Assets/Prefab/Cube.prefab", typeof(GameObject)) as GameObject;
        GameObject sphere = AssetDatabase.LoadAssetAtPath("Assets/Prefab/Sphere.prefab", typeof(GameObject)) as GameObject;

        Object[] denpendencies = EditorUtility.CollectDependencies(new Object[] { cube, sphere });
        BuildPipeline.PushAssetDependencies();
        foreach (Object res in denpendencies)
        {
            if (resTypes.Contains(res.GetType()))
            {
                Debug.Log("Build Resource: " + res);
                BuildPipeline.BuildAssetBundle(res, null, Path.Combine(assetPath, res.name + ".ab"), optionsDefault, target);
            }
        }
        BuildPipeline.PushAssetDependencies();
        BuildPipeline.BuildAssetBundle(cube, null, Path.Combine(assetPath, cube.name + ".ab"), optionsDefault, target);
        BuildPipeline.PopAssetDependencies();
        BuildPipeline.PushAssetDependencies();
        BuildPipeline.BuildAssetBundle(sphere, null, Path.Combine(assetPath, sphere.name + ".ab"), optionsDefault, target);
        BuildPipeline.PopAssetDependencies();*/

        

        GameObject sprite = AssetDatabase.LoadAssetAtPath("Assets/Resource/NGUI/Sprite.prefab", typeof(GameObject)) as GameObject;
        Debug.Log(sprite.name);
        Object[] denpendencies = EditorUtility.CollectDependencies(new Object[] { sprite });
        foreach (Object res in denpendencies)
        {
                Debug.Log("Build Resource: " + res);
                //BuildPipeline.BuildAssetBundle(res, null, Path.Combine(assetPath, res.name + ".ab"), optionsDefault, target);
        }
    }

    private static void BuildAssetBundleWithResource()
    {
        CheckAssetsBundlePath();

        foreach (Object mainPrefab in Selection.objects)
        {
            if (mainPrefab is GameObject)
            {
                Object[] denpendencies = EditorUtility.CollectDependencies(new Object[] { mainPrefab });

                ArrayList resList = new ArrayList();

                StringBuilder paths = new StringBuilder();

                BuildPipeline.PushAssetDependencies();
                bool haveResource = false, havePrefab = false;

                foreach (Object res in denpendencies)
                {
                    if (resTypes.Contains(res.GetType()))
                    {
                        Debug.Log("Build Resource: " + res);
						BuildPipeline.BuildAssetBundle(res, null, assetPath + "/" + res.name + ".ab", optionsDefault, target);
                        resList.Add(res.name);
                        paths.Append(res.name);
                        paths.Append(":");
                        paths.Append(res.name);
                        paths.Append(";");
                        haveResource = true;
                    }
                }
                if (haveResource) BuildPipeline.PushAssetDependencies();

                foreach (Object prefab in denpendencies)
                {
                    //resources related container excluded
                    if (prefab is GameObject && !resList.Contains(prefab.name) && AssetDatabase.GetAssetPath(prefab) != AssetDatabase.GetAssetPath(mainPrefab))
                    {
                        Debug.Log("Build Prefab: " + prefab);
						BuildPipeline.BuildAssetBundle(prefab, null, assetPath + "/" + prefab.name + ".ab", optionsDefault, target);
                        paths.Append(prefab.name);
                        paths.Append(":");
                        paths.Append(prefab.name);
                        paths.Append(";");
                        havePrefab = true;
                    }
                }

                if (havePrefab) BuildPipeline.PushAssetDependencies();

                if (paths.ToString() != "")
                {
                    GameObject game = Resources.LoadAssetAtPath("Assets/Editor/AssetDependencies.prefab", typeof(GameObject)) as GameObject;
                    game.GetComponent<CDependencies>().paths = paths.ToString().Substring(0, paths.ToString().Length - 1);
                    Debug.Log("Build main Prefab and dependencies: " + mainPrefab);
					BuildPipeline.BuildAssetBundle(mainPrefab, new Object[] { game }, assetPath + "/" + mainPrefab.name + ".ab", optionsDefault, target);
                }
                else
                {
                    Debug.Log("Build main Prefab: " + mainPrefab);
					BuildPipeline.BuildAssetBundle(mainPrefab, null, assetPath + "/" + mainPrefab.name + ".ab", optionsDefault, target);
                }

                if (havePrefab) BuildPipeline.PopAssetDependencies();
                if (haveResource) BuildPipeline.PopAssetDependencies();
                BuildPipeline.PopAssetDependencies();
            }
        }

        EditorUtility.UnloadUnusedAssets();
    }

    private static void BuildAssetBundleWithPackagedResource()
    {
        CheckAssetsBundlePath();

        foreach (Object mainPrefab in Selection.objects)
        {
            if (mainPrefab is GameObject)
            {
                Object[] denpendencies = EditorUtility.CollectDependencies(new Object[] { mainPrefab });

                List<string> resList = new List<string>();

                StringBuilder paths = new StringBuilder();

                BuildPipeline.PushAssetDependencies();
                bool haveResource = false, havePrefab = false;

                List<Object> resources = new List<Object>();
                foreach (Object res in denpendencies)
                {
                    if (resTypes.Contains(res.GetType()))
                    {
                        resources.Add(res);
                        resList.Add(res.name);
                        
                    }
                }
                if (resources.Count > 0)
                {
                    Object firstRes = resources[0];
                    string firstResName = firstRes.name;

                    Debug.Log("Build Resource: " + firstResName);
					BuildPipeline.BuildAssetBundle(firstRes, resources.ToArray(), assetPath + "/" + firstResName + ".ab", optionsDefault, target);

                    foreach (string name in resList)
                    {
                        paths.Append(firstResName);
                        paths.Append(":");
                        paths.Append(name);
                        paths.Append(";");
                    }
                    haveResource = true;
                }


                if (haveResource) BuildPipeline.PushAssetDependencies();

                foreach (Object prefab in denpendencies)
                {
                    //resources related container excluded
                    if (prefab is GameObject && !resList.Contains(prefab.name) && AssetDatabase.GetAssetPath(prefab) != AssetDatabase.GetAssetPath(mainPrefab))
                    {
                        Debug.Log("Build Prefab: " + prefab);
						BuildPipeline.BuildAssetBundle(prefab, null, assetPath + "/" + prefab.name + ".ab", optionsDefault, target);
                        paths.Append(prefab.name);
                        paths.Append(":");
                        paths.Append(prefab.name);
                        paths.Append(";");
                        havePrefab = true;
                    }
                }

                if (havePrefab) BuildPipeline.PushAssetDependencies();

                if (paths.ToString() != "")
                {
                    GameObject game = Resources.LoadAssetAtPath("Assets/Editor/AssetDependencies.prefab", typeof(GameObject)) as GameObject;
                    game.GetComponent<CDependencies>().paths = paths.ToString().Substring(0, paths.ToString().Length - 1);
                    Debug.Log("Build main Prefab and dependencies: " + mainPrefab);
                    
					BuildPipeline.BuildAssetBundle(mainPrefab, new Object[] { game }, assetPath + "/" + mainPrefab.name + ".ab", optionsDefault, target);
                }
                else
                {
                    Debug.Log("Build main Prefab: " + mainPrefab);
					BuildPipeline.BuildAssetBundle(mainPrefab, null, assetPath + "/" + mainPrefab.name + ".ab", optionsDefault, target);
                }

                if (havePrefab) BuildPipeline.PopAssetDependencies();
                if (haveResource) BuildPipeline.PopAssetDependencies();
                BuildPipeline.PopAssetDependencies();
            }
        }

        EditorUtility.UnloadUnusedAssets();
    }



    private static void BuildAssetBundlePackaged()
    {
        CheckAssetsBundlePath();

        foreach (Object mainPrefab in Selection.objects)
        {
            if (mainPrefab is GameObject)
            {
                Debug.Log("Build main Prefab: " + mainPrefab);
				BuildPipeline.BuildAssetBundle(mainPrefab, null, assetPath + "/" + mainPrefab.name + ".ab", optionsDefault, target);
            }
        }


        EditorUtility.UnloadUnusedAssets();
    }

    public static void CompressToServer(FileInfo fi)
    {
        /*MemoryStream ms = new MemoryStream();
        GZipOutputStream gzip = new GZipOutputStream(ms);

        FileStream infile;
        infile = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
        byte[] buffer = new byte[infile.Length];
        int count = infile.Read(buffer, 0, buffer.Length);
        infile.Close();

        gzip.Write(buffer, 0, buffer.Length);
        gzip.Close();

        byte[] press = ms.ToArray();
        */
        string relativePath = fi.FullName.Substring(inputAssetPath.FullName.Length + 1);

		FileInfo fo = new FileInfo(outputPath.FullName + "/" + relativePath);

        //支持子文件夹
        if (!Directory.Exists(fo.DirectoryName))
        {
            Directory.CreateDirectory(fo.DirectoryName);
        }

        //ZipUtil.CompressFileLZMA(fi.FullName, fo.FullName);
        /*FileStream fs = new FileStream(fo.FullName, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        fs.Write(press, 0, press.Length);
        sw.Close();
        fs.Close();*/
    }

    private static void BuildAllAssetBundle()
    {
        CheckAssetsBundlePath();

        List<Object> prefabs = new List<Object>();
        foreach (Object prefab in Selection.objects)
        {
            if (prefab is GameObject)
            {
                prefabs.Add(prefab);
            }
        }

        if (prefabs.Count > 0)
        {
            Debug.Log("Build all Prefabs: " + prefabs.Count);
			BuildPipeline.BuildAssetBundle(prefabs[0], prefabs.ToArray(), assetPath + "/" + prefabs[0].name + ".ab", optionsDefault, target);
        }

        EditorUtility.UnloadUnusedAssets();
    }

    private static string GetAssetPath(string fullpath)
    {
        return "Assets" + fullpath.Replace("\\", "/").Replace(Application.dataPath, "");
    }

    private static Object LoadPrefabAtPath(string fullpath)
    {
        return AssetDatabase.LoadAssetAtPath(GetAssetPath(fullpath), typeof(GameObject));
    }

    class AssetBundleNode
    {
        public List<Object> assets;
        public List<AssetBundleNode> dependencies;
        public string name;

        public AssetBundleNode(string n)
        {
            List<Object> assets = new List<Object>();
            List<AssetBundleNode> dependencies = new List<AssetBundleNode>();
            name = n;
        }
    }

    private static AssetBundleNode GenerateAssetBundleGraph()
    {
        AssetBundleNode root = new AssetBundleNode("Root");


        return root;
    }
    
    private static void BuildAssetBundleInDirectory(DirectoryInfo di)
    {
        FileInfo[] fis = di.GetFiles("*.prefab");
        if (fis != null && fis.Length != 0)
        {
            List<Object> assets = new List<Object>();
            foreach (FileInfo fi in fis)
            {
                assets.Add(LoadPrefabAtPath(fi.FullName));
            }

            Object[] dependencies = EditorUtility.CollectDependencies(assets.ToArray());

            foreach (Object d in dependencies)
            {
                if (!d)
                    continue;
                if (d.GetType() == typeof(GameObject))
                {
                    DirectoryInfo ddi = new FileInfo(AssetDatabase.GetAssetPath(d)).Directory;
                    //Debug.Log(ddi.FullName);
                    if (!ddi.FullName.Equals(di.FullName))
                    {
                        Debug.LogError("can not have the third layer dependencies:" + di.Name + "->" + d.name);
                    }
                }
            }

            Debug.Log("Build dependencies in dir " + di.Name);
			BuildPipeline.BuildAssetBundle(assets[0], assets.ToArray(), assetPath + "/" + di.Name + ".ab", optionsDefault, target);

        }


    }
    private static void BuildAssetBundleInDirectoryR(DirectoryInfo di)
    {
        FileInfo[] fis = di.GetFiles("*.prefab");
        System.Array.Sort(fis, new FileSystemInfoComparer());
        if (fis != null && fis.Length != 0)
        {
            List<Object> assets = new List<Object>();
            foreach (FileInfo fi in fis)
            {
                assets.Add(LoadPrefabAtPath(fi.FullName));
            }

            //如果输出路径里已包含同名的包，仍需继续再打，以记录依赖关系，在依赖项打包的时候可能会出现
            //TODO：需测试验证是否正确

            Object[] dependencies = EditorUtility.CollectDependencies(assets.ToArray());
            System.Array.Sort(dependencies, new ObjectComparer());
            List<string> dependentPaths = new List<string>();
            StringBuilder pathStr = new StringBuilder();
            bool hasPrefab = false;

            foreach (Object d in dependencies)
            {
                if (!d)
                    continue;
                if (d.GetType() == typeof(GameObject))
                {
                    DirectoryInfo ddi = new FileInfo(AssetDatabase.GetAssetPath(d)).Directory;
                    //Debug.Log(ddi.FullName);
                    if (!ddi.FullName.Equals(di.FullName) && !dependentPaths.Contains(ddi.FullName))
                    {
                        //Debug.Log(ddi.Name);
                        dependentPaths.Add(ddi.FullName);
                        pathStr.Append(ddi.Name);
                        pathStr.Append(";");
                    }
                }
            }

            if (dependentPaths.Count > 0)
            {
                hasPrefab = true;
                //需保证这里只有一层递归，如有多层会出现跨层冗余，但本质上影响不大
                BuildPipeline.PushAssetDependencies();
                foreach (string ddi in dependentPaths)
                {
                    BuildAssetBundleInDirectory(new DirectoryInfo(ddi));
                }

                GameObject game = Resources.LoadAssetAtPath("Assets/Editor/AssetDependencies.prefab", typeof(GameObject)) as GameObject;
                game.GetComponent<CDependencies>().paths = pathStr.ToString().Substring(0, pathStr.ToString().Length - 1);
                //BuildPipeline.BuildAssetBundle(mainPrefab, new Object[] { game }, Path.Combine(assetPath, mainPrefab.name + ".ab"), optionsDefault, target);
                assets.Add(game);
                Debug.Log("Build Prefab in dir " + di.Name + ", and dependencies dir " + pathStr.ToString().Substring(0, pathStr.ToString().Length - 1));
            }
            else
            {
                Debug.Log("Build Prefabs in dir " + di.Name);
            }

            //有两种实现方式，1. 预先整理出所有prefabs的依赖关系，再按依赖层级进行打包，
            //2.直接依次打包，如果测试通过的话，依赖关系依然存在，但打包效率会比前一种低，而且可能存在潜在问题
            //如果依赖的是一个prefab，且此prefab不在当前文件夹下，则把这个prefab及其同目录下的prefab打成一个包，此处使用递归，应避免环形依赖
            //mainprefab选第一个，以支持当集合只有一个元素时能通过此API直接取出
            BuildPipeline.PushAssetDependencies();
			BuildPipeline.BuildAssetBundle(assets[0], assets.ToArray(), assetPath + "/" + di.Name + ".ab", optionsDefault, target);
            BuildPipeline.PopAssetDependencies();

            if (hasPrefab)
            {
                BuildPipeline.PopAssetDependencies();
            }
        }
    }
    
    private static void BuildAssetBundleRecursively(DirectoryInfo di)
    {
        BuildAssetBundleInDirectoryR(di);

        DirectoryInfo[] dis = di.GetDirectories();
        System.Array.Sort(dis, new FileSystemInfoComparer());
        if (dis != null && dis.Length != 0)
        { 
            foreach(DirectoryInfo d in dis)
            {
                BuildAssetBundleRecursively(d);    
            }
        }
    }

    public class ObjectComparer : IComparer<Object>
    {
        public int Compare(Object obj1, Object obj2)
        {
            return obj1.name.CompareTo(obj2.name);
        }
    }

    public class FileSystemInfoComparer : IComparer<FileSystemInfo>
    {
        public int Compare(FileSystemInfo fsi1, FileSystemInfo fsi2)
        {
            return fsi1.FullName.CompareTo(fsi2.FullName);
        }
    }

    //按文件系统管理的方式来打包
    //Base Resource push first, and in memory always: shader, font, audio, animation?
    //lua, shader, font, prefab(ngui, 3dscene)
    //Texture2D, Mesh, Shader, AudioClip, AnimationClip in prefab will package alone
    //model effect按文件夹打，无依赖关系
    //sound 可能会打成一个包，一次性加载
    private static void AutoBuildAllAssetBundle()
    {
        //check duplicate name first
        //if check every time takes too long, remove this check and check it manually
        if (CheckReduplicateFolder())
        {
            Debug.LogError("build failed because of reduplicate folder name");
            return;
        }


        //每次一键build之前都需要清空assetbundle文件夹
        if (!inputAssetPath.Exists)
        {
            inputAssetPath.Create();
        }
        else
        {
            inputAssetPath.Delete(true);
            inputAssetPath.Create();
        }

        BuildPipeline.PushAssetDependencies();

        FileInfo[] fis = resourcePath.GetFiles("*.prefab", SearchOption.AllDirectories);


        //better performance than arraylist
        Object[] prefabs = new Object[fis.Length];

        for (int i = 0; i < fis.Length; ++i)
        {
            prefabs[i] = LoadPrefabAtPath(fis[i].FullName);
            Object[] dc = EditorUtility.CollectDependencies(new Object[]{prefabs[i]});
            foreach (Object o in dc)
            {
                if (o == null)
                    Debug.LogError(fis[i].FullName);
            }
            
        }

        Object[] dependencies = EditorUtility.CollectDependencies(prefabs);

        List<Object> shaders = new List<Object>();
        System.Array.Sort(dependencies, new ObjectComparer());

        //寻找所有用到的shader，并打成一个包 push
        foreach (Object d in dependencies)
        {
            if (!d)
                continue;
            if (d.GetType() == typeof(Shader))
            {
                shaders.Add(d);
            }
            //other公共依赖包，除shader以外，可能会有字体，还需要在这里单独处理
        }

		BuildPipeline.BuildAssetBundle(null, shaders.ToArray(), assetPath + "/" + "shader.ab", optionsDefault, target);

        BuildAssetBundleRecursively(resourcePath);
        
        BuildPipeline.PopAssetDependencies();

        BuildLuaAssetBundle();
        BuildLocalizaionAssetBundle();

        //ExportToServer();
    }

    [MenuItem("Assets/BuildLuaOnly")]
    public static void BuildLuaOnly()
    {

        BuildLuaAssetBundle();
    }

    [MenuItem("Assets/[SmartWar] Build Localizaion AssetBundle")]
    private static void BuildLocalizaionAssetBundle()
    {
        DirectoryInfo inputLocalizationPath = new DirectoryInfo(inLocalizationPath);

        string relativePath;

        DirectoryInfo outputLocalizationPath = new DirectoryInfo(outLocalizationPath);

        if (!outputLocalizationPath.Exists)
        {
            outputLocalizationPath.Create();
        }
        else
        {
            outputLocalizationPath.Delete(true);
            outputLocalizationPath.Create();
        }

        string targetPath;
        foreach (FileInfo fsi in inputLocalizationPath.GetFiles("*.*", SearchOption.AllDirectories))
        {
            if (fsi.Extension != META_EXTENSION)
            {
                targetPath = outputLocalizationPath.FullName + "/" + fsi.Name + ".bytes";

                if (File.Exists(targetPath))
                {
                    Debug.LogError("build lua failed: have duplicate name files:" + fsi.Name);
                    return;
                }

                File.Copy(fsi.FullName, targetPath, false);
            }
        }

        AssetDatabase.Refresh();

        FileInfo[] fis = outputLocalizationPath.GetFiles("*.bytes", SearchOption.AllDirectories);
        Object[] datas = new Object[fis.Length];
        for (int i = 0; i < fis.Length; i++)
        {
            relativePath = fis[i].FullName.Substring(outputLocalizationPath.FullName.Length + 1);
            //Debug.LogError(outLuaPath + "/" + relativePath);
            Object o = Resources.LoadAssetAtPath(outLocalizationPath + "/" + relativePath, typeof(TextAsset));
            datas[i] = o;
        }

        if (datas.Length > 0)
        {
            BuildPipeline.BuildAssetBundle(null, datas, assetPath + "/" + "localization.ab", optionsDefault, target);
            Debug.Log("build localization.ab successfully");
        }

        outputLocalizationPath.Delete(true);
    }

    private static void BuildLuaAssetBundle()
    {
        if (CheckReduplicateLuaFile())
        {
            Debug.LogError("build failed because of reduplicate lua filename");
            return;
        }

        DirectoryInfo inputLuaPath = new DirectoryInfo(inLuaPath);

        string relativePath;

        DirectoryInfo outputLuaPath = new DirectoryInfo(outLuaPath);

        if (!outputLuaPath.Exists)
        {
            outputLuaPath.Create();
        }
        else
        {
            outputLuaPath.Delete(true);
            outputLuaPath.Create();
        }

        string targetPath;
        foreach (FileInfo fsi in inputLuaPath.GetFiles("*.*ua", SearchOption.AllDirectories))
        {
            if (fsi.Extension != META_EXTENSION)
            {
                string fsiName = fsi.Name.Substring(0, fsi.Name.Length - 4);
                targetPath = outputLuaPath.FullName + "/" + fsiName + ".bytes";

                if (File.Exists(targetPath))
                {
                    Debug.LogError("build lua failed: have duplicate name files:" + fsi.Name);
                    return;
                }

                File.Copy(fsi.FullName, targetPath, false);
            }
        }

        AssetDatabase.Refresh();

        FileInfo[] fis = outputLuaPath.GetFiles("*.bytes", SearchOption.AllDirectories);
        Object[] luas = new Object[fis.Length];
        for (int i = 0; i < fis.Length; i++)
        {
            relativePath = fis[i].FullName.Substring(outputLuaPath.FullName.Length + 1);
            Object o = Resources.LoadAssetAtPath(outLuaPath + "/" + relativePath, typeof(TextAsset));
            luas[i] = o;
        }

        if (luas.Length > 0)
        {
            BuildPipeline.BuildAssetBundle(null, luas, assetPath + "/" + "lua.ab", optionsDefault, target);
            Debug.Log("build lua.ab successfully");
        }

        outputLuaPath.Delete(true);
    }
    
    private static void EmptyFolder(string dir)
    {

        if (!Directory.Exists(dir))
            return;
        foreach (string d in Directory.GetFileSystemEntries(dir))
        {
            if (File.Exists(d))
            {
                FileInfo fi = new FileInfo(d);
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    fi.Attributes = FileAttributes.Normal;
                File.Delete(d);//直接删除其中的文件  
            }
            else
            {
                DirectoryInfo d1 = new DirectoryInfo(d);
                if (d1.GetFiles().Length != 0)
                {
                    EmptyFolder(d1.FullName);////递归删除子文件夹
                }
                Directory.Delete(d);
            }
        }
    }
    [MenuItem("Assets/[SmartWar] Export to Server")]
    static void ExportToServer()
    {
        string relativePath;
        EmptyFolder(outputPath.FullName);

        MD5 md5 = new MD5CryptoServiceProvider();

        if (!outputPath.Exists)
        {
            outputPath.Create();
        }
		FileStream fs = new FileStream(outputPath.FullName + "/" + MANIFEST_FILE, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
       
        StringBuilder content = new StringBuilder();

        //时间戳作为版本号
        string version = System.DateTime.Now.Ticks.ToString();
        sw.Write(version + "\n");

        byte[] fingerprint = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(version));
        string lineStr;
        byte[] lineByte;
        string hash;

        //服务器端可以一次生成
        foreach (FileInfo fsi in inputAssetPath.GetFiles("*", SearchOption.AllDirectories))
		{
            if (fsi.Extension != META_EXTENSION)
            {
                relativePath = fsi.FullName.Substring(inputAssetPath.FullName.Length + 1);
                hash = GetMD5FromFile(fsi.FullName);
                CompressToServer(fsi);

                lineStr = relativePath + ";" + hash;
                sw.Write(lineStr + "\n");
                lineByte = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(lineStr));
                for (int i = 0; i < fingerprint.Length; ++i)
                {
                    fingerprint[i] ^= lineByte[i];
                }
            }
        }

        sw.Flush();
        sw.Close();
        fs.Close();

        StringBuilder sb = new StringBuilder();
        //sb.AppendLine("lowest version=" + System.DateTime.Today.Ticks);
        sb.AppendLine("lowest version=" + 0);
        sb.AppendLine("current version=" + version);
        sb.AppendLine("manifest hash=" + MD5ToString(fingerprint));

        //TODO:放在其它路径下，对外均可读
		File.WriteAllText(outputPath.FullName + "/" + CONFIG_FILE, sb.ToString());

		Debug.Log ("export to " + outputPath.FullName);
    }

    //优化：单独建一个md5工具类，但不确定两个工程是否都能访问到
    private static string GetMD5FromFile(string filename)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        FileStream file = new FileStream(filename, FileMode.Open);
        byte[] hash = md5.ComputeHash(file);
        file.Close();

        return MD5ToString(hash);
    }

    private static string MD5ToString(byte[] md5)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < md5.Length; i++)
        {
            sb.Append(md5[i].ToString("x2"));
        }
        return sb.ToString();
    }

    #endregion
}
