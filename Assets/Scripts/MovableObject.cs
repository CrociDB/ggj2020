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

    private Rigidbody m_Rigidbody;
    private MeshFilter m_MeshFilter;
    private Renderer m_Renderer;

    private Material m_DefaultMaterial;

    private bool m_Moving;
    private Vector3 m_TargetPosition;
    private float m_MovingForce;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_MeshFilter = GetComponent<MeshFilter>();
        m_Renderer = GetComponent<Renderer>();

        m_DefaultMaterial = m_Renderer.material;
    }

    public void Update()
    {
        if (m_Moving)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        var dir = m_TargetPosition - transform.position;
        m_Rigidbody.AddForce(dir * m_MovingForce);
    }

    public void SelectObject()
    {
        m_Renderer.material = m_SelectedMaterial;
        m_Rigidbody.useGravity = false;
        m_Moving = true;
    }

    public void UnselectObject()
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
}
