using UnityEditor;
using UnityEngine;

public class BatchRename : EditorWindow
{
    // Variables
    private string prefix = "";
    private string suffix = "";
    private string baseName = "";

    private bool numbered = false;
    private int startingNum = 1;

    private GameObject[] selectedObjects = null;

    [MenuItem("Tools/BatchRenamer")]
    public static void ShowWindow()
    {
        GetWindow<BatchRename>("BatchRenamer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Rename Settings", EditorStyles.boldLabel);

        prefix = EditorGUILayout.TextField("Prefix", prefix);
        baseName = EditorGUILayout.TextField("Name", baseName);
        suffix = EditorGUILayout.TextField("Suffix", suffix);

        numbered = EditorGUILayout.Toggle("Numbered", numbered);
        if (numbered)
        {
            startingNum = EditorGUILayout.IntField("Start from", startingNum);
        }

        if (GUILayout.Button("Rename Objects"))
        {
            RenameObjects();
        }

    }

    private void RenameObjects()
    {
        selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length <= 0)
        {
            Debug.LogWarning("No GameObjects selected. Please, select at least one GameObjects to rename.");
            return;
        }

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            // The $ symbol makes the string interpolated so the parts between brackets {} are replaced
            string newName = $"{prefix}{(string.IsNullOrEmpty(baseName) ? selectedObjects[i].name : baseName)}{suffix}";

            if (numbered) newName += $"_{startingNum + i}";

            Undo.RecordObject(selectedObjects[i], newName);
            selectedObjects[i].name = newName;
        }

        Debug.Log($"Renamed {selectedObjects.Length} GameObjects");
    }

}
