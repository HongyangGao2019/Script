using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace DailyMeals
{
    public class SaveManager : MonoBehaviour
    {
        [SerializeField] private string fileName;
         
        public event UnityAction SaveEvent; 
        public event UnityAction LoadEvent; 
        public static SaveManager Singleton { get; private set; }
        private string DataSavePath
        {
            get => $"{Application.persistentDataPath}/{fileName}.json";
        }
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Singleton = this;
        }

        private void Start()
        {
            GameManager.Singleton.InitLevel += Load;
            GameManager.Singleton.EndLevel += Save;
        }        
        private void OnDestroy()
        {
            GameManager.Singleton.InitLevel -= Load;
            GameManager.Singleton.EndLevel -= Save;
        }

        public void Save()
        {
            SaveData saveData = new SaveData(GameManager.Singleton.GoldCoin, GameManager.Singleton.Money);
            string data=JsonUtility.ToJson(saveData);
            CheckAndCreateNewFile();
            File.WriteAllText(DataSavePath, data);
            SaveEvent?.Invoke();
        }

        public void Load()
        {
            CheckAndCreateNewFile();
            string data = File.ReadAllText(DataSavePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(data);
            GameManager.Singleton.SetGoldCoin(saveData.goldCoin);
            GameManager.Singleton.SetMoney(saveData.money);
            LoadEvent?.Invoke();
        }

        private void CheckAndCreateNewFile()
        {
            if (!File.Exists(DataSavePath))
            {
                FileStream fs=File.Create(DataSavePath);
                fs.Close();
                File.WriteAllText(DataSavePath,JsonUtility.ToJson(new SaveData(0, 0f)));
            }
        }
    }
}

