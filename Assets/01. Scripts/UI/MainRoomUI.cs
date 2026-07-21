using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Infos;

public class MainRoomUI : MonoBehaviour
{
    [Header("Status Panel")]
    [SerializeField] private RectTransform statusBoxRT;
    [SerializeField] private GameObject kidStatus;
    [SerializeField] private Image kidsProfileImg;
    [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private Vector2 onPos = new Vector2(-900, 10);
    private Vector2 offPos = new Vector2(-80, 10);
    private float duration = 0.3f;
    private string profilePath = "Kids/Profiles/";
    private Coroutine slideRoutine;
    
    [Header("Status Slider")]
    [SerializeField] private Slider hungryStatus;
    [SerializeField] private Slider toiletStatus;
    [SerializeField] private Slider sleepStatus;
    [SerializeField] private Image hungryFillArea;
    [SerializeField] private Image toiletFillArea;
    [SerializeField] private Image sleepFillArea;
    [SerializeField] private Sprite goodStatusSprite;
    [SerializeField] private Sprite normalStatusSprite;
    [SerializeField] private Sprite badStatusSprite;
    
    [Header("타이머 용")]
    [SerializeField] private Image hungryTimeImg;
    [SerializeField] private Image toiletTimeImg;
    [SerializeField] private Image sleepTimeImg;

    public void InitUI()
    {
        statusBoxRT.anchoredPosition = offPos;

    }

    public void SettingStatusBox(NeedType type, NeedLevel level, string kidsName)
    {
        SetProfileImg(kidsName);
        SetStatusSlider(type, level);
        SetStatusFillArea(type, level);
    }

    public void StatusBoxSliding(bool show)
    {
        Vector2 target = show ? onPos : offPos;
        
        if (slideRoutine != null) StopCoroutine(slideRoutine);
        slideRoutine = StartCoroutine(SlideTo(target));
    }

    private IEnumerator SlideTo(Vector2 target)
    {

        Vector2 start = statusBoxRT.anchoredPosition;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float e = ease.Evaluate(t);
            statusBoxRT.anchoredPosition = Vector2.Lerp(start, target, e);
            yield return null;
        }
        statusBoxRT.anchoredPosition = target;
    }

    private void SetProfileImg(string kidsName)
    {
        
        string fileName = kidsName + "_profile";
        Debug.Log(profilePath + fileName);
        Sprite profileSprite = Resources.Load<Sprite>(profilePath + fileName);
        kidsProfileImg.sprite = profileSprite;
    }

    private void SetStatusSlider(NeedType type, NeedLevel level)
    {
        switch (type)
        {
            case NeedType.Hunger:
                hungryStatus.value = (int)level;
                break;
            case NeedType.Toilet:
                toiletStatus.value = (int)level;
                break;
            case NeedType.Sleep:
                sleepStatus.value = (int)level;
                break;
        }
    }

    private void SetStatusFillArea(NeedType type, NeedLevel level)
    {
        Sprite barSprite;
        if (level >= NeedLevel.Good)
            barSprite = goodStatusSprite;
        else if (level >= NeedLevel.Bad)
            barSprite = normalStatusSprite;
        else
            barSprite = badStatusSprite;
        
        switch (type)
        {
            case NeedType.Hunger:
                hungryFillArea.sprite = barSprite;
                break;
            case NeedType.Toilet:
                toiletFillArea.sprite = barSprite;
                break;
            case NeedType.Sleep:
                sleepFillArea.sprite = barSprite;
                break;
        }
    }

}

