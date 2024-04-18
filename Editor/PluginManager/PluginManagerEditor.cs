using UnityEngine;
using UnityEditor;

public class PluginManagerEditor : EditorWindow
{
    private PluginConfig pluginConfig;
    private Vector2 scrollPosition;

    [MenuItem("WiSdom/Plugin Manager", priority = 0)]
    public static void ShowWindow()
    {
        GetWindow<PluginManagerEditor>("Plugin Manager");
    }
    private GUIStyle headerStyle = null;

    void OnEnable()
    {
        headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.alignment = TextAnchor.MiddleCenter;
        pluginConfig = AssetDatabase.LoadAssetAtPath<PluginConfig>("Assets/WiSdomToolSuite/Editor/PluginManager/PluginConfig.asset");
    }

    void OnGUI()
    {
        if (pluginConfig == null)
        {
            GUILayout.Label("Plugin Config not loaded!", EditorStyles.boldLabel);
            return;
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add New Plugin"))
        {
            AddNewPlugin();
        }
        if (GUILayout.Button("Open Plugin Config"))
        {
            Selection.activeObject = pluginConfig;
        }
        if (GUILayout.Button("Open enum file"))
        {
            // open PluginConfig.cs in ide
            string path = "Assets/WiSdomToolSuite/Editor/PluginManager/PluginConfig.cs";
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, 0);
        }
        GUILayout.EndHorizontal();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUILayout.BeginVertical(); // Begin vertical grouping

        EditorGUILayout.BeginHorizontal(); // Create a header row
        GUILayout.Label("", EditorStyles.boldLabel, GUILayout.Width(50));
        GUILayout.Label("Name", EditorStyles.boldLabel, GUILayout.Width(100));
        GUILayout.Label("Define Symbol", EditorStyles.boldLabel, GUILayout.Width(150));
        GUILayout.Label("", GUILayout.Width(60)); // For remove button spacing
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < pluginConfig.plugins.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            pluginConfig.plugins[i].IsEnable = EditorGUILayout.Toggle("", pluginConfig.plugins[i].IsEnable, GUILayout.Width(50));
            pluginConfig.plugins[i].Name = (ePlugin)EditorGUILayout.EnumPopup("", pluginConfig.plugins[i].Name, GUILayout.Width(100));
            pluginConfig.plugins[i].DefineSymbol = EditorGUILayout.TextField("", pluginConfig.plugins[i].DefineSymbol, GUILayout.Width(150));

            // if (GUILayout.Button("Remove", GUILayout.Width(60)))
            // {
            //     RemovePlugin(i);
            //     break; // Exit loop to avoid modifying the collection during iteration
            // }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical(); // End vertical grouping
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Save Changes"))
        {
            // Add define symbols, remove if not enabled or not defined
            foreach (var plugin in pluginConfig.plugins)
            {
                if (plugin.IsEnable && !string.IsNullOrEmpty(plugin.DefineSymbol))
                {
                    if (!PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Contains(plugin.DefineSymbol))
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup) + ";" + plugin.DefineSymbol);
                    }
                }
                else
                {
                    if (PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Contains(plugin.DefineSymbol))
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Replace(plugin.DefineSymbol, ""));
                    }
                }
            }

            EditorUtility.SetDirty(pluginConfig);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private void AddNewPlugin()
    {
        var list = new System.Collections.Generic.List<PluginData>(pluginConfig.plugins);
        list.Add(new PluginData { Name = ePlugin.None, IsEnable = false, DefineSymbol = "" });
        pluginConfig.plugins = list.ToArray();
        EditorUtility.SetDirty(pluginConfig);
    }

    // private void RemovePlugin(int index)
    // {
    //     var list = new System.Collections.Generic.List<PluginData>(pluginConfig.plugins);
    //     list.RemoveAt(index);
    //     pluginConfig.plugins = list.ToArray();
    //     EditorUtility.SetDirty(pluginConfig);
    // }
}
