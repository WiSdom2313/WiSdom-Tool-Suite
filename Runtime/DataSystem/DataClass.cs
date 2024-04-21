using System;
using MessagePack;
namespace WiSdom.SaveSystem.Data
{
    [MessagePackObject]
    [Serializable]
    public class PlayerData
    {
        [Key(0)]
        public int Level = 0;
        // Add more properties as needed
    }

    [MessagePackObject]
    [Serializable]
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
        // Add more settings as needed
    }
}