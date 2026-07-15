using System;
using UnityEngine;

public enum UserSex
{
    Male,
    Female
}

public class GameInfo : MonoBehaviour
{
    private string userName; // xml 형태로 기기 내에 저장하기
    private UserSex userSex; // xml 형태로 기기 내에 저장하기
    public static GameInfo instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    
}
