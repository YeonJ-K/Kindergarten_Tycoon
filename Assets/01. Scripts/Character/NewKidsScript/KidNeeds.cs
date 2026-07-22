using UnityEngine;
using Infos;

public class KidNeeds
{
    public NeedLevel[] kidStatus;
    private int levelDropTimeMin;
    private int levelDropTimeMax;
    private float levelDropTimer;
    private float[] limitTimer;
    private float requestTime;
    private float badTime;
    
    private float stressTimer;
    public bool levelChanged;
    public bool isActive;
    

    public KidNeeds()
    {
        kidStatus = new NeedLevel[3];
        limitTimer = new float[3];
        for (int i = 0; i < kidStatus.Length; i++)
            kidStatus[i] = NeedLevel.VeryGood;
        // 임시
        levelDropTimeMin = 6;
        levelDropTimeMax = 10;
        requestTime = 25f;
        badTime = 15f;
        stressTimer = 8f;
        // ===
        levelDropTimer = GetLevelDropTime();
        isActive = false;
    }
    public NeedLevel Get(NeedType type) => kidStatus[(int)type];
    private int GetLevelDropTime() => Random.Range(levelDropTimeMin, levelDropTimeMax);

    public void Tick(float dt)
    {
        if (!RoundManager.instance.roundStart) return;
        
        levelDropTimer -= dt;
        if (levelDropTimer <= 0f)
        {
            LevelDrop();
            levelDropTimer = GetLevelDropTime();
        }
        
        WaitRequestProcess(dt);
        StressProcess(dt);
        
    }

    // 6초마다 호출. 아주 좋음, 좋음 단계에서만 해당 기능 실행
    private void LevelDrop()
    {
        if (Random.value < 0.5f)
        {
            int dropTypePick = Random.Range(0, 3);
            NeedLevel before = kidStatus[dropTypePick];
            if (kidStatus[dropTypePick] > NeedLevel.Normal)
                kidStatus[dropTypePick]--;
            
            if (before == NeedLevel.Good && kidStatus[dropTypePick] == NeedLevel.Normal)
                levelChanged = true;
        }
    }

    public float GetTimerRatio(NeedType type)
    {
        if (kidStatus[(int)type] == NeedLevel.VeryBad)
            return 1f;
            
        return limitTimer[(int)type] / requestTime;    
    }

    

    private void WaitRequestProcess(float dt)
    {
        for (int i = 0; i < kidStatus.Length; i++)
        {
            NeedLevel before = kidStatus[i];
            if (kidStatus[i] <= NeedLevel.Normal && kidStatus[i] > NeedLevel.VeryBad)
            {
                limitTimer[i] += dt;
                if (limitTimer[i] >= requestTime)
                {
                    kidStatus[i] = NeedLevel.VeryBad;
                }

                else if (limitTimer[i] >= badTime)
                {
                    kidStatus[i] = NeedLevel.Bad;
                }
            }
            else
            {
                limitTimer[i] = 0f;
            }
            
            if (before == NeedLevel.Bad && kidStatus[i] == NeedLevel.VeryBad)
                levelChanged = true;
            
            if (before != NeedLevel.VeryBad && kidStatus[i] == NeedLevel.VeryBad)
            {
                RoundManager.instance.IncreaseStress();
            }
        }
    }

    private void StressProcess(float dt)
    {
        bool anyVeryBad = false;
        for (int i = 0; i < kidStatus.Length; i++)
            if (kidStatus[i] == NeedLevel.VeryBad) anyVeryBad = true;

        if (anyVeryBad)
        {
            stressTimer -= dt;
            if (stressTimer <= 0f)
            {
                RoundManager.instance.IncreaseStress();
                stressTimer = 8f;
            }
        }
        else
        {
            stressTimer = 8f;   // 아주나쁨 없으면 리셋
        }
    }

    public NeedLevel GetWorst()
    {
        NeedLevel worst = NeedLevel.VeryGood;
        for (int i = 0; i < kidStatus.Length; i++)
        {
            if (kidStatus[i] < worst)
                worst = kidStatus[i];
        }
        return worst;
    }
}
