using UnityEditor;
using UnityEngine;

public class BatchReplace : EditorWindow
{
    private GameObject[] selectedGameObjects = null;
    public GameObject prefab = null;

    [MenuItem("Tools/BatchReplacer")]
    public static void ShowWindow()
    {
        GetWindow<BatchReplace>("BatchReplacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Replacer Settings", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab to replace with", prefab, typeof(GameObject), false);

        if (GUILayout.Button("Replace GameObjects"))
        {
            ReplaceGameObjects();
        }
    }

    private void ReplaceGameObjects()
    {
        selectedGameObjects = Selection.gameObjects;

        if (selectedGameObjects.Length <= 0)
        {
            Debug.LogWarning("No GameObjects selected. Please, select at least one GameObject to replace.");
            return;
        }

        if (prefab == null)
        {
            Debug.LogWarning("No GameObject to replace with. Please, select a GameObject to replace the selection with.");
            return;
        }

        foreach (GameObject selectedObject in selectedGameObjects)
        {
            if (selectedObject == null)
            {
                Debug.LogWarning("One of the selected GameObjects is null.");
                continue;
            }

            // Record the state of the object before replacing
            Undo.RecordObject(selectedObject, "Replace GameObject");

            // Instantiate the prefab at the same position and rotation
            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            newObject.transform.position = selectedObject.transform.position;
            newObject.transform.rotation = selectedObject.transform.rotation;

            // If the object has a parent, parent the new object to the same parent
            newObject.transform.SetParent(selectedObject.transform.parent);

            // Record the replacement for undo purposes
            Undo.RegisterCreatedObjectUndo(newObject, "Instantiate Prefab");

            // Destroy the original object and record it for undo
            Undo.DestroyObjectImmediate(selectedObject);

        }

        Debug.Log($"Replaced {selectedGameObjects.Length} GameObjects");
    }
}

