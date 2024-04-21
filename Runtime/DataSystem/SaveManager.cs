using System.Collections.Concurrent;
using System.IO;
using Cysharp.Threading.Tasks;
using MessagePack;
using UnityEngine;
using WiSdom;

public static class SaveManager
{
    // Method to save data with a specific cancellation token
    private static ConcurrentDictionary<string, string> _saveNames = new ConcurrentDictionary<string, string>();
    public static async UniTask SaveData<T>(T data, string fileBase)
    {
#if USE_MEMORYPACKJSON
            // Save as JSON
            string jsonPath = Path.Combine(Application.persistentDataPath, fileBase + ".json");
            string jsonData = MessagePackSerializer.SerializeToJson(data);
            using (StreamWriter stream = new StreamWriter(jsonPath))
            {
                await stream.WriteAsync(jsonData);
            }
#else

        // Save as binary
        string binaryPath = Path.Combine(Application.persistentDataPath, Utility.EncodeStringToByte(fileBase) + ".bin");
        byte[] serializedData = MessagePackSerializer.Serialize(data);
        using (FileStream stream = new FileStream(binaryPath, FileMode.Create))
        {
            await stream.WriteAsync(serializedData, 0, serializedData.Length);
        }
#endif
    }

    // Generic method to load data of any type
    public static async UniTask<T> LoadData<T>(string fileBase)
    {
#if USE_MEMORYPACKJSON
            // Load from JSON
            string jsonPath = Path.Combine(Application.persistentDataPath, fileBase + ".json");
            using (StreamReader stream = new StreamReader(jsonPath))
            {
                string jsonData = await stream.ReadToEndAsync();
                return MessagePackSerializer.Deserialize<T>(MessagePackSerializer.ConvertFromJson(jsonData));
            }
#else
        // Load from binary
        string binaryPath = Path.Combine(Application.persistentDataPath, Utility.EncodeStringToByte(fileBase) + ".bin");
        using (FileStream stream = new FileStream(binaryPath, FileMode.Open))
        {
            byte[] serializedData = new byte[stream.Length];
            await stream.ReadAsync(serializedData, 0, (int)stream.Length);
            return MessagePackSerializer.Deserialize<T>(serializedData);
        }
#endif
    }

    public static void ClearData(string fileBase)
    {
#if USE_MEMORYPACKJSON
        // Clear data
        string jsonPath = Path.Combine(Application.persistentDataPath, fileBase + ".json");
        File.Delete(jsonPath);
#else
        // Clear data
        string binaryPath = Path.Combine(Application.persistentDataPath, Utility.EncodeStringToByte(fileBase) + ".bin");
        File.Delete(binaryPath);
#endif
    }
}