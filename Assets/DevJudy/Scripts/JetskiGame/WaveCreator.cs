using UnityEngine;
using System;

[DefaultExecutionOrder(-10)]
public class WaveCreator : MonoBehaviour
{
    private MeshCollider meshCollider;

    [SerializeField] private Transform playerTransform;

    //Public Properties
    [SerializeField] private int dimension = 10;
    [SerializeField] private float uvScale = 2f;
    [SerializeField] private Octave[] octaves;

    // Player
    private Vector3 previousPlayerPosition;
    [SerializeField] private float updateThreshold;

    //Mesh
    private MeshFilter meshFilter;
    private Mesh mesh;

    private int Index(int _x, int _z)
    {
        return _x * (dimension + 1) + _z;
    }

    private int Index(float _x, float _z)
    {
        return Index((int)_x, (int)_z);
    }

    [ContextMenu("Generate Mesh")]
    private void Awake()
    {
        //Mesh Setup
        mesh = new Mesh();

        mesh.name = gameObject.name;
        mesh.vertices = GenerateVerts();
        mesh.triangles = GenerateTries();
        mesh.uv = GenerateUVs();


        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
            meshCollider.sharedMesh = mesh;
    }

    // This is called by the buoyancy components
    public float GetHeight(Vector3 _position)
    {
        //scale factor and position in local space
        var scale = new Vector3(1 / transform.lossyScale.x, 0, 1 / transform.lossyScale.z);
        var localPos = Vector3.Scale((_position - transform.position), scale);

        //get edge points
        var p1 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Floor(localPos.z));
        var p2 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Ceil(localPos.z));
        var p3 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Floor(localPos.z));
        var p4 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Ceil(localPos.z));

        //clamp if the position is outside the plane
        p1.x = Mathf.Clamp(p1.x, 0, dimension);
        p1.z = Mathf.Clamp(p1.z, 0, dimension);
        p2.x = Mathf.Clamp(p2.x, 0, dimension);
        p2.z = Mathf.Clamp(p2.z, 0, dimension);
        p3.x = Mathf.Clamp(p3.x, 0, dimension);
        p3.z = Mathf.Clamp(p3.z, 0, dimension);
        p4.x = Mathf.Clamp(p4.x, 0, dimension);
        p4.z = Mathf.Clamp(p4.z, 0, dimension);

        //get the max distance to one of the edges and take that to compute max - dist
        var max = Mathf.Max(Vector3.Distance(p1, localPos), Vector3.Distance(p2, localPos), Vector3.Distance(p3, localPos),
            Vector3.Distance(p4, localPos) + Mathf.Epsilon);
        var dist = (max - Vector3.Distance(p1, localPos))
                   + (max - Vector3.Distance(p2, localPos))
                   + (max - Vector3.Distance(p3, localPos))
                   + (max - Vector3.Distance(p4, localPos) + Mathf.Epsilon);
        //weighted sum
        var height = mesh.vertices[Index(p1.x, p1.z)].y * (max - Vector3.Distance(p1, localPos))
                     + mesh.vertices[Index(p2.x, p2.z)].y * (max - Vector3.Distance(p2, localPos))
                     + mesh.vertices[Index(p3.x, p3.z)].y * (max - Vector3.Distance(p3, localPos))
                     + mesh.vertices[Index(p4.x, p4.z)].y * (max - Vector3.Distance(p4, localPos));

        //scale
        return height * transform.lossyScale.y / dist;
    }

    private Vector3[] GenerateVerts()
    {
        var verts = new Vector3[(dimension + 1) * (dimension + 1)];

        //equally distributed verts
        for (int x = 0; x <= dimension; x++)
        {
            for (int z = 0; z <= dimension; z++)
            {
                verts[Index(x, z)] = new Vector3(x, 0, z);
            }
        }

        return verts;
    }

    private int[] GenerateTries()
    {
        var tries = new int[mesh.vertices.Length * 6];

        //two triangles are one tile
        for (int x = 0; x < dimension; x++)
        {
            for (int z = 0; z < dimension; z++)
            {
                tries[Index(x, z) * 6 + 0] = Index(x, z);
                tries[Index(x, z) * 6 + 1] = Index(x + 1, z + 1);
                tries[Index(x, z) * 6 + 2] = Index(x + 1, z);
                tries[Index(x, z) * 6 + 3] = Index(x, z);
                tries[Index(x, z) * 6 + 4] = Index(x, z + 1);
                tries[Index(x, z) * 6 + 5] = Index(x + 1, z + 1);
            }
        }

        return tries;
    }

    private Vector2[] GenerateUVs()
    {
        var uvs = new Vector2[mesh.vertices.Length];

        //always set one uv over n tiles than flip the uv and set it again
        for (int x = 0; x <= dimension; x++)
        {
            for (int z = 0; z <= dimension; z++)
            {
                var vec = new Vector2((x / uvScale) % 2, (z / uvScale) % 2);
                uvs[Index(x, z)] = new Vector2(vec.x <= 1 ? vec.x : 2 - vec.x, vec.y <= 1 ? vec.y : 2 - vec.y);
            }
        }

        return uvs;
    }

    private void FixedUpdate()
    {
        CalculateWaves();
    }

    private void CalculateWaves()
    {
        var verts = mesh.vertices;
        for (int x = 0; x <= dimension; x++)
        {
            for (int z = 0; z <= dimension; z++)
            {
                var y = 0f;
                for (int o = 0; o < octaves.Length; o++)
                {
                    if (octaves[o].alternate)
                    {
                        var perl = Mathf.PerlinNoise((x * octaves[o].scale.x) / dimension, (z * octaves[o].scale.y) / dimension) * Mathf.PI * 2f;
                        y += Mathf.Cos(perl + octaves[o].speed.magnitude * Time.time) * octaves[o].height;
                    }
                    else
                    {
                        var perl = Mathf.PerlinNoise((x * octaves[o].scale.x + Time.time * octaves[o].speed.x) / dimension,
                            (z * octaves[o].scale.y + Time.time * octaves[o].speed.y) / dimension) - 0.5f;
                        y += perl * octaves[o].height;
                    }
                }

                verts[Index(x, z)] = new Vector3(x, y, z);
            }
        }

        mesh.vertices = verts;
        mesh.RecalculateNormals();
    }

    [Serializable]
    public struct Octave
    {
        public Vector2 speed;
        public Vector2 scale;
        public float height;
        public bool alternate;
    }
}
