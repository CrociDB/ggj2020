using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
public class ChangeProjection : MonoBehaviour
{
    public Camera m_Camera;

    private MeshFilter m_Mesh;
    private MeshCollider m_MeshCollider;

    [Range(0.0f, 1.0f)]
    public float m_Amount = 1.0f;
    private float m_Scroll;

    private List<Vector3> vertices;

    public void Start()
    {
        m_Mesh = GetComponent<MeshFilter>();
        m_MeshCollider = GetComponent<MeshCollider>();
        m_MeshCollider.sharedMesh = m_Mesh.mesh;

        vertices = new List<Vector3>();
        m_Mesh.mesh.GetVertices(vertices);
    }

    public void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    ProjectIt();
        //}

        m_Scroll = Mathf.Clamp01(m_Scroll - Input.mouseScrollDelta.y * .05f);
        var a = Mathf.Lerp(60.0f, 120.0f, m_Scroll);
        //m_Camera.fieldOfView = a;
        ProjectIt(a);
    }

    private void ProjectIt(float val)
    {
        var matrix = Matrix4x4.Scale(Vector3.one * (1.0f + m_Scroll));

        Vector3[] v = new Vector3[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            v[i] = vertices[i];
            //v[i] = m_Camera.worldToCameraMatrix * vertices[i];
            v[i] = matrix * v[i];
            //v[i] = m_Camera.cameraToWorldMatrix * v[i];
        }

        m_Mesh.mesh.vertices = v;
        m_Mesh.mesh.RecalculateBounds();
        m_MeshCollider.sharedMesh = m_Mesh.mesh;
    }
}
