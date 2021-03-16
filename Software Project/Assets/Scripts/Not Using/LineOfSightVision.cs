using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class LineOfSightVision : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private Mesh mesh;
    private Vector3 origin;
    private float los;
    private float startingAngles;

    private float startingAngle = 90;
    RaycastHit hit;

    public static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRadious = angle * (Mathf.PI / 180f);

        return new Vector3(Mathf.Cos(angleRadious), Mathf.Sin(angleRadious));
    }

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        los = 90f;
        origin = transform.position;mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        los = 90f;
        origin = transform.position;
    }

    private void LateUpdate()
    {
        int rayCount = 3000;
        float angle = startingAngle;
        float angleIncrease = los / rayCount;
        float viewDistance = 10f;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];
        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit hit;

            if (Physics.Raycast(origin, GetVectorFromAngle(angle), out hit, viewDistance, layerMask))
            {
                vertex = hit.point;
            }
            else
            {
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            }

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}