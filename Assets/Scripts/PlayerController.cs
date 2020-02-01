using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Tweak Values")]
    public float m_Speed = 3.0f;
    public float m_LookSensitivity = 3.0f;
    public float m_LookSmoothRate = 15.0f;
    public float m_JumpForce = 2.0f;
    
    [Header("References")]
    public Transform m_CameraTransform;

    private Vector3 m_MoveHorizontal;
    private Vector3 m_MovVertical;
    private Vector3 m_Velocity;
    private Rigidbody m_Rigid;
    private Vector3 m_Rotation;
    private Vector3 m_CameraRotation;
    private bool m_CursorIsLocked = true;
    private bool m_LookLock = false;

    public CameraConstantShake CameraShake { get; set; }

    private void Start()
    {
        m_Rigid = GetComponent<Rigidbody>();
        CameraShake = GetComponentInChildren<CameraConstantShake>();
    }

    public void Update()
    {
        var movX = Input.GetAxis("Horizontal");
        var movY = Input.GetAxis("Vertical");

        m_MoveHorizontal = transform.right * movX;
        m_MovVertical = transform.forward * movY;

        m_Velocity = (m_MoveHorizontal + m_MovVertical) * m_Speed;

        if (m_CursorIsLocked && !m_LookLock)
        {
            var rot = Input.GetAxisRaw("Mouse X");
            m_Rotation = Vector3.Lerp(m_Rotation, new Vector3(0, rot, 0) * m_LookSensitivity, m_LookSmoothRate * Time.deltaTime);

            rot = Input.GetAxisRaw("Mouse Y");
            m_CameraRotation = Vector3.Lerp(m_CameraRotation, new Vector3(rot, 0, 0) * m_LookSensitivity, m_LookSmoothRate * Time.deltaTime);

            if (m_Velocity != Vector3.zero)
            {
                m_Rigid.MovePosition(m_Rigid.position + m_Velocity * Time.fixedDeltaTime);
            }

            if (m_Rotation != Vector3.zero)
            {
                m_Rigid.MoveRotation(m_Rigid.rotation * Quaternion.Euler(m_Rotation));
            }

            if (m_CameraTransform != null)
            {
                m_CameraTransform.transform.Rotate(-m_CameraRotation);
            }

            // Jump
            if (Input.GetKeyDown(KeyCode.Space) && NotFalling())
            {
                m_Rigid.AddForce(Vector3.up * m_JumpForce, ForceMode.Impulse);
            }
        }

        InternalLockUpdate();
    }

    private bool NotFalling()
    {
        Ray r = new Ray(transform.position, -Vector3.up);
        return Physics.Raycast(r, 1.2f, ~(1 << 8));
    }

    private void InternalLockUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            m_CursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_CursorIsLocked = true;
        }

        if (m_CursorIsLocked)
        {
            LockCursor();
        }
        else if (!m_CursorIsLocked)
        {
            UnlockCursor();
        }
#endif
    }

    public void LockLook()
    {
        m_LookLock = true;
    }

    public void UnlockLook()
    {
        m_LookLock = false;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}