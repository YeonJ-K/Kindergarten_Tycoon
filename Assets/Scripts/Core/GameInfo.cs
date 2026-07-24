using System;
using System.IO;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class GameInfo : MonoBehaviour
    {
        public static GameInfo instance { get; private set; }
        private SaveData data;
        
        public string userName => data.userName;
        public UserSex userSex => data.userSex;
        public int level => data.level;
        
        
        private string FilePath => Path.Combine(Application.persistentDataPath, "users.json");

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            Load();
            // 임시
            SetUser("aa", UserSex.Female);
        }

        public void Save()
        {
            string json = JsonUtility.ToJson(data, true); // 빌드할 땐 false로 들여쓰기 하지 않게 바꿔서 용량 줄이기
            File.WriteAllText(FilePath, json);
        }

        private void Load()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                data = JsonUtility.FromJson<SaveData>(json);
            }

            else
            {
                data = new SaveData();
            }
        }

        public void SetUser(string name, UserSex sex)
        {
            data.userName = name;
            data.userSex = sex;
            Save();
        }
        
        // 사용자가 지정한 맵 크기, 입구 저장해야 함
        // 사용자가 정한 오브젝트 배치 저장해야 함
    }
}