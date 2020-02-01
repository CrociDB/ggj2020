using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using UnityEngine.UI;
using System;
using UnityEngine.Rendering.PostProcessing;

public class CameraConstantShake : MonoBehaviour
{
    [Header("Tweak")]
    public float m_MaxDist;
    public float m_Interval;
    [RangeAttribute(0.0f, 1.0f)]
    public float m_Amount;

    private float m_Time;

    private Camera m_Camera;

    private ColorGrading m_EffectColorGrading;

    private Tween m_Tween;

    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
        m_Camera.gameObject.GetComponent<PostProcessVolume>().profile.TryGetSettings<ColorGrading>(out m_EffectColorGrading);
        FadeIn();
    }

    public void FadeIn()
    {
        if (m_Tween != null) m_Tween.Kill();
        m_Tween = DOTween.To(() => m_EffectColorGrading.postExposure.value, x => m_EffectColorGrading.postExposure.value = x, 0.0f, 1.5f)
                         .From(8.0f)
                         .SetEase(Ease.OutQuad);
    }

    public void FadeOut()
    {
        if (m_Tween != null) m_Tween.Kill();
        m_Tween = DOTween.To(() => m_EffectColorGrading.postExposure.value, x => m_EffectColorGrading.postExposure.value = x, 8.0f, .3f)
                         .From(0.0f)
                         .SetEase(Ease.OutQuad);
    }

    public void ShakeFor(float duration)
    {
        m_Camera.DOShakeRotation(duration, 7.0f, 90);
    }

    public void Update()
    {
        m_Time += Time.deltaTime;
        if (m_Time >= m_Interval)
        {
            m_Time = 0.0f;

            var rotation = Vector3.zero;
            rotation.x = UnityEngine.Random.Range(0.0f, m_MaxDist) * m_Amount;
            rotation.y = UnityEngine.Random.Range(0.0f, m_MaxDist) * m_Amount;

            transform.localEulerAngles = rotation;
        }
    }
}
