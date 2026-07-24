using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace YEONJI.Kindergarten
{
    public class MiniGame_HibiscusPlay : MonoBehaviour
    {
        [SerializeField] private GameObject startBox;
        [SerializeField] private GameObject explainBox;
        [SerializeField] private GameObject gameBox;
        [SerializeField] private GameObject resultBox;
        
        [Header("Game Field")]
        [SerializeField] private Sprite[] teacherSprites;
        [SerializeField] private TextMeshProUGUI gameTxt;
        [SerializeField] private Slider gameSlider;
        [SerializeField] private Image teacherImage;
        [SerializeField] private TextMeshProUGUI timeText;
        private int spriteIndex;
        private float playTime;
        private string fullText = "무궁화 꽃이 피었습니다";
        private float charTimer;
        private int charIndex;
        private bool isWaitChar;
        private bool isSafe;
        private bool isPlaying;
        
        [Header("Result Field")]
        [SerializeField] private TextMeshProUGUI resultTxt;
        [SerializeField] private TextMeshProUGUI resultExplainTxt;
        private int rewardMoney;
        
        private void Awake()
        {
            // 임시
            playTime = 30f;
            rewardMoney = 300;
            // ----
        }

        private void Start()
        {
            InGameCore.ROUND.SetMiniGamePlaying(true);
            startBox.SetActive(true);
            explainBox.SetActive(false);
            gameBox.SetActive(false);
            resultBox.SetActive(false);
            spriteIndex = 0;
            teacherImage.sprite = teacherSprites[spriteIndex];
            gameTxt.text = "";
        }

        private void Update()
        {
            if (!InGameCore.ROUND.isMiniGamePlaying) return;
            if (!isPlaying) return;
            
            playTime -= Time.deltaTime;
            timeText.text = playTime.ToString("0");
            if (playTime <= 0) Lose();
            PrintGameText();
            
    #if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                if (isSafe)
                {
                    gameSlider.value++;
                    SetTeacherImage();
                    if (gameSlider.value >= 100) Win();
                }
                else
                {
                    Lose();
                }
            }
    #elif UNITY_ANDROID        
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                     if (isSafe)
                    {
                        gameSlider.value++;
                        SetTeacherImage();
                        if (gameSlider.value >= 100) Win();
                    }
                    else
                    {
                        Lose();
                    }
                }
            }

    #endif
        }

        public void GamePlay()
        {
            isPlaying = true;
            isWaitChar = true;
        }
        

        private void PrintGameText()
        {
            charTimer -= Time.deltaTime;
            if (charTimer > 0) return;
            if (isWaitChar)
            {
                charIndex++;
                gameTxt.text = fullText.Substring(0, charIndex);
                isSafe = true;

                if (charIndex >= fullText.Length)
                {
                    isWaitChar = false;
                    isSafe = false;
                    charTimer = Random.Range(0.5f, 1.5f);
                }
                else
                {
                    charTimer = Random.Range(0.5f, 1f);
                }
            }
            else
            {
                charIndex = 0;
                charIndex++;
                gameTxt.text = fullText.Substring(0, charIndex);
                isSafe = true;
                isWaitChar = true;
                charTimer = Random.Range(0.5f, 1f);
            }
            
        }

        private void SetTeacherImage()
        {
            spriteIndex++;
            if (spriteIndex >= teacherSprites.Length) 
                spriteIndex = 0;
            teacherImage.sprite = teacherSprites[spriteIndex];
        }

        private void Lose()
        {
            isPlaying = false;
            gameBox.SetActive(false);
            resultBox.SetActive(true);
            resultTxt.text = "게임에서 졌습니다...";
            resultTxt.color = Color.red;
            resultExplainTxt.text = "스트레스 수치에서 1 증가합니다.";
            InGameCore.ROUND.IncreaseStress();
            InGameCore.ROUND.SetGameResult(false);
        }

        private void Win()
        {
            isPlaying = false;
            gameBox.SetActive(false);
            resultBox.SetActive(true);
            resultTxt.text = "게임에서 이겼습니다!";
            resultTxt.color = Color.blue;
            resultExplainTxt.text = $"정산시 보상금이 {rewardMoney}원 추가됩니다.";
            InGameCore.ROUND.GetMiniGameMoney(rewardMoney);
            InGameCore.ROUND.SetGameResult(true);
        }

        public void ExitGame()
        {
            InGameCore.ROUND.SetMiniGamePlaying(false);
            Destroy(gameObject);
        }

    }
}

