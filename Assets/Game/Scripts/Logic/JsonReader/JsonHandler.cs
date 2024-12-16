using System;
using System.IO;
using UnityEngine;

namespace Game.Scripts.Logic.JsonReader
{
    public class JsonHandler
    {
        private readonly string _filePath = Path.Combine(Application.persistentDataPath, "user.json");

        public Account AccountInit()
        {
            Account account;
            
            if (!File.Exists(_filePath))
            {
                account = new Account
                {
                    name = "Ilias"
                };
            }
            else
            {
                account = LoadFromJson();
            }

            return account;
        }
        
        public void SaveToJson(Account data)
        {
            try
            {
                var json = JsonUtility.ToJson(data, true);
                File.WriteAllText(_filePath, json);
                Debug.Log("Data saved successfully: " + _filePath);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save data: " + e.Message);
            }
        }
        
        public Account LoadFromJson()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    var data = JsonUtility.FromJson<Account>(json);
                    Debug.Log("Data loaded from JSON");
                    return data;
                }

                Debug.LogWarning("File is not found: " + _filePath);
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError("Something went wrong: " + e.Message);
                return null;
            }
        }
    }
}
