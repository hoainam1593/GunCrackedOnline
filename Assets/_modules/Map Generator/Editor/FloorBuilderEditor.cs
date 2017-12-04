using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FloorBuilder))]
public class FloorBuilderEditor : Editor
{
    private SerializedProperty m_floorTile;
    private SerializedProperty m_floorTileSize;
    private SerializedProperty m_tilesCountWidth;
    private SerializedProperty m_tilesCountHeight;

    void OnEnable()
    {
        m_floorTile = serializedObject.FindProperty("m_floorTile");
        m_floorTileSize = serializedObject.FindProperty("m_floorTileSize");
        m_tilesCountWidth = serializedObject.FindProperty("m_tilesCountWidth");
        m_tilesCountHeight = serializedObject.FindProperty("m_tilesCountHeight");
    }

    public override void OnInspectorGUI()
    {
        // Draw script.
        EditorGUILayout.ObjectField(
            "Script",
            MonoScript.FromMonoBehaviour((FloorBuilder)target),
            typeof(FloorBuilder),
            false);

        // Draw variables.
        DrawTargetProperties();

        // Build button.
        if (GUILayout.Button("Build"))
        {
            OnBuildButtonClicked();
        }
    }

    void DrawTargetProperties()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_floorTile);
        EditorGUILayout.PropertyField(m_floorTileSize);
        EditorGUILayout.PropertyField(m_tilesCountWidth);
        EditorGUILayout.PropertyField(m_tilesCountHeight);

        serializedObject.ApplyModifiedProperties();
    }

    void OnBuildButtonClicked()
    {
        var floorBuilder = target as FloorBuilder;

        // Delete existed tiles.
        var sz = floorBuilder.transform.childCount;
        for (int i = sz - 1; i >= 0; i--)
        {
            DestroyImmediate(floorBuilder.transform.GetChild(i).gameObject);
        }

        // Build tiles.
        var floorWidth = floorBuilder.m_tilesCountWidth * floorBuilder.m_floorTileSize.x;
        var floorHeight = floorBuilder.m_tilesCountHeight * floorBuilder.m_floorTileSize.z;
        var pos = new Vector3(-floorWidth / 2, 0, -floorHeight / 2);
        
        for (int iw = 0; iw < floorBuilder.m_tilesCountWidth; iw++)
        {
            for (int ih = 0; ih < floorBuilder.m_tilesCountHeight; ih++)
            {
                var obj = Instantiate(floorBuilder.m_floorTile);
                obj.transform.localPosition = pos;
                obj.transform.SetParent(floorBuilder.transform, false);
                
                pos.z += floorBuilder.m_floorTileSize.z;
            }

            pos.x += floorBuilder.m_floorTileSize.x;
            pos.z = -floorHeight / 2;
        }
    }
}
