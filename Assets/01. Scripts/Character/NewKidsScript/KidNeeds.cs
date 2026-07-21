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
        // ===
        levelDropTimer = GetLevelDropTime();
        isActive = false;
    }
    public NeedLevel Get(NeedType type) => kidStatus[(int)type];
    private int GetLevelDropTime() => Random.Range(levelDropTimeMin, levelDropTimeMax);

    // 6초마다 호출. 아주 좋음, 좋음 단계에서만 해당 기능 실행
    private void LevelDrop()
    {
        if (Random.value < 0.5f)
        {
            int dropTypePick = Random.Range(0, 3);
            if (kidStatus[dropTypePick] > NeedLevel.Normal)
                kidStatus[dropTypePick]--;
            Debug.Log($"{(NeedType)dropTypePick} 하락 → {kidStatus[dropTypePick]}");
        }
    }

    public void Tick(float dt)
    {
        if (!RoundManager.instance.roundStart) return;
        
        levelDropTimer -= dt;
        if (levelDropTimer <= 0f)
        {
            LevelDrop();
            levelDropTimer = GetLevelDropTime();
        }

        for (int i = 0; i < kidStatus.Length; i++)
        {
            if (kidStatus[i] <= NeedLevel.Normal && kidStatus[i] > NeedLevel.VeryBad)
            {
                limitTimer[i] += dt;
                if (limitTimer[i] >= requestTime)
                    kidStatus[i] = NeedLevel.VeryBad;
                else if (limitTimer[i] >= badTime)
                    kidStatus[i] = NeedLevel.Bad;
            }
            else
            {
                limitTimer[i] = 0f;
            }
        }
    }
}
