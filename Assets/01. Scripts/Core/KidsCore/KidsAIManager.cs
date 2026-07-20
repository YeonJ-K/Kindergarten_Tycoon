using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidsAI
{
    public KidAgent agent;
    public KidsContext context;
    public StateMachine machine;
}

public class KidsAIManager : MonoBehaviour
{
    public static KidsAIManager instance { get; private set; }
    private List<KidsAI> kidAIs;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        
        kidAIs = new List<KidsAI>();
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        foreach (KidsAI kidAI in kidAIs)
        {
            kidAI.agent.Tick(dt); // 이동 갱신
            kidAI.machine.Tick(dt); // 상태 판단
        }

        RemoveExited();
    }

    public void Register(KidAgent agent)
    {
        var context = new KidsContext();
        context.agent = agent;

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
                KidsManager.instance.UnRegister(agent);
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
