using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Objective : MonoBehaviour
{
    [Header("References")]
    public GameObject m_Model;

    [Header("Tweak Values")]
    public float m_FloatAmount;
    public float m_FloatSpeed;
    public float m_RotateSpeed;

    public void Update()
    {
        m_Model.transform.Rotate(Vector3.up * m_RotateSpeed);
        m_Model.transform.localPosition = Vector3.up * Mathf.Sin(Time.time * m_FloatSpeed) * m_FloatAmount;
    }
}
