using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#region Enums

enum WallEndPoint
{
    Head,
    Tail
}

enum BuildDirection
{
    Left,
    Right,
    Straight
}

#endregion

[CustomEditor(typeof(WallBuilder))]
public class WallBuilderEditor : Editor
{

    #region Inspector variables
    
    private WallEndPoint m_wallEndPoint;
    private BuildDirection m_buildDirection;
    private bool m_hasDoor;

    #endregion

    #region Properties

    private WallEndPoint _WallEndPoint
    {
        get { return m_wallEndPoint; }
        set
        {
            if (m_wallEndPoint != value)
            {
                m_wallEndPoint = value;
                BuildWallSystem(true);
            }
        }
    }

    private BuildDirection _BuildDirection
    {
        get { return m_buildDirection; }
        set
        {
            if (m_buildDirection != value)
            {
                m_buildDirection = value;
                BuildWallSystem(true);
            }
        }
    }

    private bool _HasDoor
    {
        get { return m_hasDoor; }
        set
        {
            if (m_hasDoor != value)
            {
                m_hasDoor = value;
                BuildWallSystem(true);
            }
        }
    }

    #endregion

    #region Private variables

    private GameObject m_previewWall = null;
    private GameObject m_previewWallHasDoor = null;
    private GameObject m_previewColumn = null;

    // Sometimes, Unity create multiple instances of Editor class.
    // This can lead to some problems e.g. OnEnable function get called several times.
    // But OnInspectorGUI function only be called in an instance.
    private bool m_isEnabledGUI = false;

    private WallBuilder m_wallBuilder;
    private WallBuilderProperties m_wallBuilderProperties;
    
    private const float BLOCK_SPACE = 10.0f;

    #endregion

    #region Enable/Disable

    GameObject InstantiatePreviewModel(GameObject prefab)
    {
        var obj = Instantiate(prefab);

        obj.transform.SetParent(m_wallBuilder.transform.parent, false);
        obj.name = "[preview]" + obj.name;
        SetColorForObject(obj);

        return obj;
    }

    void OnEnableGUI()
    {
        // Get components.
        m_wallBuilder = target as WallBuilder;
        m_wallBuilderProperties = m_wallBuilder.GetComponent<WallBuilderProperties>();

        // Instantiate preview models
        if (!IsPrefab())
        {
            var wallBuilder = (WallBuilder)target;

            m_previewWall = InstantiatePreviewModel(wallBuilder.m_wall);
            m_previewWallHasDoor = InstantiatePreviewModel(wallBuilder.m_wallHasDoor);
            m_previewColumn = InstantiatePreviewModel(wallBuilder.m_column);
            
            BuildWallSystem(true);
        }
        
    }
    
    void OnDisable()
    {
        m_isEnabledGUI = false;

        if (m_previewColumn != null)
        {
            DestroyImmediate(m_previewColumn);
            m_previewColumn = null;
        }
        if (m_previewWall != null)
        {
            DestroyImmediate(m_previewWall);
            m_previewWall = null;
        }
        if (m_previewWallHasDoor != null)
        {
            DestroyImmediate(m_previewWallHasDoor);
            m_previewWallHasDoor = null;
        }
    }

    #endregion

    #region Draw inspector

    public override void OnInspectorGUI()
    {
        // Init stuffs before drawing.
        if (!m_isEnabledGUI)
        {
            OnEnableGUI();
            m_isEnabledGUI = true;
        }
        
        // Draw variables.
        DrawTargetProperties();
        if (!IsPrefab())
        {
            DrawBuildingConfigs();

            // Build button.
            GUILayout.Space(BLOCK_SPACE);
            if (GUILayout.Button("Build"))
            {
                OnBuildButtonClicked();
            }
        }
    }

    void DrawTargetProperties()
    {
        serializedObject.Update();

        var property = serializedObject.GetIterator();
        if (property.NextVisible(true))
        {
            do
            {
                EditorGUILayout.PropertyField(property);
            }
            while (property.NextVisible(false));
        }

        serializedObject.ApplyModifiedProperties();
    }

    void DrawBuildingConfigs()
    {
        GUILayout.Space(BLOCK_SPACE);

        // Choose end point to build.
        _WallEndPoint = (WallEndPoint)EditorGUILayout.EnumPopup("Choose end point", _WallEndPoint);

        // Choose build direction.
        _BuildDirection = (BuildDirection)EditorGUILayout.EnumPopup("Choose direction", _BuildDirection);

        // Has door or not.
        _HasDoor = EditorGUILayout.Toggle("Has door", _HasDoor);
    }

    #endregion
    
    #region Building wall

    void BuildColumn(bool isPreview, Vector3 pos)
    {
        if (isPreview || (m_buildDirection == BuildDirection.Straight))
        {
            return;
        }
        
        var obj = Instantiate(m_wallBuilder.m_column);

        obj.transform.localPosition = pos;
        obj.transform.SetParent(m_wallBuilder.transform.parent, false);
    }

    void BuildPreviewColumn(bool isPreview, Vector3 pos)
    {
        if (!isPreview)
        {
            return;
        }

        if (m_buildDirection == BuildDirection.Straight)
        {
            m_previewColumn.SetActive(false);
        }
        else
        {
            m_previewColumn.SetActive(true);
            m_previewColumn.transform.localPosition = pos;
        }
    }

    void BuildWall(bool isPreview, GameObject model, Vector3 pos, Quaternion rot)
    {
        if (isPreview)
        {
            return;
        }
        
        var obj = Instantiate(model);

        obj.transform.localPosition = pos;
        obj.transform.localRotation = rot;
        obj.transform.SetParent(m_wallBuilder.transform.parent, false);
        obj.name = model.name;
    }

    void BuildPreviewWall(bool isPreview, Vector3 pos, Quaternion rot)
    {
        if (!isPreview)
        {
            return;
        }

        m_previewWall.SetActive(false);
        m_previewWallHasDoor.SetActive(false);

        var wall = (m_hasDoor ? m_previewWallHasDoor : m_previewWall);

        wall.SetActive(true);
        wall.transform.localPosition = pos;
        wall.transform.localRotation = rot;
    }

    void BuildWallSystem(bool isPreview)
    {
        // Calculate end point.
        var endPointPos = m_wallBuilder.transform.localPosition;
        var transDir = ((m_wallEndPoint == WallEndPoint.Head) ? 1 : -1) * m_wallBuilder.transform.right;

        endPointPos += transDir * (m_wallBuilderProperties.m_localSize.x / 2);

        // Build column.
        var colModel = m_wallBuilder.m_column;
        var colModelSz = colModel.GetComponent<WallBuilderProperties>().m_localSize;
        var colPos = endPointPos + transDir * (colModelSz.x / 2.0f);

        colPos.y = colModel.transform.localPosition.y;

        BuildColumn(isPreview, colPos);
        BuildPreviewColumn(isPreview, colPos);

        // Build wall or wall has door?
        var wallModel = (m_hasDoor ? m_wallBuilder.m_wallHasDoor : m_wallBuilder.m_wall);
        var wallModelSz = wallModel.GetComponent<WallBuilderProperties>().m_localSize;
        var wallPos = (m_buildDirection == BuildDirection.Straight) ? endPointPos : colPos;
        var wallRot = m_wallBuilder.transform.rotation;

        switch (m_buildDirection)
        {
            case BuildDirection.Left:
                wallPos -= m_wallBuilder.transform.forward * (wallModelSz.x / 2 + colModelSz.z / 2);
                wallRot *= Quaternion.Euler(0, 90, 0);
                break;
            case BuildDirection.Right:
                wallPos += m_wallBuilder.transform.forward * (wallModelSz.x / 2 + colModelSz.z / 2);
                wallRot *= Quaternion.Euler(0, -90, 0);
                break;
            case BuildDirection.Straight:
                wallPos += transDir * (wallModelSz.x / 2);
                break;
        }

        wallPos.y = wallModel.transform.localPosition.y;

        BuildWall(isPreview, wallModel, wallPos, wallRot);
        BuildPreviewWall(isPreview, wallPos, wallRot);
    }

    #endregion

    #region Misc

    void OnBuildButtonClicked()
    {
        BuildWallSystem(false);
    }

    bool IsPrefab()
    {
        var type = PrefabUtility.GetPrefabType(target);
        return ((type == PrefabType.Prefab) || (type == PrefabType.ModelPrefab));
    }

    void SetColorForObject(GameObject obj)
    {
        var properties = obj.GetComponent<WallBuilderProperties>();
        if (properties != null)
        {
            SetColorForObject(obj, properties.m_previewColor);
        }
    }

    void SetColorForObject(GameObject obj, Color color)
    {
        var renderer = obj.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            var mat = new Material(renderer.sharedMaterial);

            // Standard material.
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color);

            // Non-standard material.
            mat.color = color;

            renderer.sharedMaterial = mat;
        }

        foreach (Transform child in obj.transform)
        {
            SetColorForObject(child.gameObject, color);
        }
    }

    #endregion

}
