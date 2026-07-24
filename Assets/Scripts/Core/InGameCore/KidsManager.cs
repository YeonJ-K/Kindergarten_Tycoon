using System;
using System.Collections.Generic;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace YEONJI.Kindergarten
{
// 유치원생 관리. 생성, 목록 보관, 전체 위치 조회
    public class KidsManager : BaseManager
    {
        // ------- Params
        [SerializeField] private List<GameObject> kidsPrefabs;
        
        private List<KidAgent> agents;
        private PlayerController player;
        
        public List<GameObject> KidsPrefabs => kidsPrefabs;

        public override async UniTask Task_Init()
        {
            await base.Task_Init();
        }

        public override void Init()
        {
            agents = new List<KidAgent>();
            base.Init();
        }

        public void SetPlayer(PlayerController playerController)
        {
            player = playerController;
        }

        public void Register(KidAgent kid) => agents.Add(kid);
        public void UnRegister(KidAgent kid) => agents.Remove(kid);

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
}