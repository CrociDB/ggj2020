using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveController : MonoBehaviour
{
    [Header("References")]
    public Camera m_Camera;
    public Image m_AimImage;
    public LineRenderer m_LineRenderer;

    [Header("Tweak values")]
    public float m_RaycastDistance = 20.0f;
    public float m_DistanceTargetMoving = 5.0f;
    public float m_MovingForce;

    private MovableObject m_SelectedObject;
    private bool m_MovingObject;

    public void Update()
    {
        var targetPosition = transform.position + m_Camera.transform.forward * m_DistanceTargetMoving;
        UpdateLine(targetPosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (m_SelectedObject != null)
            {
                m_SelectedObject.SelectObject();
                m_MovingObject = true;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (m_SelectedObject != null)
            {
                m_SelectedObject.UnselectObject();
            }

            m_MovingObject = false;
        }

        if (m_MovingObject)
        {
            m_SelectedObject.UpdateTargetPosition(targetPosition, m_MovingForce);
        }
        else
        {
            Ray r = new Ray(m_Camera.transform.position, m_Camera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, m_RaycastDistance, (1 << 9)))
            {
                m_AimImage.color = Color.red;
                m_SelectedObject = hit.collider.gameObject.GetComponent<MovableObject>();
            }
            else
            {
                m_AimImage.color = Color.white;
                m_SelectedObject = null;
            }
        }
    }

    private void UpdateLine(Vector3 targetPosition)
    {
        m_LineRenderer.enabled = m_SelectedObject != null && m_MovingObject;
        if (m_SelectedObject && m_MovingObject)
        {
            m_LineRenderer.SetPosition(0, transform.position);
            m_LineRenderer.SetPosition(1, targetPosition);
            m_LineRenderer.SetPosition(2, m_SelectedObject.transform.position);
        }
    }
}
