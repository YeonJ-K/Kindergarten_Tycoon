using System;
using System.Collections.Generic;
using UnityEngine;

// 유치원생 관리. 생성, 목록 보관, 전체 위치 조회
public class KidsManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> kidsPrefabs;
    private List<Kids> kidsList;
    private PlayerController player;

    public static KidsManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        kidsList = new List<Kids>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void Register(Kids kid)
    {
        kidsList.Add(kid);
    }

    public void UnRegister(Kids kid)
    {
        kidsList.Remove(kid);
    }

    public HashSet<Vector2Int> GetOccupied(Vector2Int pos)
    {
        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();
        foreach (var kid in kidsList)
        {
            if (kid.CurrentCell == pos) continue;
            occupied.Add(kid.CurrentCell);
        }
        occupied.Add(player.CurrentCell);

        return occupied;
    }

    public void Entering(Vector2Int pos)
    {
        
    }
}
