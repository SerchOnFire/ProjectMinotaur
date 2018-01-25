using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;
using System;
using System.Collections;


public class SceneEditor : EditorWindow
{
    Dictionary<string, GameObject[]> levelEditorData = new Dictionary<string, GameObject[]>();

    public static float globalRows = 1;
    public static float globalColumns = 1;
    public static float globalBoxWidth = 50;

    string titleStyle = "Box";
    LevelData level;

    Rect bigBoxSize = new Rect(0, 0, 40, 40);
    Rect smallButtonA = new Rect(0, 0, 23, 23);
    Rect smallButtonB = new Rect(0, 0, 46, 23);
    Rect mediumButtonA = new Rect(0, 0, 63, 23);
    Rect mediumButtonB = new Rect(0, 0, 90, 23);
    Rect largeButton = new Rect(0, 0, 130, 23);

    delegate void WindowDelegate();
    WindowDelegate windowDelegate;

    GUIStyle style = new GUIStyle();

    public enum CameraView
    { Front, TopDown, Isometric, Trimetric }

    public enum Assets
    { None, Platforms, Decoration, Gameplay, Bases }
    Assets currentType = Assets.None;

    public enum StyleAssets
    { None, Cave, Forest, Ruins }

    public enum EditorIcons
    { None, Mirror, Rotate, Delete }

    public enum EditorTools
    { None, Add, Update, Mirror, Rotate, Erase, ScaleDown, ScaleToOne, ScaleUp, MoveTo}
    EditorTools currentTool = EditorTools.None;

    static EditorWindow sceneEditorWindow;
    bool initialized = false;

    public Texture addTexture;
    public Texture mirrorTexture;
    public Texture eraseTexture;
    public Texture rotateTexture;
    public Texture scaleDownTexture;
    public Texture scaleUpTexture;
    public Texture scaleOneTexture;

    public Texture[] arrowTexture = new Texture[9];

    Vector2 scrollPosAssets = new Vector2();

    [MenuItem("Custom Tools/Editor/Level Editor")]
    public static void ShowWindow()
    {
        System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
        System.Type type = assembly.GetType("UnityEditor.GameView");
        sceneEditorWindow = EditorWindow.GetWindow<SceneEditor>("Level Editor", true, type);
        sceneEditorWindow.maxSize = new Vector3(500, 200);
        sceneEditorWindow.Show();
    }

    void Initialize()
    {
        Debug.Log(this + "-Initialize");
        initialized = false;
        SceneView.onSceneGUIDelegate += SceneInteraction;

        InitializeData();
        initialized = true;
    }

    void InitializeData()
    {
        LoadLevelEditorResources();
        LoadEditorIcons();
        Debug.Log("InitializeData");
    }

    void OnGUI()
    {
        if (!initialized)
        {
            Initialize();
            SetSceneView(CameraView.Front);
        }
        else
        {
            BuildEditorContent();
            Repaint();
        }
    }

    void BuildEditorContent()
    {
        Rect tempRect = new Rect();
        float windowWidth = Screen.width / 2;
        style.normal.textColor = Color.white;
        /*--------------"First Column"-------------*/
        tempRect = new Rect(0, 0, Screen.width * 1 / 6, Screen.height);
        windowDelegate = DrawFirstColumn;
        BuildWindow(windowDelegate, windowWidth, "", tempRect, Vector2.zero);

        /*-------------"Second Column"-------------*/
        tempRect = new Rect(Screen.width * 1 / 6, 0, Screen.width * 1 / 6, Screen.height);
        windowDelegate = DrawSecondColumn;
        BuildWindow(windowDelegate, windowWidth, "", tempRect, Vector2.zero);

        /*-------------"Third Column"-------------*/
        tempRect = new Rect(Screen.width * 2 / 6, 0, Screen.width * 3 / 6, Screen.height);
        windowDelegate = DrawThirdColumn;
        BuildWindow(windowDelegate, windowWidth, "", tempRect, Vector2.zero);

        /*--------------"Last Column"-------------*/
        tempRect = new Rect(Screen.width * 5 / 6, 0, Screen.width * 1 / 6, Screen.height);
        windowDelegate = DrawLastColumn;
        BuildWindow(windowDelegate, windowWidth, "", tempRect, Vector2.zero);
    }

    Vector2 BuildWindow(WindowDelegate tempDelegate, float windowWidth, string name, Rect rect, Vector2 scrollPos)
    {
        //GUILayout.BeginArea(rect, ("  " + name), style);
        GUILayout.BeginArea(rect);
        {
            EditorGUILayout.BeginVertical();
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                {
                    tempDelegate();
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }
        GUILayout.EndArea();
        return scrollPos;
    }

    void DrawFirstColumn()
    {
        DrawLayersBox();
        DrawEraseSection();
    }

    void DrawSecondColumn()
    {
        DrawToolsSection();
        DrawMoveToSection();
    }

    void DrawThirdColumn()
    {
        EditorGUILayout.BeginHorizontal(titleStyle);
        {
            if (GUILayout.Button(Assets.Platforms.ToString(), GUILayout.Width(mediumButtonB.width), GUILayout.Height(mediumButtonB.height)))
                LoadPreviews(Assets.Platforms);

            if (GUILayout.Button(Assets.Decoration.ToString(), GUILayout.Width(mediumButtonB.width), GUILayout.Height(mediumButtonB.height)))
                LoadPreviews(Assets.Decoration);

            if (GUILayout.Button(Assets.Gameplay.ToString(), GUILayout.Width(mediumButtonB.width), GUILayout.Height(mediumButtonB.height)))
                LoadPreviews(Assets.Gameplay);

            if (GUILayout.Button(Assets.Bases.ToString(), GUILayout.Width(mediumButtonB.width), GUILayout.Height(mediumButtonB.height)))
                LoadPreviews(Assets.Bases);
        }
        EditorGUILayout.EndHorizontal();

        scrollPosAssets = EditorGUILayout.BeginScrollView(scrollPosAssets);
        {
            EditorGUILayout.BeginVertical();
            {
                if (assetsCount > 0)
                {
                    for (int i = 0; i < assetsCount / 6 + 1; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            for (int j = 0; j < 6; j++)
                            {
                                if (j + (6 * i) < assetsCount)
                                {
                                    if (GUILayout.Button(assetTextures[(j + (6 * i))], GUILayout.Width(mediumButtonA.width), GUILayout.Height(mediumButtonA.width)))
                                    {
                                        Debug.Log("GameObject: " + levelEditorData[currentType.ToString()][(j + (6 * i))].name);
                                    }
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();
    }


    //List<GameObject> prefabs = new List<GameObject>();
    List<Texture> assetTextures = new List<Texture>();
    int assetsCount = 0;

    void LoadPreviews(Assets type)
    {
        currentType = type;
        assetTextures.Clear();
        //prefabs.Clear();

        assetsCount = (levelEditorData[type.ToString()]).Length;

        for (int i = 0, iEnd = assetsCount; i < iEnd; i++)
        {
            assetTextures.Add(AssetPreview.GetAssetPreview(levelEditorData[type.ToString()][i]));
            //prefabs.Add(levelEditorData[type.ToString()][i]);
        }
        //Debug.Log("assetTextures, count" + assetsCount);
    }

    void LoadIcons()
    {

    }

    void DrawLastColumn()
    {
        EditorGUILayout.BeginHorizontal(titleStyle);
        if (GUILayout.Button("File", GUILayout.Width(smallButtonB.width), GUILayout.Height(smallButtonB.height)))
        {
            isFileWindowSelected = true;
            isSceneWindowsSelected = false;
        }

        if (GUILayout.Button("Scene", GUILayout.Width(smallButtonB.width), GUILayout.Height(smallButtonB.height)))
        {
            isFileWindowSelected = false;
            isSceneWindowsSelected = true;
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        if (isFileWindowSelected)
        {
            DrawFileButtons();
        }
        else
        {
            DrawGridSettings();
            DrawCameraSceneView();
        }
    }


    void DrawToolsSection()
    {
        EditorGUILayout.BeginHorizontal("Box");
        {
            GUILayout.Label("Tools: ", EditorStyles.boldLabel);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Update", GUILayout.Width(largeButton.width), GUILayout.Height(largeButton.height)))
            {  }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button(mirrorTexture, GUILayout.Width(bigBoxSize.width), GUILayout.Height(bigBoxSize.height)))
            {
                Debug.Log("mirror");
                currentTool = EditorTools.Mirror;
            }
            if (GUILayout.Button(addTexture, GUILayout.Width(bigBoxSize.width), GUILayout.Height(bigBoxSize.height)))
            {
                Debug.Log("add");
                currentTool = EditorTools.Add;
            }
            if (GUILayout.Button(eraseTexture, GUILayout.Width(bigBoxSize.width), GUILayout.Height(bigBoxSize.height)))
            {
                Debug.Log("erase");
                currentTool = EditorTools.Erase;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button(scaleDownTexture, GUILayout.Width(bigBoxSize.width), GUILayout.Height(bigBoxSize.height)))
            {
                Debug.Log("scale down");
                currentTool = EditorTools.ScaleDown;
            }
            if (GUILayout.Button(scaleOneTexture, GUILayout.Width(bigBoxSize.width), GUILayout.Height(bigBoxSize.height)))
            {
                Debug.Log("scale to 1");
                currentTool = EditorTools.ScaleToOne;
            }
            if (GUILayout.Button(scaleUpTexture, GUILayout.Width(bigBoxSize.width), GUILayout.Height(bigBoxSize.height)))
            {
                Debug.Log("scale up");
                currentTool = EditorTools.ScaleUp;
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void DrawEraseSection()
    {
        EditorGUILayout.BeginHorizontal(titleStyle);
        {
            GUILayout.Label("Erase: ", EditorStyles.boldLabel);
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button(Assets.Platforms.ToString(), GUILayout.Width(mediumButtonB.width), GUILayout.Height(mediumButtonB.height)))
            EraseObjects(Assets.Platforms);

        if (GUILayout.Button(Assets.Gameplay.ToString(), GUILayout.Width(mediumButtonB.width), GUILayout.Height(mediumButtonB.height)))
            EraseObjects(Assets.Gameplay);

        if (GUILayout.Button(Assets.Decoration.ToString(), GUILayout.Width(mediumButtonB.width), GUILayout.Height(mediumButtonB.height)))
            EraseObjects(Assets.Decoration);

        if (GUILayout.Button(Assets.Bases.ToString(), GUILayout.Width(mediumButtonB.width), GUILayout.Height(mediumButtonB.height)))
            EraseObjects(Assets.Bases);
    }

    void EraseObjects(Assets asset)
    {

    }

    float memGlobalRows;
    float memGlobalColumns;

    void DrawGridSettings()
    {
        EditorGUILayout.BeginHorizontal(titleStyle);
        {
            GUILayout.Label("GridSettings: ", EditorStyles.boldLabel);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Rows:", GUILayout.Width(smallButtonB.width));
            GUILayout.Label("" + globalRows, GUILayout.Width(smallButtonA.width));
            globalRows = (int)GUILayout.HorizontalSlider(globalRows, 1, 10);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Cols:", GUILayout.Width(smallButtonB.width));
            GUILayout.Label("" + globalColumns, GUILayout.Width(smallButtonA.width));
            globalColumns = (int)GUILayout.HorizontalSlider(globalColumns, 1, 10);
        }
        EditorGUILayout.EndHorizontal();

        if (globalRows != memGlobalRows || globalColumns != memGlobalColumns)
        {
            RescaleEditorBase();
            SceneView.RepaintAll();
        }

        memGlobalRows = globalRows;
        memGlobalColumns = globalColumns;
    }

    void RescaleEditorBase()
    {
        GameObject.Find("EditorBase").GetComponent<BoxCollider>().size = new Vector3((globalColumns + 2) * globalBoxWidth, 1, (globalRows + 2 )* globalBoxWidth);
    }

    bool isLayerPlatformsActive;
    bool isLayerDecorationActive;
    bool isLayerBasesActive;
    bool isLayerGameplayAssetsActive;

    bool isLayerPlatformsVisible;
    bool isLayerDecorationVisible;
    bool isLayerBasesVisible;
    bool isLayerGameplayAssetsVisible;

    void DrawLayersBox()    
    {
        EditorGUILayout.BeginHorizontal(titleStyle);
        {
            GUILayout.Label("Layers: ", EditorStyles.boldLabel, GUILayout.Width(mediumButtonB.width));
            GUILayout.Label("A", EditorStyles.boldLabel);
            GUILayout.Label("V", EditorStyles.boldLabel);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Platforms", GUILayout.Width(mediumButtonB.width));
            isLayerPlatformsActive = GUILayout.Toggle(isLayerPlatformsActive, "");
            isLayerPlatformsVisible = GUILayout.Toggle(isLayerPlatformsVisible, "");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Decoration", GUILayout.Width(mediumButtonB.width));
            isLayerDecorationActive = GUILayout.Toggle(isLayerDecorationActive, "");
            isLayerDecorationVisible = GUILayout.Toggle(isLayerDecorationVisible, "");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Bases", GUILayout.Width(mediumButtonB.width));
            isLayerBasesActive = GUILayout.Toggle(isLayerBasesActive, "");
            isLayerBasesVisible = GUILayout.Toggle(isLayerBasesVisible, "");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Gameplay", GUILayout.Width(mediumButtonB.width));
            isLayerGameplayAssetsActive = GUILayout.Toggle(isLayerGameplayAssetsActive, "");
            isLayerGameplayAssetsVisible = GUILayout.Toggle(isLayerGameplayAssetsVisible, "");
        }
        EditorGUILayout.EndHorizontal();
    }

    void DrawMoveToSection()
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button(arrowTexture[0], GUILayout.Width(bigBoxSize.width), GUILayout.Height(bigBoxSize.height)))
                MovePlatform("01");

            if (GUILayout.Button(arrowTexture[1], GUILayout.Width(bigBoxSize.width), GUILayout.Height(bigBoxSize.height)))
                MovePlatform("02");

            if (GUILayout.Button(arrowTexture[2], GUILayout.Width(bigBoxSize.width), GUILayout.Height(bigBoxSize.height)))
                MovePlatform("03");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button(arrowTexture[3], GUILayout.Width(bigBoxSize.width), GUILayout.Height(bigBoxSize.height)))
                MovePlatform("04");

            if (GUILayout.Button(arrowTexture[4], GUILayout.Width(bigBoxSize.width), GUILayout.Height(bigBoxSize.height)))
                RotatePlatform();

            if (GUILayout.Button(arrowTexture[5], GUILayout.Width(bigBoxSize.width), GUILayout.Height(bigBoxSize.height)))
                MovePlatform("06");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button(arrowTexture[6], GUILayout.Width(bigBoxSize.width), GUILayout.Height(bigBoxSize.height)))
                MovePlatform("07");

            if (GUILayout.Button(arrowTexture[7], GUILayout.Width(bigBoxSize.width), GUILayout.Height(bigBoxSize.height)))
                MovePlatform("08");

            if (GUILayout.Button(arrowTexture[8], GUILayout.Width(bigBoxSize.width), GUILayout.Height(bigBoxSize.height)))
                MovePlatform("09");
        }
        EditorGUILayout.EndHorizontal();
    }



    void MovePlatform(string movement)
    {
        switch (movement) /*---X Movement---*/
        {
            case "01": case "04": case "07": /*---Left---*/
                break;
            case "03": case "06": case "09": /*---Right---*/
                break;
            default:
                break;
        }
        switch (movement) /*---Z Movement---*/
        {
            case "01": case "02": case "03": /*---Backward---*/
                break;
            case "07": case "08": case "09": /*---Forward---*/
                break;
            default:
                break;
        }
    }

    void RotatePlatform()
    {

    }

    bool isFileWindowSelected = true;
    bool isSceneWindowsSelected = false;

    void DrawFileButtons()
    {
        if (GUILayout.Button("Save", GUILayout.Width(mediumButtonB.width), GUILayout.Height(mediumButtonB.height)))
            SaveAsset(level);

        if (GUILayout.Button("Load", GUILayout.Width(mediumButtonB.width), GUILayout.Height(mediumButtonB.height)))
            LoadAsset(level);
    }


    float sceneViewSize = 200;
    float memSceneViewSize;

    void DrawCameraSceneView()
    {
        float inferiorLimit = 10;
        float superiorLimit = 500;

        if (memSceneViewSize != sceneViewSize)
            SetSceneView(sceneViewSize);
        
        memSceneViewSize = sceneViewSize;

        EditorGUILayout.BeginHorizontal(titleStyle);
        {
            GUILayout.Label("Scene View Camera: ", EditorStyles.boldLabel);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Size:", GUILayout.Width(smallButtonB.width - 10));
            GUILayout.Label("" + sceneViewSize, GUILayout.Width(smallButtonA.width + 10));
            sceneViewSize = (int)GUILayout.HorizontalSlider(sceneViewSize, inferiorLimit, superiorLimit);
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button(CameraView.TopDown.ToString(), GUILayout.Width(mediumButtonB.width), GUILayout.Height(mediumButtonB.height)))
            SetSceneView(CameraView.TopDown);

        if (GUILayout.Button(CameraView.Isometric.ToString(), GUILayout.Width(mediumButtonB.width), GUILayout.Height(mediumButtonB.height)))
            SetSceneView(CameraView.Isometric);

        if (GUILayout.Button(CameraView.Trimetric.ToString(), GUILayout.Width(mediumButtonB.width), GUILayout.Height(mediumButtonB.height)))
            SetSceneView(CameraView.Trimetric);

        if (GUILayout.Button(CameraView.Front.ToString(), GUILayout.Width(mediumButtonB.width), GUILayout.Height(mediumButtonB.height)))
            SetSceneView(CameraView.Front);
    }

    void SetSceneView(CameraView cameraView)
    {
        switch (cameraView)
        {
            case CameraView.Front:
                SetSceneView(Quaternion.Euler(0f, 0f, 0f), sceneViewSize);
                break;
            case CameraView.TopDown:
                SetSceneView(Quaternion.Euler(89f, 0f, 0f), sceneViewSize);
                break;
            case CameraView.Isometric:
                SetSceneView(Quaternion.Euler(45, 45, 0f), sceneViewSize);
                break;
            case CameraView.Trimetric:
                SetSceneView(Quaternion.Euler(27.5f, 45, 0f), sceneViewSize);
                break;
        }
    }

    void SetSceneView(Quaternion quaternion, float size)
    {
        Selection.activeObject = GameObject.Find("Composition");
        SceneView.FrameLastActiveSceneView();
        SceneView.lastActiveSceneView.orthographic = true;
        SceneView.lastActiveSceneView.rotation = quaternion;
        SceneView.lastActiveSceneView.size = size;
        Selection.activeObject = null;
    }

    void SetSceneView(float size)
    {
        Selection.activeObject = GameObject.Find("Composition");
        SceneView.FrameLastActiveSceneView();
        SceneView.lastActiveSceneView.orthographic = true;
        SceneView.lastActiveSceneView.size = size;
        Selection.activeObject = null;
    }


    void LoadAsset(LevelData level)
    {

    }

    void SaveAsset(LevelData level)
    {

        EditorUtility.SetDirty(level);
    }

    void OnDestroy()
    {
        sceneEditorWindow = null;
        initialized = false;
        SceneView.onSceneGUIDelegate -= SceneInteraction;
    }

    private void SceneInteraction(SceneView sceneView)
    {
  		Event currentEvent = Event.current;
		if (currentEvent != null) 
		{
			if (currentEvent.type == EventType.mouseDown && Event.current.button == 0) 
			{
                Debug.Log("Click");

                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    InteractionResult(hit);
                }
                else
                {
                    //if (currenTool != EditorTools.None)
                    //ClearAllVars();
                }
            }
		}

        /*if (currenTool != EditorTools.None)
        {
            Selection.activeGameObject = null;
        }*/


    }

    private void InteractionResult(RaycastHit hit)
    { }

    void SaveSection()
    {
            GameObject map = GameObject.Find("Map").gameObject;

            LevelData levelData = new LevelData();
            //levelData.baseData

           /* JsonOperator jsonOperator = new JsonOperator();
            jsonOperator.SaveData(section, type.ToString(), name);

            curSectionString = "Sections/" + type.ToString() + "/" + name;
            ShowLoadWindow = false;
            ReleaseMemory();
            ClearAllVars();
            type = SectionType.SelectFolder;*/
    }


    void LoadLevelEditorResources()
    {
        Debug.Log("LoadLevelEditorResources");
        levelEditorData.Clear();

        GameObject[] tempArray = Resources.LoadAll("Bases").Cast<GameObject>().ToArray();
        levelEditorData.Add(Assets.Bases.ToString(), tempArray);
        tempArray = Resources.LoadAll("Platforms").Cast<GameObject>().ToArray();
        levelEditorData.Add(Assets.Platforms.ToString(), tempArray);
        tempArray = Resources.LoadAll("Gameplay").Cast<GameObject>().ToArray();
        levelEditorData.Add(Assets.Gameplay.ToString(), tempArray);
        tempArray = Resources.LoadAll("Decoration").Cast<GameObject>().ToArray();
        levelEditorData.Add(Assets.Decoration.ToString(), tempArray);

        Debug.Log("Keys: " + levelEditorData.Keys.Count);

    }

    void LoadEditorIcons()
    {
        addTexture = Resources.Load("Icons/add") as Texture;
        mirrorTexture = Resources.Load("Icons/mirror") as Texture;
        eraseTexture = Resources.Load("Icons/erase") as Texture;
        rotateTexture = Resources.Load("Icons/rotate") as Texture;
        scaleDownTexture = Resources.Load("Icons/scaleDown") as Texture;
        scaleUpTexture = Resources.Load("Icons/scaleUp") as Texture;
        scaleOneTexture = Resources.Load("Icons/scaleOne") as Texture;

        for (int i = 0; i < 9; i++)
        {
            if (i != 4)
            {
                arrowTexture[i] = Resources.Load("Icons/drag0" + (i + 1)) as Texture;
            }
            else
            {
                arrowTexture[i] = Resources.Load("Icons/rotate") as Texture;
            }
        }

    }
    [DrawGizmo(GizmoType.NonSelected | GizmoType.NonSelected)]
    private static void DrawGrids(Transform gridT, GizmoType gType)
    {
        if (EditorSceneManager.GetActiveScene().name == "LevelEditor")
        {
            float gridLimitW = globalColumns * globalBoxWidth;
            float gridLimitH = globalRows * globalBoxWidth;

            float boxW = gridLimitW / 2 + 50;
            float boxH = gridLimitH / 2 + 50;

            Vector3 vHeight = new Vector3(-gridLimitW / 2, 0, -gridLimitH / 2);
            Vector3 vWidth = new Vector3(-gridLimitW / 2, 0, -gridLimitH / 2);
            Vector3 vBoxA = new Vector3(-boxW / 2, 0, -boxH / 2);
            Vector3 vBoxB = new Vector3(-boxW / 2, 0, -boxH / 2);

            Gizmos.color = Color.cyan;
            for (int i = 0; i <= 1; i++)
            {
                Gizmos.DrawLine(new Vector3(boxW, 0, boxH), new Vector3(-boxW, 0, boxH));
                Gizmos.DrawLine(new Vector3(boxW, 0, -boxH), new Vector3(-boxW, 0, -boxH));
                Gizmos.DrawLine(new Vector3(boxW, 0, boxH), new Vector3(boxW, 0, -boxH));
                Gizmos.DrawLine(new Vector3(-boxW, 0, boxH), new Vector3(-boxW, 0, -boxH));
            }

            Gizmos.color = Color.blue;
            for (int i = 0; i <= globalColumns; i++)
            {
                Gizmos.DrawLine(vWidth, vWidth + Vector3.forward * gridLimitH);
                vWidth.x += gridLimitW / globalColumns;
            }

            for (int i = 0; i <= globalRows; i++)
            {
                Gizmos.DrawLine(vHeight, vHeight + Vector3.right * gridLimitW);
                vHeight.z += gridLimitH / globalRows;
            }

        }

    }

}