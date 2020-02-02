using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [Header("References")]
    public Transform m_MovingPart;

    [Header("Tweaks")]
    public float m_OpenPosition;
    public float m_ClosedPosition;

    [Header("Movement")]
    public bool m_Open = false;

    public void Update()
    {
        var pos = m_MovingPart.localPosition;
        if (m_Open)
        {
            pos.z = Mathf.Lerp(pos.z, m_OpenPosition, 5.0f * Time.deltaTime);
        }
        else
        {
            pos.z = Mathf.Lerp(pos.z, m_ClosedPosition, 5.0f * Time.deltaTime);
        }
        m_MovingPart.localPosition = pos;
    }
}
