using System;
using MessagePack;
namespace WiSdom.SaveSystem.Data
{
    [Serializable]
    [MessagePackObject]
    public class VersionData
    {
        [Key("Version")]
        public string Version = "0.1.0";
        [Key("Number")]
        public int Number = 1;
        // Add more properties as needed
    }

    [Serializable]
    [MessagePackObject]
    public class SettingsData
    {
        [Key(0)]
        public bool Master = true;
        [Key(1)]
        public float MasterVolume = 100;
        [Key(2)]
        public bool Music = true;
        [Key(3)]
        public float MusicVolume = 100;
        [Key(4)]
        public bool SFX = true;
        [Key(5)]
        public float SFXVolume = 100;
        [Key(6)]
        public bool Vibration = true;
        // Add more settings as needed
    }

    [Serializable]
    [MessagePackObject]
    public class UserInfo
    {
        [Key(0)]
        public string Name;
        [Key(1)]
        public int Age;
        // Add more player data as needed
    }
}