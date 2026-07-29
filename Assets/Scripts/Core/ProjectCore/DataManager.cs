using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public static class JsonLoader
    {
        public static T Load<T>(string resourcePath)
        {
            TextAsset json = Resources.Load<TextAsset>(resourcePath);
            if (json == null)
            {
                Debug.LogError($"[JsonLoader] JSON 없음: {resourcePath}");
                return default;
            }
            return JsonUtility.FromJson<T>(json.text);
        }
    }
    public class DataManager : BaseManager
    {

        private SaveData saveData = new SaveData();
        
        public FurnitureMetaData Furniture { get; private set; }
        public string userName => saveData.userName;
        public UserSex userSex => saveData.userSex;
        public int level => saveData.level;
        
        public override async UniTask Task_Init()
        {
            await base.Task_Init();
        }

        public override void Init()
        {
            base.Init();

            LoadTables();
            LoadSave();
            // 임시
            SetUser("aa", UserSex.Female);

        }
        
        private string FilePath => Path.Combine(Application.persistentDataPath, "users.json");

        

        public void Save()
        {
            string json = JsonUtility.ToJson(saveData, true); // 빌드할 땐 false로 들여쓰기 하지 않게 바꿔서 용량 줄이기
            File.WriteAllText(FilePath, json);
        }

        private void LoadSave()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                saveData = JsonUtility.FromJson<SaveData>(json);
            }

            else
            {
                saveData = new SaveData();
            }
        }
        
        // ---- Table 
        private void LoadTables()
        {
            Furniture = new FurnitureMetaData();
            var json = Resources.Load<TextAsset>("Data/FurnitureTable");
            Furniture.Setting(json.text);
        }

        public void SetUser(string name, UserSex sex)
        {
            saveData.userName = name;
            saveData.userSex = sex;
            Save();
        }
        
        // 사용자가 지정한 맵 크기, 입구 저장해야 함
        // 사용자가 정한 오브젝트 배치 저장해야 함
    }

    public class SaveData
    {
        public string userName;
        public UserSex userSex;
        public int level;
        public int levelPerKidsNum;

    }
}