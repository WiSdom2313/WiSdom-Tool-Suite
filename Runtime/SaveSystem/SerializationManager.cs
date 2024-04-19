using System.IO;
using MessagePack;
using UnityEngine;

namespace WiSdom.SaveSystem
{
    public class SerializationManager : MonoBehaviour
    {
        // Generic method to save data of any type
        public static async Cysharp.Threading.Tasks.UniTask SaveData<T>(T data, string file)
        {
            file = Path.Combine(Application.persistentDataPath, file);
            byte[] serializedData = MessagePackSerializer.Serialize(data);
            using (FileStream stream = new FileStream(file, FileMode.OpenOrCreate))
            {
                await stream.WriteAsync(serializedData, 0, serializedData.Length);
            }
        }

        // Generic method to load data of any type
        public static async Cysharp.Threading.Tasks.UniTask<T> LoadData<T>(string file)
        {
            file = Path.Combine(Application.persistentDataPath, file + ".dat");
            using (FileStream stream = new FileStream(file, FileMode.OpenOrCreate))
            {
                byte[] buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                return MessagePackSerializer.Deserialize<T>(buffer);
            }
        }
    }
}
