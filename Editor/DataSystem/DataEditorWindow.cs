using UnityEditor;
using UnityEngine;
using WiSdom.SaveSystem;  // Adjust namespace to where your DataManager lives

public class DataEditorWindow : EditorWindow
{
    private string _message = "Data Editor";
    [MenuItem("WiSdom/Data Editor")]
    public static void ShowWindow()
    {
        GetWindow<DataEditorWindow>("Data Editor");
    }

    private void OnGUI()
    {
        if (DataManager.I == null)
        {
            GUILayout.Label("DataManager not found", EditorStyles.boldLabel);
            return;
        }
        GUILayout.BeginHorizontal();
        // Button to save all data
        if (GUILayout.Button("Save All Data"))
        {
            DataManager.I.SaveAllData().Forget();
            Debug.Log("Data Saved");
        }
        // Button to load all data
        if (GUILayout.Button("Load All Data"))
        {
            DataManager.I.LoadAllData().Forget();
            Debug.Log("Data Loaded");
        }
        // Button to clear all data
        if (GUILayout.Button("Clear All Data"))
        {
            DataManager.I.ClearAllData();
            Debug.Log("Data Cleared");
        }

        GUILayout.EndHorizontal();
        _message = EditorGUILayout.TextField("Message", _message);

        EditorGUILayout.Space();
    }
}
