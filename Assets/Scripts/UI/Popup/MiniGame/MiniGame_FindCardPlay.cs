using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace YEONJI.Kindergarten
{

    public class MiniGame_FindCardPlay : UIBase
    {
        public override UIType UIType { get => UIType.MiniGame_FindCardPlay; }
        
        [Header("Main")] [SerializeField] private GameObject startBox;
        [SerializeField] private GameObject explainBox;
        [SerializeField] private GameObject gameBox;
        [SerializeField] private GameObject resultBox;

        [Header("Card")] [SerializeField] private Sprite[] cardSprites;

        [Header("Game")] [SerializeField] private GameObject cardPrefab;
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
        private List<MiniGame_Card> cards; // 기존 카드드
        private List<MiniGame_Card> selectedCards; // 선택한 카드 (0, 1, 2)
        private List<MiniGame_Card> answerCards; // 정답을 맞춘 카드
        private float turnTime;
        private float timer;
        private bool isPlaying;
        private bool isPlayerTurn;
        private int flipCard;
        private int playerScore;
        private int kidScore;
        private bool isChecking;

        // AI용 변수
        private HashSet<MiniGame_Card> rememberCards;
        private CardMiniGameLevel AILevel;
        private bool AIWork;
        private float easyRate;
        private float normalRate;
        private float hardRate;

        [Header("Result")] [SerializeField] private TextMeshProUGUI resultTxt;
        [SerializeField] private TextMeshProUGUI resultExplainTxt;
        [SerializeField] private TextMeshProUGUI resultKidScoreTxt;
        [SerializeField] private TextMeshProUGUI resultPlayerScoreTxt;
        private int rewardMoney;

        public void Init()
        {
            flipCard = 0;

            playerScore = 0;
            kidScore = 0;
            // 임시 
            turnTime = 5f;
            rewardMoney = 200;
            easyRate = 0.2f;
            normalRate = 0.4f;
            hardRate = 0.6f;
            // ===
            AIWork = false;
            cards = new List<MiniGame_Card>();
            answerCards = new List<MiniGame_Card>();
            selectedCards = new List<MiniGame_Card>();
            rememberCards = new HashSet<MiniGame_Card>();
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
                MiniGame_Card card = go.transform.GetChild(1).GetComponent<MiniGame_Card>();
                card.Init(CardPick);
                cards.Add(card);
            }

            Shuffle();
            timer = turnTime;
            isPlayerTurn = false;
            kidTurnPanel.SetActive(false);
            playerTurnPanel.SetActive(false);
            kidTurnTxt.gameObject.SetActive(false);
            playerTurnTxt.gameObject.SetActive(false);
            // 임시 
            AILevel = CardMiniGameLevel.EASY;
            // ---
        }

        void Update()
        {
            if (!isPlaying) return;
            if (answerCards.Count == cards.Count)
            {
                if (playerScore > kidScore)
                {
                    Win();
                    return;
                }

                if (playerScore < kidScore)
                {
                    Lose();
                    return;
                }

                Draw();
                return;
            }

            timer -= Time.deltaTime;
            turnTimer.value = timer / turnTime;

            if (isPlayerTurn)
            {
                if (timer > 0)
                {
                    // 이때만 카드 선택 가능
                }
                else if (!isChecking)
                    TurnSetting(false, isPlayerTurn);
            }
            else // 유치원생 차례
            {
                if (timer > 0)
                {
                    if (AIWork)
                    {
                        AIWork = false;
                        StartCoroutine(KidCardAI());
                    }

                }
                else if (!isChecking)
                    TurnSetting(false, isPlayerTurn);
            }
        }

        private void SettingProfile()
        {
            string kidName = InGameCore.ROUND.miniGameKid.name.Replace("(Clone)", "");
            kidName = kidName.Replace("Kid", "");
            kidName = kidName.Replace("_", "");
            string profilePath = "Kids/Profiles/";

            kidProfileImage.sprite = Resources.Load<Sprite>(profilePath + kidName);
        }

        // Fisher-Yates Shuffle 알고리즘으로 카드 섞기
        private void Shuffle()
        {
            Sprite[] spriteShuffle = new Sprite[cardSprites.Length * 2];
            for (int i = 0; i < spriteShuffle.Length; i++)
            {
                spriteShuffle[i] = cardSprites[i / 2];
            }

            // 섞기
            for (int i = spriteShuffle.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);

                (spriteShuffle[i], spriteShuffle[j]) = (spriteShuffle[j], spriteShuffle[i]);
            }

            // 나누기
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].SetCardImg(spriteShuffle[i]);
            }
        }

        IEnumerator showCard(MiniGame_Card card)
        {
            card.Flip();
            yield return new WaitForSeconds(2f);
            card.Flip();
        }

        void CardPick(MiniGame_Card card)
        {
            if (!isPlayerTurn) return;

            Picking(card);
        }

        void Picking(MiniGame_Card card)
        {
            if (answerCards.Contains(card)) return;
            if (flipCard > 1) return;
            if (selectedCards.Contains(card)) return;

            flipCard++;
            card.Flip();
            AIRememberCard(card);
            selectedCards.Add(card);

            if (flipCard == 2)
            {
                StartCoroutine(CheckMatch());
            }
        }

        IEnumerator CheckMatch()
        {
            isChecking = true;
            yield return new WaitForSeconds(0.6f);
            if (selectedCards.Count < 2)
            {
                isChecking = false;
                yield break;
            }

            if (selectedCards[0].GetCardName() == selectedCards[1].GetCardName() &&
                selectedCards[0] != selectedCards[1])
            {
                for (int i = 0; i < selectedCards.Count; i++)
                {
                    answerCards.Add(selectedCards[i]);
                    rememberCards.Remove(selectedCards[i]);
                }

                TurnSetting(true, isPlayerTurn);
            }
            else
            {
                TurnSetting(false, isPlayerTurn);
            }

            isChecking = false;
        }

        void TurnSetting(bool isRight, bool isPlayer)
        {
            if (isRight)
            {
                if (isPlayer)
                {
                    playerScore++;
                    playerScoreTxt.text = playerScore.ToString();
                }
                else
                {
                    kidScore++;
                    kidScoreTxt.text = kidScore.ToString();
                    AIWork = true;
                }

                timer = turnTime;
                flipCard = 0;
                selectedCards.Clear();
                return;
            }

            isPlayerTurn = !isPlayer;
            AIWork = !isPlayerTurn;
            flipCard = 0;
            for (int i = 0; i < selectedCards.Count; i++)
                selectedCards[i].Flip();
            selectedCards.Clear();
            kidTurnPanel.SetActive(!isPlayerTurn);
            playerTurnPanel.SetActive(isPlayerTurn);
            kidTurnTxt.gameObject.SetActive(!isPlayerTurn);
            playerTurnTxt.gameObject.SetActive(isPlayerTurn);
            timer = turnTime;
        }

        IEnumerator KidCardAI()
        {
            yield return new WaitForSeconds(0.5f);
            float waitSec = 0;
            switch (AILevel)
            {
                case CardMiniGameLevel.EASY:
                    waitSec = Random.Range(2f, 4f);
                    break;
                case CardMiniGameLevel.NORMAL:
                    waitSec = Random.Range(1f, 4.5f);
                    break;
                case CardMiniGameLevel.HARD:
                    waitSec = Random.Range(0.5f, 2f);
                    break;
            }

            MiniGame_Card cardFair1 = null;
            MiniGame_Card cardFair2 = null;
            if (AIFindFair(out cardFair1, out cardFair2))
            {
                Picking(cardFair1);
                yield return new WaitForSeconds(waitSec);
                Picking(cardFair2);
            }
            else
            {
                List<MiniGame_Card> cardList = new List<MiniGame_Card>();
                foreach (MiniGame_Card card in cards)
                {
                    if (answerCards.Contains(card))
                        continue;
                    cardList.Add(card);
                }

                int randomIndex1 = Random.Range(0, cardList.Count);
                ;
                cardFair1 = cardList[randomIndex1];
                cardList.RemoveAt(randomIndex1);
                int randomIndex2 = Random.Range(0, cardList.Count);
                cardFair2 = cardList[randomIndex2];
                Picking(cardFair1);
                yield return new WaitForSeconds(waitSec);
                Picking(cardFair2);
            }
        }

        private bool AIFindFair(out MiniGame_Card a, out MiniGame_Card b)
        {
            foreach (var remember in rememberCards)
            {
                foreach (var another in rememberCards)
                {
                    if (remember == another)
                        continue;
                    if (another.GetCardName() == remember.GetCardName())
                    {
                        a = remember;
                        b = another;
                        rememberCards.Remove(remember);
                        rememberCards.Remove(another);
                        return true;
                    }
                }
            }

            a = null;
            b = null;
            return false;
        }

        private void AIRememberCard(MiniGame_Card card)
        {
            float rate = 0;
            switch (AILevel)
            {
                case CardMiniGameLevel.EASY:
                    rate = easyRate;
                    break;
                case CardMiniGameLevel.NORMAL:
                    rate = normalRate;
                    break;
                case CardMiniGameLevel.HARD:
                    rate = hardRate;
                    break;
            }

            if (Random.value < rate)
            {
                rememberCards.Add(card);
            }
        }

        public void StartGame()
        {
            StartCoroutine(StartRoutine());
        }

        IEnumerator StartRoutine()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                StartCoroutine(showCard(cards[i]));
                AIRememberCard(cards[i]);
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(3.5f);
            isPlaying = true;
            kidTurnPanel.SetActive(!isPlayerTurn);
            playerTurnPanel.SetActive(isPlayerTurn);
            kidTurnTxt.gameObject.SetActive(!isPlayerTurn);
            playerTurnTxt.gameObject.SetActive(isPlayerTurn);
            AIWork = true;
        }

        private void Win()
        {
            isPlaying = false;
            gameBox.SetActive(false);
            resultBox.SetActive(true);
            resultKidScoreTxt.text = kidScore.ToString();
            resultPlayerScoreTxt.text = playerScore.ToString();
            resultTxt.text = "게임에서 이겼습니다!";
            resultTxt.color = Color.blue;
            resultExplainTxt.text = $"정산시 보상금이 {rewardMoney}원 추가됩니다.";
            InGameCore.ROUND.GetMiniGameMoney(rewardMoney);
            InGameCore.ROUND.SetGameResult(true);
        }

        private void Lose()
        {
            isPlaying = false;
            gameBox.SetActive(false);
            resultBox.SetActive(true);
            resultKidScoreTxt.text = kidScore.ToString();
            resultPlayerScoreTxt.text = playerScore.ToString();
            resultTxt.text = "게임에서 졌습니다...";
            resultTxt.color = Color.red;
            resultExplainTxt.text = "스트레스 수치에서 1 증가합니다.";
            InGameCore.ROUND.IncreaseStress();
            InGameCore.ROUND.SetGameResult(false);
        }

        private void Draw()
        {
            isPlaying = false;
            gameBox.SetActive(false);
            resultBox.SetActive(true);
            resultKidScoreTxt.text = kidScore.ToString();
            resultPlayerScoreTxt.text = playerScore.ToString();
            resultTxt.text = "비겼습니다.";
            resultTxt.color = Color.black;
            resultExplainTxt.text = "아무런 보상을 획득하지 않습니다";
        }

        public void ExitGame()
        {
            InGameCore.ROUND.SetMiniGamePlaying(false);
            Destroy(gameObject);
        }
    }
}