using System;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    private Vector2Int playerSpawn;
    private Vector2Int kidSpawn;

    private void Awake()
    {
        playerSpawn = new Vector2Int();
        kidSpawn = new Vector2Int();
    }
}
