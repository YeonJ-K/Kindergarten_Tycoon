using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class KidsAI
    {
        public KidAgent agent;
        public KidsContext context;
        public StateMachine machine;
    }

    public class KidsAIManager : BaseManager
    {
        private List<KidsAI> kidAIs;

        public override async UniTask Task_Init()
        {
            await base.Task_Init();
        }

        public override void Init()
        {
            base.Init();
        }
        
        private void Awake()
        {
            kidAIs = new List<KidsAI>();
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            foreach (KidsAI kidAI in kidAIs)
            {
                kidAI.context.needs.Tick(dt);
                kidAI.agent.Tick(dt); // 이동 갱신
                kidAI.machine.Tick(dt); // 상태 판단
            }

            RemoveExited();
        }

        public void Register(KidAgent agent)
        {
            var context = new KidsContext();
            context.agent = agent;
            agent.SetContext(context);
            
            var machine = new StateMachine(context, new EnteringState());
            context.machine = machine;

            var kidAI = new KidsAI { agent = agent, context = context, machine = machine };
            kidAIs.Add(kidAI);
        }

        private void RemoveExited()
        {
            for (int i = kidAIs.Count - 1; i >= 0; i--)
            {
                if (kidAIs[i].context.wantExit)
                {
                    var agent = kidAIs[i].agent;
                    InGameCore.KIDS.UnRegister(agent);
                    kidAIs.RemoveAt(i);
                    Destroy(agent.gameObject);
                }
            }
        }
        
        public void ExitAll()
        {
            foreach (var kidAI in kidAIs)
            {
                kidAI.machine.ChangeState(new ExitingState());
            }
        }

    }
}