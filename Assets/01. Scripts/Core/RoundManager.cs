using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Infos;
using Random = UnityEngine.Random;


public class RoundManager : MonoBehaviour
{
    [SerializeField] private GameObject[] playerPrefab; // 나중에 데이터 시트로 가져오기?
    private Vector2Int playerSpawn;
    public Vector2Int kidsSpawnPos { get; private set; }

    public static RoundManager instance;

    private float enterDuration;
    private float exitDuration;
    private float roundFinishTime;
    private int roundPerKids;
    
    public bool roundStart { get; private set; }
    public int StressCount { get; private set; }
    public int MiniGameMoney { get; private set; }
    public bool isMiniGamePlaying { get;  private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        MiniGameMoney = 0;
        isMiniGamePlaying = false;
        
        // 임시 값 
        playerSpawn = new Vector2Int(11,3);
        enterDuration = 15;
        exitDuration = 15;
        roundFinishTime = 180;
        roundPerKids = 3;
        // -----

    }

    private void Start()
    {
        StartCoroutine(RoundRoutine());
    }

    private IEnumerator RoundRoutine()
    {
        SpawnPlayer();
        SetKidsSpawnPos();
        
        yield return StartCoroutine(Entering());

        yield return new WaitForSeconds(roundFinishTime);
        Debug.Log("퇴장 시작");
        roundStart = false;
        KidsAIManager.instance.ExitAll();
    }

    private void SpawnPlayer()
    {
        GameObject go = Instantiate(GameInfo.instance.userSex == UserSex.Male ? playerPrefab[0] : playerPrefab[1],
            GridMap.instance.GridToWorld(playerSpawn.x, playerSpawn.y), Quaternion.identity);
        KidsManager.instance.SetPlayer(go.GetComponent<PlayerController>());
    }

    private void SetKidsSpawnPos()
    {
        GridMap.instance.GetZoneBounds(ZoneType.Entrance, out var min, out var max);
        int x = (min.x + max.x) / 2;
        int y = max.y;
        if (GridMap.instance.GetCell(x, y).zone != ZoneType.Entrance) return;
        kidsSpawnPos = new Vector2Int(x, y);
    }

    // 라운드마다 겹치지 않는 랜덤한 유치원생 생성하기 위한 번호 뽑기
    // Fisher-Yates 사용
    private List<int> GetKidsNum(int levelPerKidsNum)
    {
        List<int> all = new List<int>();
        for (int i = 0; i < KidsManager.instance.KidsPrefabs.Count; i++)
        {
            all.Add(i);
        }
        
        for (int i = all.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = 0;
            temp = all[i];
            all[i] = all[j];
            all[j] = temp;
        }

        List<int> roundKidsList = new List<int>();
        int count = Mathf.Min(levelPerKidsNum, all.Count);
        for (int i = 0; i < count; i++)
        {
            roundKidsList.Add(all[i]);
        }
        return roundKidsList;
    }

    private IEnumerator Entering()
    {
        List<int> kidsNum = GetKidsNum(roundPerKids);
        
        float remainTime = enterDuration;
        for (int i = 0; i < kidsNum.Count; i++)
        {
            GameObject go = Instantiate(KidsManager.instance.KidsPrefabs[kidsNum[i]],
                GridMap.instance.GridToWorld(kidsSpawnPos.x, kidsSpawnPos.y), Quaternion.identity);
            KidAgent agent = go.GetComponent<KidAgent>();
            agent.Init(kidsSpawnPos);
            KidsAIManager.instance.Register(agent);
            
            int remainCount = kidsNum.Count - i;
            float baseInterval = remainTime / remainCount;
            float wait = Random.Range(baseInterval * 0.3f, baseInterval);
            remainTime -= wait;
            yield return new WaitForSeconds(wait); 
        }
        roundStart = true;
        ViewController.instance.SwitchTo(ViewMode.MainRoom);
    }

    public void IncreaseStress() => StressCount++;
    public void GetMiniGameMoney(int money)=> MiniGameMoney += money;
    public void SetMiniGamePlaying(bool isPlaying) => isMiniGamePlaying = isPlaying;
    
    

}
