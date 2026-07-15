using System.Collections;
using UnityEngine;

// ────────────────────────────────────────────────────────────
// 유치원생 머리 위 원형 타이머
//   SpriteRenderer + SpriteRadialFill 셰이더로 동작.
//   MaterialPropertyBlock을 써서 머티리얼 인스턴스 생성을 피한다.
// ────────────────────────────────────────────────────────────

[RequireComponent(typeof(SpriteRenderer))]
public class RadialTimer : MonoBehaviour
{
    private static readonly int FillAmountID = Shader.PropertyToID("_FillAmount");

    private SpriteRenderer sr;
    private MaterialPropertyBlock mpb;
    private Coroutine running;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
        SetFill(1f);
        gameObject.SetActive(false);   // 평소엔 꺼둠
    }

    // 채우기 값 설정 (0 = 빈 상태, 1 = 꽉 참)
    public void SetFill(float value)
    {
        sr.GetPropertyBlock(mpb);
        mpb.SetFloat(FillAmountID, Mathf.Clamp01(value));
        sr.SetPropertyBlock(mpb);
    }

    // duration 초 동안 1 → 0 으로 줄어드는 타이머 시작
    public void StartTimer(float duration, System.Action onComplete = null)
    {
        gameObject.SetActive(true);
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(TimerRoutine(duration, onComplete));
    }

    public void StopTimer()
    {
        if (running != null) StopCoroutine(running);
        running = null;
        gameObject.SetActive(false);
    }

    private IEnumerator TimerRoutine(float duration, System.Action onComplete)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetFill(1f - (elapsed / duration));   // 1에서 0으로
            yield return null;
        }

        SetFill(0f);
        running = null;
        onComplete?.Invoke();
        gameObject.SetActive(false);
    }
}
