using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePack;
using UnityEngine;
using WiSdom.DesignPattern;
using WiSdom.SaveSystem.Data;

namespace WiSdom.SaveSystem
{
    public class DataManager : Singleton<DataManager>
    {
        #region Fields
        private static ConcurrentDictionary<string, CancellationTokenSource> TokenSources = new ConcurrentDictionary<string, CancellationTokenSource>();
        #endregion

        #region Methods
        // Method to save data with a specific cancellation token
        public static async UniTask SaveData<T>(T data, string fileBase)
        {
            CancellationTokenSource cts = TokenSources.GetOrAdd(fileBase, _ => new CancellationTokenSource());

            try
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
                string binaryPath = Path.Combine(Application.persistentDataPath, Utility.GetSecureFileName(fileBase) + ".bin");
                byte[] serializedData = MessagePackSerializer.Serialize(data);
                using (FileStream stream = new FileStream(binaryPath, FileMode.Create))
                {
                    await stream.WriteAsync(serializedData, 0, serializedData.Length, cts.Token);
                    cts.Token.ThrowIfCancellationRequested();
                }
#endif
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Save operation for {fileBase} was canceled.");
                // Optionally clean up or handle the cancellation specifically
                // For instance, delete partially written files if needed
            }
            finally
            {
                // Once the save operation is completed or canceled, dispose of the CancellationTokenSource
                if (TokenSources.TryRemove(fileBase, out CancellationTokenSource existingCts))
                {
                    existingCts.Dispose();
                }
            }
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
            string binaryPath = Path.Combine(Application.persistentDataPath, Utility.DecodeFileName(fileBase) + ".bin");
            using (FileStream stream = new FileStream(binaryPath, FileMode.Open))
            {
                byte[] serializedData = new byte[stream.Length];
                await stream.ReadAsync(serializedData, 0, (int)stream.Length);
                return MessagePackSerializer.Deserialize<T>(serializedData);
            }
#endif
        }

        #endregion


        #region Field Generators
        public PlayerData PlayerData;
        public SettingsData SettingsData;
        #endregion
        #region Methods Generators
        public static bool ISALLDATALOADED = false;
        public async UniTaskVoid LoadAllData()
        {
            ISALLDATALOADED = false;
            var task1 = LoadData<PlayerData>("PlayerData");
            PlayerData = await task1;
            var task2 = LoadData<SettingsData>("SettingsData");
            SettingsData = await task2;
            // all done
            await (task1, task2);
            ISALLDATALOADED = true;

        }
        public static bool ISDATACHANGE = false;
        public async UniTaskVoid SaveAllData()
        {
            ISDATACHANGE = true;
            await SaveData(PlayerData, "PlayerData");
            await SaveData(SettingsData, "SettingsData");
            ISDATACHANGE = false;
        }

        public void ClearAllData()
        {
#if USE_MEMORYPACKJSON
            File.Delete(Path.Combine(Application.persistentDataPath, "PlayerData.json"));
            File.Delete(Path.Combine(Application.persistentDataPath, "SettingsData.json"));
#else
            File.Delete(Path.Combine(Application.persistentDataPath, Path.Combine(Application.persistentDataPath, Utility.DecodeFileName("PlayerData") + ".bin")));
            File.Delete(Path.Combine(Application.persistentDataPath, Path.Combine(Application.persistentDataPath, Utility.DecodeFileName("SettingsData") + ".bin")));
#endif
        }
        #endregion
    }
}