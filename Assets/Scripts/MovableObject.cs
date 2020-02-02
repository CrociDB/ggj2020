using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Renderer))]
public class MovableObject : MonoBehaviour
{
    public Material m_SelectedMaterial;
    public Material m_DefaultMaterial;

    [Range(0.1f, 5.0f)]
    public float m_MinScale;
    [Range(0.1f, 5.0f)]
    public float m_MaxScale;

    public bool m_Single = false;

    public float m_CurrentScale;
    private float m_TargetScale;

    private Rigidbody m_Rigidbody;
    private MeshFilter m_MeshFilter;
    private Renderer m_Renderer;


    private bool m_Moving;
    private bool m_Scaling;
    private Vector3 m_TargetPosition;
    private float m_MovingForce;
    private float m_DefaultMass;

    public Rigidbody Rigidbody
    {
        get
        {
            return m_Rigidbody;
        }
    }

    private void OnEnable()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_MeshFilter = GetComponent<MeshFilter>();
        m_Renderer = GetComponent<Renderer>();

        m_DefaultMass = m_Rigidbody.mass;

        m_TargetScale = m_CurrentScale;
    }

    public void Update()
    {
        if (m_Moving)
        {
            UpdatePosition();
        }

        if (m_Scaling)
        {
            UpdateScale();
        }
    }

    private void UpdateScale()
    {
        m_CurrentScale = Mathf.Clamp(
                Mathf.Lerp(m_CurrentScale, m_TargetScale, 8.0f * Time.deltaTime),
                m_MinScale,
                m_MaxScale);

        transform.localScale = Vector3.one * m_CurrentScale;
        m_Rigidbody.mass = m_DefaultMass * m_CurrentScale;
    }

    public void ResetScale()
    {
        m_TargetScale = m_CurrentScale = 1.0f;

        transform.localScale = Vector3.one * m_CurrentScale;
        m_Rigidbody.mass = m_DefaultMass * m_CurrentScale;
    }

    private void UpdatePosition()
    {
        var dir = m_TargetPosition - transform.position;
        m_Rigidbody.AddForce(dir * m_MovingForce);
    }


    public void SelectObjectScale()
    {
        m_Renderer.material = m_SelectedMaterial;
        m_Rigidbody.useGravity = false;
        m_Scaling = true;
    }

    public void UnselectObjectScale()
    {
        m_Renderer.material = m_DefaultMaterial;
        m_Rigidbody.useGravity = true;
        m_Scaling = false;
    }

    public void SelectObjectMove()
    {
        m_Renderer.material = m_SelectedMaterial;
        m_Rigidbody.useGravity = false;
        m_Moving = true;
    }

    public void UnselectObjectMove()
    {
        m_Renderer.material = m_DefaultMaterial;
        m_Rigidbody.useGravity = true;
        m_Moving = false;
    }


    public void UpdateTargetPosition(Vector3 targetPosition, float force)
    {
        m_TargetPosition = targetPosition;
        m_MovingForce = force;
    }

    public void UpdateTargetScale(float scale)
    {
        m_TargetScale = scale;
    }
}
