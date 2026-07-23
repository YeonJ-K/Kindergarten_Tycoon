using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FindCardPlay : MonoBehaviour
{
    [Header("Main")] 
    [SerializeField] private GameObject startBox;
    [SerializeField] private GameObject explainBox;
    [SerializeField] private GameObject gameBox;
    [SerializeField] private GameObject resultBox;
    
    [Header("Card")]
    [SerializeField] private Sprite[] cardSprites;
    
    [Header("Game")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private RectTransform cardGridRT;
    [SerializeField] private Slider turnTimer;
    [SerializeField] private TextMeshProUGUI kidTurnTxt;
    [SerializeField] private TextMeshProUGUI playerTurnTxt;
    [SerializeField] private GameObject kidTurnPanel;
    [SerializeField] private GameObject playerTurnPanel;
    [SerializeField] private TextMeshProUGUI kidScoreTxt;
    [SerializeField] private TextMeshProUGUI playerScoreTxt;
    [SerializeField] private Image kidProfileImage;
    [SerializeField] private Image playerProfileImage;
    private List<FindCardPlay_Card> cards; // 기존 카드드
    private List<FindCardPlay_Card> selectedCards; // 선택한 카드 (0, 1, 2)
    private List<FindCardPlay_Card> answerCards; // 정답을 맞춘 카드
    private float turnTime;
    private float waitTime;
    private float timer;
    private bool isPlaying;
    private bool isPlayerTurn;
    private int flipCard;
    private int playerScore;
    private int kidScore;
    
    [Header("Result")]
    [SerializeField] private TextMeshProUGUI resultTxt;
    [SerializeField] private TextMeshProUGUI resultExplainTxt;
    [SerializeField] private TextMeshProUGUI resultKidScoreTxt;
    [SerializeField] private TextMeshProUGUI resultPlayerScoreTxt;
    private int rewardMoney;

    private void Awake()
    {
        flipCard = 0;
        isPlayerTurn = false;
        playerScore = 0;
        kidScore = 0; 
        // 임시 
        turnTime = 5f;
        rewardMoney = 200;
        // ===
        cards = new List<FindCardPlay_Card>();
    }

    void Start()
    {
        startBox.SetActive(true);
        explainBox.SetActive(false);
        gameBox.SetActive(false);
        resultBox.SetActive(false);
        playerScoreTxt.text = playerScore.ToString();
        kidScoreTxt.text = kidScore.ToString();

        for (int i = 0; i < cardSprites.Length * 2; i++)
        {
            GameObject go = Instantiate(cardPrefab, cardGridRT);
            FindCardPlay_Card card = go.transform.GetChild(1).GetComponent<FindCardPlay_Card>();
            card.Init(CardPick);
            cards.Add(card);
        }
        Shuffle();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        turnTimer.value = timer / turnTime;
        
        waitTime += Time.deltaTime;
        if (isPlayerTurn)
        {
            if (timer > 0)
            {
                // 이때만 카드 선택 가능
            }
            else
            {
                isPlayerTurn = false;
                TurnSetting(isPlayerTurn);
                timer = 0;
            }
        }
        else // 유치원생 차례
        {
            if (timer > 0)
            {
                // 카드 AI 동작하게 하기
            }
            else
            {
                isPlayerTurn = true;
                TurnSetting(isPlayerTurn);
                timer = 0;
            }        
        }
    }

    // Fisher-Yates Shuffle 알고리즘으로 카드 섞기
    private void Shuffle()
    {
        Sprite[] spriteShuffle = new Sprite[cardSprites.Length * 2];
        for (int i = 0; i < spriteShuffle.Length; i++)
        {
           spriteShuffle[i] = cardSprites[i/2];
        }

        // 섞기
        for (int i = spriteShuffle.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i+1);
            
            (spriteShuffle[i], spriteShuffle[j]) = (spriteShuffle[j], spriteShuffle[i]);
        }

        // 나누기
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].SetCardImg(spriteShuffle[i]);
        }
    }

    void CardPick(FindCardPlay_Card card)
    {
        if (answerCards.Contains(card)) return;
        if (flipCard > 1) return;
        if (waitTime < 0.4f) return;

        card.Flip();
        selectedCards.Add(card);
        flipCard++;

        if (flipCard == 2)
        {
            if (selectedCards[0].GetCardName() == selectedCards[1].GetCardName() && selectedCards[0] != selectedCards[1])
            {
                playerScore++;
                playerScoreTxt.text = playerScore.ToString();
                for (int i = 0; i < selectedCards.Count; i++)
                {
                    answerCards.Add(selectedCards[i]);
                }
                selectedCards.Clear();
                flipCard = 0;
            }
            else
            {
                for (int i = 0; i < selectedCards.Count; i++)
                {
                    selectedCards[i].Flip();
                }
                isPlayerTurn = false;
                selectedCards.Clear();
                flipCard = 0;
            }
        }
    }

    void TurnSetting(bool isPlayer)
    {
        kidTurnPanel.SetActive(!isPlayer);
        playerTurnPanel.SetActive(isPlayer);
        kidTurnTxt.gameObject.SetActive(!isPlayer);
        playerTurnTxt.gameObject.SetActive(isPlayer);
        
    }

    public void StartGame()
    {
        isPlaying = true;
    }

    private void Win()
    {
        isPlaying = false;
        gameBox.SetActive(false);
        resultBox.SetActive(true);
        resultTxt.text = "게임에서 이겼습니다!";
        resultTxt.color = Color.blue;
        resultExplainTxt.text = $"정산시 보상금이 {rewardMoney}원 추가됩니다.";
        RoundManager.instance.GetMiniGameMoney(rewardMoney);
        RoundManager.instance.SetGameResult(true);
    }

    private void Lose()
    {
        isPlaying = false;
        gameBox.SetActive(false);
        resultBox.SetActive(true);
        resultTxt.text = "게임에서 졌습니다...";
        resultTxt.color = Color.red;
        resultExplainTxt.text = "스트레스 수치에서 1 증가합니다.";
        RoundManager.instance.IncreaseStress();
        RoundManager.instance.SetGameResult(false);
    }

    public void ExitGame()
    {
        RoundManager.instance.SetMiniGamePlaying(false);
        Destroy(gameObject);
    }
}
