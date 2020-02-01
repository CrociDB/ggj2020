using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConstantShake : MonoBehaviour
{
    [Header("Tweak")]
    public float m_MaxDist;
    public float m_Interval;
    [RangeAttribute(0.0f, 1.0f)]
    public float m_Amount;

    private float m_Time;

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
