using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace YEONJI.Kindergarten
{
    public class POPUP_RoundAlert : UIBase
    {
        public override UIType UIType { get { return UIType.RoundAlert; } }
        [SerializeField] private AnimationCurve sweepCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public RectTransform startImgRect;
        public RectTransform endImgRect;


        public void StartRound()
        {
            StartCoroutine(SweepRoutine(startImgRect));
        }

        public void EndRound()
        {
            StartCoroutine(SweepRoutine(endImgRect));
        }

        IEnumerator SweepRoutine(RectTransform rect)
        {
            float startX = 1500f;
            float endX = -2500f;
            float duration = 3f;
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                float e = sweepCurve.Evaluate(t);
                float x = Mathf.Lerp(startX, endX, e);
                rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
                yield return null;
            }
            rect.anchoredPosition = new Vector2(endX, rect.anchoredPosition.y);
            CloseUI();
        }
    }
}
