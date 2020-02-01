using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    private Objective m_Goal;

    private void Awake()
    {
        m_Goal = FindObjectOfType<Objective>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Goal.Win();
        }
    }
}
