using UnityEngine;

[CreateAssetMenu(fileName = "PluginConfig", menuName = "Configuration/PluginConfig")]
public class PluginConfig : ScriptableObject
{
    public PluginData[] plugins;
}

[System.Serializable]
public class PluginData
{
    public ePlugin Name;
    public bool IsEnable;
    public string DefineSymbol;
}
public enum ePlugin
{
    None,
    Addressables,
    UniTaskV2,
    MessagePack,
}
