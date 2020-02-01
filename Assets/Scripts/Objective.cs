using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class Objective : MonoBehaviour
{
    [Header("References")]
    public GameObject m_Model;

    [Header("Tweak Values")]
    public float m_FloatAmount;
    public float m_FloatSpeed;
    public float m_RotateSpeed;

    [Header("Game Flow")]
    public string m_NextLevel;

    private PlayerController m_Player;

    private void Awake()
    {
        m_Player = FindObjectOfType<PlayerController>();
    }

    public void Update()
    {
        var rs = m_RotateSpeed + (1.0f - Mathf.Clamp01((transform.position - m_Player.transform.position).sqrMagnitude / 600.0f)) * m_RotateSpeed * 8.0f;

        m_Model.transform.Rotate(Vector3.up * rs);
        m_Model.transform.localPosition = Vector3.up * Mathf.Sin(Time.time * m_FloatSpeed) * m_FloatAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController p;
        if (other.gameObject.TryGetComponent<PlayerController>(out p) && p == m_Player)
        {
            Debug.Log("YOU WON");
            GetComponent<Collider>().enabled = false;
            m_Player.CameraShake.FadeOut();
            Invoke("NextLevel", .4f);
        }
    }

    private void NextLevel()
    {
        SceneManager.LoadScene(m_NextLevel);
    }
}
