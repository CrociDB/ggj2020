using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Wall m_Wall;

    private SpringJoint m_Joint;

    private void Awake()
    {
        m_Joint = GetComponentInChildren<SpringJoint>();
    }

    public void Update()
    {
        m_Wall.m_Open = m_Joint.currentForce.magnitude >= 0.93f;
    }
}
