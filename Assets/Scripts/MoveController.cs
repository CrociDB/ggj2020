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
    private Vector3 m_TargetPosition;

    public void Update()
    {
        m_TargetPosition = transform.position + m_Camera.transform.forward * m_DistanceTargetMoving;
        UpdateLine();

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
            m_SelectedObject.UpdateTargetPosition(m_TargetPosition, m_MovingForce);
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

    private void UpdateLine()
    {
        int amount = 20;
        m_LineRenderer.enabled = m_SelectedObject != null && m_MovingObject;

        List<Vector3> positions = new List<Vector3>();
        if (m_SelectedObject && m_MovingObject)
        {
            Vector3[] points = new Vector3[]
            {
                transform.position
                    - (m_TargetPosition - transform.position).normalized * 1.3f
                    - (m_SelectedObject.transform.position - transform.position).normalized * 4.0f,
                transform.position,
                Vector3.Lerp(transform.position, m_TargetPosition, .8f + (Mathf.Sin(Time.time * 2.0f) * .3f + .1f)),
                (m_TargetPosition + m_SelectedObject.transform.position) * (.5f + (Mathf.Sin(Time.time * .5f) * .02f)),
                m_SelectedObject.transform.position,
                m_SelectedObject.transform.position
                    + (m_SelectedObject.transform.position - m_TargetPosition).normalized * 2.0f
                    + (m_SelectedObject.transform.position - transform.position).normalized * 3.0f,
            };

            var lastPoint = transform.position;
            var currentPoint = points[0];
            var nextPoint = points[1];
            var nextNextPoint = points[2];

            for (int j = 0; j < points.Length - 2; j++)
            {
                currentPoint = points[j];
                nextPoint = points[(j + 1)];
                nextNextPoint = points[(j + 2)];

                for (int i = 1; i < amount; i++)
                {
                    float t = (float)i / (float)amount;
                    positions.Add(GetCatmullRomPosition(t, lastPoint, currentPoint, nextPoint, nextNextPoint));
                }

                lastPoint = currentPoint;
            }
        }

        m_LineRenderer.positionCount = positions.Count;
        m_LineRenderer.SetPositions(positions.ToArray());
    }

    private void OnDrawGizmos()
    {
        if (m_SelectedObject && m_MovingObject)
        {
            Vector3[] points = new Vector3[]
            {
                transform.position
                    - (m_TargetPosition - transform.position).normalized * 1.3f
                    - (m_SelectedObject.transform.position - transform.position).normalized * 4.0f,
                transform.position,
                Vector3.Lerp(transform.position, m_TargetPosition, .8f + (Mathf.Sin(Time.time * 2.0f) * .3f + .1f)),
                (m_TargetPosition + m_SelectedObject.transform.position) * (.5f + (Mathf.Sin(Time.time * .5f) * .02f)),
                m_SelectedObject.transform.position,
                m_SelectedObject.transform.position
                    + (m_SelectedObject.transform.position - m_TargetPosition).normalized * 2.0f
                    + (m_SelectedObject.transform.position - transform.position).normalized * 3.0f,
            };

            for (int j = 0; j < points.Length; j++)
            {
                Gizmos.DrawSphere(points[j], 0.2f);
            }
        }
    }

    private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

        return pos;
    }
}
