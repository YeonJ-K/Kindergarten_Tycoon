using System;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private GameObject[] playerPrefab;
    [SerializeField] private KidsManager kidsManager;
    private Vector2Int playerSpawn;
    private Vector2Int kidSpawn;

    private void Awake()
    {
        // 임시 값 
        playerSpawn = new Vector2Int();
        kidSpawn = new Vector2Int();
        
        // -----
    }

    private void Start()
    {
        GameObject go = Instantiate(playerPrefab[0], new Vector3(playerSpawn.x, playerSpawn.y), Quaternion.identity);
        
    }
}
