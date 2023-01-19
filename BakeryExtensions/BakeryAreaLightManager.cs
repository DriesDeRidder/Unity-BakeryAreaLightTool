using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(BakeryAreaLightManager))]
public class BakeryAreaLightManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BakeryAreaLightManager manager = (BakeryAreaLightManager)target;

        if (GUILayout.Button("Get All Area Lights"))
        {
            manager.GetAllAreaLights();
        }
    }
}
#endif

public class BakeryAreaLightManager : MonoBehaviour
{
    /// <summary>
    /// This is a simple script that manages all the area lights you've placed with the tool.
    /// It disables the area light meshes at start so you can't forget to disable them.
    /// It also sets its shadowcastingmode to off so during the baking of the scene, it won't be included in the bake.
    /// also disables self shadow.
    /// </summary>

    public List<BakeryLightMesh> bakeryLightMeshes = new();

    void Start()
    {
        GetAllAreaLights();
        DisableAllAreaLights();
    }

    public void GetAllAreaLights()
    {
        bakeryLightMeshes.Clear();
        bakeryLightMeshes = FindObjectsOfType<BakeryLightMesh>().ToList();

        foreach (BakeryLightMesh lightMesh in bakeryLightMeshes)
        {
            if (lightMesh.GetComponent<MeshRenderer>() == false) continue;

            lightMesh.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lightMesh.selfShadow = false;
        }

    }

    public void DisableAllAreaLights()
    {
        foreach (BakeryLightMesh lightmesh in bakeryLightMeshes)
        {
            lightmesh.gameObject.SetActive(false);
        }
    }
}
