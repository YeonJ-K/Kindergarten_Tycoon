using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
// 유치원생 관리. 생성, 목록 보관, 전체 위치 조회
public class KidsManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> kidsPrefabs;
    //private List<Kids> kidsList;
    private List<KidAgent> agents;
    private PlayerController player;

    public static KidsManager instance { get; private set; }
    public List<GameObject> KidsPrefabs => kidsPrefabs;
    

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        //kidsList = new List<Kids>();
        agents = new List<KidAgent>();

    }

    public void SetPlayer(PlayerController playerController)
    {
        player = playerController;
    }

    public void Register(KidAgent kid)=> agents.Add(kid);
    public void UnRegister(KidAgent kid)=> agents.Remove(kid);

    //public HashSet<Vector2Int> GetOccupied(Kids self)
    public HashSet<Vector2Int> GetOccupied(KidAgent self)
    {
        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();
        foreach (var kid in agents)
        {
            if (kid == self) continue;
            occupied.Add(kid.CurrentCell);
            occupied.Add(kid.NextCell);
        }
        occupied.Add(player.CurrentCell);
        return occupied;
    }
    
    public HashSet<Vector2Int> GetOccupied()
    {
        var occupied = new HashSet<Vector2Int>();
        foreach (var kid in agents)
        {
            occupied.Add(kid.CurrentCell);
            occupied.Add(kid.NextCell);
        }

        return occupied;
    }

    public KidAgent GetKidAgent(Vector2Int cell)
    {
        foreach (var agent in agents)
        {
            if (agent.CurrentCell == cell)
                return agent;
        }
        return null;
    }

}
