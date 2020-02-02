using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class MoveController : MonoBehaviour
{
    [Header("References")]
    public Camera m_Camera;
    public CameraConstantShake m_CameraShake;
    public Image m_AimImage;
    public LineRenderer m_LineRenderer;
    public PostProcessProfile m_PostProfile;

    [Header("Prefabs")]
    public ParticleSystem m_DuplicateFX;

    [Header("Tweak values")]
    public float m_RaycastDistance = 20.0f;
    public float m_DistanceTargetMoving = 5.0f;
    public float m_MovingForce;

    private PlayerController m_Player;

    private MovableObject m_SelectedObject;
    private bool m_MovingObject;
    private bool m_ScalingObject;
    private Vector3 m_TargetPosition;

    private float m_ScaleAmount;

    private ChromaticAberration m_EffectChromaticAberration;
    private Vignette m_EffectVignette;

    public void Awake()
    {
        m_Player = GetComponent<PlayerController>();
        m_ScaleAmount = 0.3f;

        m_PostProfile.TryGetSettings(out m_EffectChromaticAberration);
        m_PostProfile.TryGetSettings(out m_EffectVignette);
    }

    public void Update()
    {
        m_TargetPosition = transform.position + m_Camera.transform.forward * m_DistanceTargetMoving;

        UpdateLine();
        UpdateCamera();

        // Move
        if (Input.GetMouseButtonDown(0))
        {
            if (m_SelectedObject != null)
            {
                m_SelectedObject.SelectObjectMove();
                m_MovingObject = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (m_SelectedObject != null)
            {
                m_SelectedObject.UnselectObjectMove();
            }

            m_MovingObject = false;
        }

        // Scale
        if (Input.GetMouseButtonDown(1))
        {
            if (m_SelectedObject != null)
            {
                m_SelectedObject.SelectObjectScale();
                m_ScalingObject = true;
                m_Player.LockLook();
                m_ScaleAmount = Mathf.InverseLerp(m_SelectedObject.m_MinScale, m_SelectedObject.m_MaxScale, m_SelectedObject.m_CurrentScale);
            }
        }
       else if (Input.GetMouseButtonUp(1))
        {
            if (m_SelectedObject != null)
            {
                m_SelectedObject.UnselectObjectScale();
            }

            m_ScaleAmount = 0.3f;
            m_Player.UnlockLook();
            m_ScalingObject = false;
        }

        if (m_MovingObject)
        {
            m_SelectedObject.UpdateTargetPosition(m_TargetPosition, m_MovingForce);
        }
        else if (m_ScalingObject)
        {
            UpdateScaling();
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

    private void UpdateCamera()
    {
        m_CameraShake.m_Amount = 0.0f;

        if (m_ScalingObject)
        {
            m_Camera.fieldOfView = Mathf.Lerp(
                m_Camera.fieldOfView,
                Mathf.Lerp(40.0f, 120.0f, m_ScaleAmount),
                10.0f * Time.deltaTime);
            
            m_CameraShake.m_Amount = Mathf.Clamp01((m_ScaleAmount - .1f) / .9f);

        }
        else if (m_MovingObject)
        {
            m_Camera.fieldOfView = Mathf.Lerp(
                m_Camera.fieldOfView,
                92.0f,
                10.0f * Time.deltaTime);
        
            m_CameraShake.m_Amount = 0.04f;
        }

        m_Camera.fieldOfView = Mathf.Lerp(
                m_Camera.fieldOfView,
                60.0f,
                10.0f * Time.deltaTime);

        var normalized = Mathf.Clamp01((m_ScaleAmount - .3f) / .7f);

        m_EffectChromaticAberration.intensity.value = normalized;
        m_EffectVignette.intensity.value = Mathf.Clamp01(m_ScaleAmount) * .5f;
    }

    private void UpdateScaling()
    {
        var y = Input.GetAxisRaw("Mouse Y");

        m_ScaleAmount -= y * 0.04f;
        m_SelectedObject.UpdateTargetScale(Mathf.Lerp(m_SelectedObject.m_MinScale, m_SelectedObject.m_MaxScale, m_ScaleAmount));

        if (m_ScaleAmount >= 1.0f && !m_SelectedObject.m_Single)
        {
            DuplicateObject();

            if (m_SelectedObject != null)
            {
                m_SelectedObject.UnselectObjectScale();
            }

            m_ScaleAmount = 0.3f;
            m_Player.UnlockLook();
            m_ScalingObject = false;

            m_SelectedObject = null;
        }
    }

    private void DuplicateObject()
    {
        var fx = Instantiate(m_DuplicateFX);
        fx.transform.position = m_SelectedObject.transform.position;

        m_SelectedObject.ResetScale();

        var copy = Instantiate(m_SelectedObject);
        copy.ResetScale();
        m_SelectedObject.transform.position -= transform.right;
        copy.transform.position += transform.right;

        m_SelectedObject.UnselectObjectScale();
        copy.UnselectObjectScale();

        m_SelectedObject.Rigidbody.AddForce(transform.right * -2.0f + Vector3.up * 3.0f, ForceMode.Impulse);
        copy.Rigidbody.AddForce(transform.right * 2.0f + Vector3.up * 3.0f, ForceMode.Impulse);

        m_CameraShake.ShakeFor(0.3f);
    }

    private void UpdateLine()
    {
        int amount = 25;
        m_LineRenderer.enabled = m_SelectedObject != null && m_MovingObject;

        List<Vector3> positions = new List<Vector3>();
        if (m_SelectedObject && m_MovingObject)
        {
            Vector3[] points = new Vector3[]
            {
                transform.position
                    - (m_TargetPosition - transform.position).normalized * 0.4f
                    - Vector3.Dot(m_SelectedObject.transform.position - transform.position, transform.right) * transform.right * 2.0f,
                transform.position,
                Vector3.Lerp(transform.position, m_TargetPosition, .8f + (Mathf.Sin(Time.time * 2.0f) * .2f)),
                (m_TargetPosition + m_SelectedObject.transform.position) * (.5f + (Mathf.Sin(Time.time * 2.5f) * .02f)),
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
