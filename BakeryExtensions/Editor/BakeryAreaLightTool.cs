///created by Dries De Ridder aka thatLongDev
///Find updates on my github https://github.com/DriesDeRidder/BakeryAreaLightTool
///enjoy!
///Place this script in the editor folder

using UnityEditor;
using UnityEngine;

public class BakeryAreaLightTool : EditorWindow
{
    [MenuItem("Bakery/Extensions/Arealight tool")]
    private static void OpenWindow()
    {
        EditorWindow setupWindow = GetWindow<BakeryAreaLightTool>("Arealight Tool");
        setupWindow.minSize = new Vector2(420, 420);
        setupWindow.Show();
    }

    private Bounds boundsToMatch;

    private Color pressedButton = Color.blue;

    public BakeryLightMesh currentLightMesh;
    public Transform[] objectsToMatch;
    private BakeryAreaLightManager bakeryLightManager;

    public Color lightColor = Color.white;
    public Color PrevlightColor = Color.white;

    public float intensity = 1.0f;
    public float prevIntensity = 1.0f;

    public float cutOff = 100.0f;
    public float prevCutOff = 100.0f;

    public float areaScale = 0.9f;
    public float previousAreaScale = 0.9f;

    public float xMove = 0.0f;
    private float xMovePrev = 0.0f;

    public float zMove = 0.0f;
    private float zMovePrev = 0.0f;

    public float yMove = 0.0f;
    private float yMovePrev = 0.0f;


    private Vector3 LightMeshStartPos = new();

    public enum ScaleDirection
    {
        XZ,
        ZX
    }

    public ScaleDirection scaleDirection = ScaleDirection.XZ;

    private void OnGUI()
    {
        if (CanShowObjects())
        {
            currentLightMesh = (BakeryLightMesh)EditorGUILayout.ObjectField("Current Light Mesh", currentLightMesh, typeof(BakeryLightMesh), true);
        }
        else
        {
            EditorGUILayout.HelpBox("Select one or more object that act as a window. Then press the button below to create an " +
                "area light that fits this window. Then play with the settings to fit your needs.", MessageType.Info);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Light Settings", EditorStyles.boldLabel);

        #region light color
        lightColor = EditorGUILayout.ColorField("Light Color", lightColor);

        if (lightColor != PrevlightColor)
        {
            SetLightMeshColor();
        }
        PrevlightColor = lightColor;

        #endregion

        #region light intensity

        intensity = EditorGUILayout.FloatField("Intensity", intensity);

        if (intensity != prevIntensity)
        {
            SetLightIntensity();
        }
        prevIntensity = intensity;

        #endregion

        #region cutoff distance

        cutOff = EditorGUILayout.FloatField("cutoff ", cutOff);

        if (cutOff != prevCutOff)
        {
            SetLightCutoffDistance();
        }
        prevCutOff = cutOff;

        #endregion

        EditorGUILayout.Space();


        if (CanShowObjects())
        {
            #region scale
            EditorGUILayout.LabelField("Scale", EditorStyles.boldLabel, GUILayout.Width(150));

            areaScale = EditorGUILayout.Slider("areaScale", areaScale, 0.6f, 1.2f);

            if (areaScale != previousAreaScale)
            {
                ScaleLightMesh();
            }
            previousAreaScale = areaScale;

            #endregion

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Move direction", EditorStyles.boldLabel, GUILayout.Width(150));

            #region position
            xMove = EditorGUILayout.Slider("x Move", xMove, -0.5f, 0.5f);
            if (xMove != xMovePrev)
            {
                MoveX();
            }
            xMovePrev = xMove;

            yMove = EditorGUILayout.Slider("y Move", yMove, -0.5f, 0.5f);
            if (yMove != yMovePrev)
            {
                MoveY();
            }
            yMovePrev = yMove;

            zMove = EditorGUILayout.Slider("z Move", zMove, -0.5f, 0.5f);

            if (zMove != zMovePrev)
            {
                MoveZ();
            }
            zMovePrev = zMove;

            #endregion

            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();

            #region scale direction
            EditorGUILayout.LabelField("Scale Direction", EditorStyles.boldLabel, GUILayout.Width(150));

            var defaultColor = GUI.color;

            if (scaleDirection == ScaleDirection.XZ)
            {
                GUI.color = pressedButton;
            }
            else
            {
                GUI.color = defaultColor;

            }
            if (GUILayout.Button("XZ"))
            {
                scaleDirection = ScaleDirection.XZ;
                ScaleLightMesh();
            }

            if (scaleDirection == ScaleDirection.ZX)
            {
                GUI.color = pressedButton;
            }
            else
            {
                GUI.color = defaultColor;
            }


            if (GUILayout.Button("ZX"))
            {
                scaleDirection = ScaleDirection.ZX;

                ScaleLightMesh();
            }
            GUI.color = defaultColor;
            #endregion

            EditorGUILayout.EndHorizontal();

            #region orientation buttons
            EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel, GUILayout.Width(150));

            if (GUILayout.Button("Flip Bakery Mesh", GUILayout.Height(30)))
            {
                FlipBakeryMesh();
            }

            if (GUILayout.Button("Rotate 90 degrees", GUILayout.Height(30)))
            {
                Rotate90Degrees();
            }
            #endregion
        }

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Create new", EditorStyles.boldLabel, GUILayout.Width(150));

        if (GUILayout.Button("Create New Bakery Mesh Light On Selection", GUILayout.Height(30)))
        {
            if (bakeryLightManager == null)
            {
                bakeryLightManager = FindObjectOfType<BakeryAreaLightManager>();
                if (bakeryLightManager == null)
                {
                    var lightManager = new GameObject("BakeryAreaLightManager");
                    bakeryLightManager = lightManager.AddComponent<BakeryAreaLightManager>();
                    bakeryLightManager.GetAllAreaLights();
                }
            }

            if (Selection.activeGameObject == null)
            {
                Debug.LogError("Please select a gameobject in the scene first");
                return;
            }

            if (Selection.transforms.Length > 0)
            {
                objectsToMatch = Selection.transforms;
            }

            if (objectsToMatch[0].GetComponent<MeshRenderer>() == false)
            {
                Debug.LogError("No meshrenderer on current object found");
                return;
            }

            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Undo.RegisterCreatedObjectUndo(go, "Create Bakery light");

            currentLightMesh = go.AddComponent<BakeryLightMesh>();

            go.name = "AreaLight";
            Undo.RegisterChildrenOrderUndo(bakeryLightManager, "Create Bakery light");
            go.transform.parent = bakeryLightManager.transform;

            bakeryLightManager.GetAllAreaLights();

            SetLightIntensity();
            SetLightMeshColor();

            var ecam = SceneView.lastActiveSceneView.camera.transform;
            go.transform.position = ecam.position + ecam.forward;
            var bakeryRuntimePath = ftLightmaps.GetRuntimePath();
            var mat = AssetDatabase.LoadAssetAtPath(bakeryRuntimePath + "ftDefaultAreaLightMat.mat", typeof(Material)) as Material;
            go.GetComponent<MeshRenderer>().material = mat;
            go.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            currentLightMesh.selfShadow = false;
            var arr = new GameObject[1];
            arr[0] = go;
            Selection.objects = arr;

            //extra
            areaScale = 0.9f;
            MatchLightToObjectSize();

            LightMeshStartPos = boundsToMatch.center;
            currentLightMesh.transform.SetPositionAndRotation(LightMeshStartPos, Quaternion.Inverse(objectsToMatch[0].rotation));
            ResetXYZMove();
        }
    }

    public void MatchLightToObjectSize()
    {
        if (currentLightMesh == null)
        {

            Debug.LogError("No area light selected");
            return;
        }

        if (objectsToMatch.Length == 0)
        {
            Debug.LogError("No active gameobject selected");
            return;
        }

        if (objectsToMatch[0].GetComponent<MeshRenderer>() == false)
        {
            Debug.LogError("No meshrenderer on current object found");
            return;
        }

        Undo.RecordObject(currentLightMesh.gameObject, "Changed light mesh");

        boundsToMatch = objectsToMatch[0].GetComponent<MeshRenderer>().bounds;
        foreach (var obj in objectsToMatch)
        {
            boundsToMatch.Encapsulate(obj.GetComponent<MeshRenderer>().bounds);
        }

        ScaleLightMesh();
    }



    public void ScaleLightMesh()
    {
        if (currentLightMesh == null)
        {
            Debug.LogError("No area light selected");
            return;
        }

        Vector3 scaleDir = boundsToMatch.size;
        Vector3 actualScale = scaleDir;

        switch (scaleDirection)
        {
            case ScaleDirection.XZ:
                actualScale = new Vector3(scaleDir.x, scaleDir.y, scaleDir.z);
                break;
            case ScaleDirection.ZX:
                actualScale = new Vector3(scaleDir.z, scaleDir.y, scaleDir.x);

                break;
        }
        currentLightMesh.gameObject.transform.localScale = actualScale * areaScale;
    }

    public void MoveX()
    {
        if (currentLightMesh == null) return;
        zMove = 0;
        yMove = 0;
        currentLightMesh.transform.localPosition = new(LightMeshStartPos.x + xMove, LightMeshStartPos.y, LightMeshStartPos.z);
    }

    public void MoveZ()
    {
        if (currentLightMesh == null) return;

        xMove = 0;
        yMove = 0;
        currentLightMesh.transform.localPosition = new(LightMeshStartPos.x, LightMeshStartPos.y, LightMeshStartPos.z + zMove);
    }

    public void MoveY()
    {
        if (currentLightMesh == null) return;

        xMove = 0;
        zMove = 0;
        currentLightMesh.transform.localPosition = new(LightMeshStartPos.x, LightMeshStartPos.y + yMove, LightMeshStartPos.z);
    }

    public bool CanShowButtons()
    {
        return Selection.activeGameObject = null;
    }

    public bool CanShowObjects()
    {
        return currentLightMesh != null && objectsToMatch.Length > 0;
    }

    public void ResetXYZMove()
    {
        xMove = 0;
        yMove = 0;
        zMove = 0;
    }

    private MaterialPropertyBlock propertyBlock;
    public void SetLightMeshColor()
    {
        if (currentLightMesh == null) return;
        currentLightMesh.color = lightColor;

        propertyBlock = new MaterialPropertyBlock();

        currentLightMesh.GetComponent<MeshRenderer>().GetPropertyBlock(propertyBlock);

        propertyBlock.SetColor("_Color", lightColor);

        currentLightMesh.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }

    public void SetLightIntensity()
    {
        if (currentLightMesh == null) return;
        currentLightMesh.intensity = intensity;
    }

    public void SetLightCutoffDistance()
    {
        if (currentLightMesh == null) return;
        currentLightMesh.cutoff = cutOff;
    }


    public void FlipBakeryMesh()
    {
        if (currentLightMesh == null) return;
        currentLightMesh.transform.Rotate(0, 180, 0);
    }


    public void Rotate90Degrees()
    {
        if (currentLightMesh == null) return;
        currentLightMesh.transform.Rotate(0, 90, 0);
    }
}