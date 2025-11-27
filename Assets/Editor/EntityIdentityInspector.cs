#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor.Callbacks;

[CustomEditor(typeof(EntityIdentity))]
public class EntityIdentityInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var t = (EntityIdentity)target;

        EditorGUILayout.LabelField("GUID (read-only)", EditorStyles.boldLabel);
        EditorGUILayout.SelectableLabel(string.IsNullOrEmpty(t.GUID) ? "<empty>" : t.GUID, GUILayout.Height(18));

        // Small help text
        EditorGUILayout.HelpBox("GUIDs are assigned only to scene instances and are saved into the scene file. Use the batch menu to assign GUIDs to all entities in the scene.", MessageType.Info);
    }

    [MenuItem("Tools/Entity Identity/Assign & Save GUIDs In Scene")]
    private static void AssignGuidsInScene()
    {
        // Find all EntityIdentity in the active scene (including inactive)
        var all = UnityEngine.Object.FindObjectsOfType<EntityIdentity>(true);
        bool anyAssigned = false;

        foreach (var e in all)
        {
            // Only operate on scene instances (skip prefab assets)
            if (!e.gameObject.scene.IsValid())
                continue;

            if (string.IsNullOrEmpty(e.GUID))
            {
                // Use SerializedObject so the Inspector recognizes the change immediately
                var so = new SerializedObject(e);
                var prop = so.FindProperty("guid");
                prop.stringValue = System.Guid.NewGuid().ToString();
                so.ApplyModifiedProperties();

                EditorUtility.SetDirty(e);
                anyAssigned = true;
            }
        }

        if (anyAssigned)
        {
            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
            // Save immediately to ensure persistence
            EditorSceneManager.SaveScene(scene);
            EditorUtility.DisplayDialog("EntityIdentity", "Assigned GUIDs to scene instances and saved the scene.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("EntityIdentity", "No scene instances without GUIDs were found.", "OK");
        }
    }
}
#endif

