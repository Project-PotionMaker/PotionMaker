using DG.Tweening;
using UnityEngine;

public class LightColorByPhase : MonoBehaviour
{
    private Light targetLight;

    private Color dawnBlue = new Color(0.40f, 0.60f, 1.0f); // 새벽의 푸른빛
    private Color duskRed = new Color(1.00f, 0.70f, 0.10f); // 저녁의 붉은빛
    private Color darker = new Color(0.05f, 0.05f, 0.10f); // 저녁의 붉은빛
    private Color noonWhite = Color.white;                   // 정오(중간값) 흰색

    private Vector3 dawnEuler = new Vector3(40f, -90f, 0f);   // 동쪽에서 비스듬히 비추는 각도
    private Vector3 duskEuler = new Vector3(40f, 90f, 0f);  // 서쪽
    private Vector3 dipEuler = new Vector3(-80f, 180f, 0f); // 남쪽으로 기울어지는 각도
    private Vector3 noonEuler = new Vector3(80f, 0f, 0f); // 정오(중간값) 


    private Tween _colorTween;
    private const float DURATION = 2f;
    private void OnEnable()
    {
        if (targetLight == null)
        {
            targetLight = GetComponent<Light>();
        }
        PhaseManager.Instance.OnTimerRunning += OnTimerRunning;
        PhaseManager.Instance.OnDayPassed += DayChangeLight;

        // 처음 진입 시 현재 페이즈에 맞춰 즉시 적용
        ApplyPhaseImmediate();
    }

    private void OnDisable()
    {
        if (PhaseManager.Instance != null)
        {
            PhaseManager.Instance.OnTimerRunning -= OnTimerRunning;
            PhaseManager.Instance.OnDayPassed -= DayChangeLight;
        }
        KillTween();
    }

    // ===== 이벤트 핸들러 =====

    // 시간 흐름 틱(ServingPhase에서만 불린다고 가정)
    private void OnTimerRunning()
    {
        if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
        {
            float t = Mathf.Clamp01(PhaseManager.Instance.CurrentTimeRate);
            targetLight.color = EvaluateServingColor(t);
            UpdateServingRotation(t);
        }
    }
    private void UpdateServingRotation(float t01)
    {
        if (t01 > 0.5f)
        {
            float u = (t01-0.5f) / 0.5f;
            float pitchX = Mathf.LerpAngle(noonEuler.x, dawnEuler.x, u); // 고도↑
            float yawY = Mathf.LerpAngle(noonEuler.y, dawnEuler.y, u); // 90→180
            transform.localRotation = Quaternion.Euler(pitchX, yawY, 0f);
        }
        else
        {
            float u = t01 / 0.5f;
            float pitchX = Mathf.LerpAngle(duskEuler.x, noonEuler.x, u); // 고도↓
            float yawY = Mathf.LerpAngle(duskEuler.y, noonEuler.y, u); // 180→270
            transform.localRotation = Quaternion.Euler(pitchX, yawY, 0f);
        }
    }

    private void DayChangeLight()
    {
        KillTween();
        _colorTween = DOTween.Sequence()
            .Append(targetLight.DOColor(darker, DURATION * 0.5f).SetEase(Ease.InSine))
            .Join(transform.DORotate(dipEuler, DURATION * 0.5f, RotateMode.Fast).SetEase(Ease.InSine))
            .Append(targetLight.DOColor(dawnBlue, DURATION * 0.5f).SetEase(Ease.OutSine))
            .Join(transform.DORotate(dawnEuler, DURATION * 0.5f, RotateMode.Fast).SetEase(Ease.OutSine));
    }

    private void KillTween()
    {
        if (_colorTween != null && _colorTween.IsActive())
        {
            _colorTween.Kill();
        }
        _colorTween = null;
    }

    private Color EvaluateServingColor(float t01)
    {
        if (t01 > 0.7f)
        {
            // 0 ~ 0.5 : 새벽(푸른빛) → 화이트
            float u = (t01-0.7f) / 0.3f; // 0~1로 정규화
            return Color.Lerp(noonWhite, dawnBlue, u);
        }
        else if(t01<0.3f)
        {
            // 0.5 ~ 1 : 화이트 → 저녁(붉은빛)
            float u = t01 / 0.3f; // 0~1로 정규화
            return Color.Lerp(duskRed, noonWhite, u);
        }
        else
        {
            return Color.white;
        }
    }

    // 현재 페이즈 기준으로 즉시 색 적용
    private void ApplyPhaseImmediate()
    {
        switch (PhaseManager.Instance.CurrentPhase.PhaseType)
        {
            case EPhaseType.PreparingPhase:
                targetLight.color = dawnBlue;
                targetLight.transform.rotation = Quaternion.Euler(dawnEuler);
                break;
            case EPhaseType.EndingPhase:
                targetLight.color = duskRed;
                targetLight.transform.rotation = Quaternion.Euler(duskEuler);
                break;
        }
    }
}
