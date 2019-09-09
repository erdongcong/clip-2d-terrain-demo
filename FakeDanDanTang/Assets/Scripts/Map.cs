using System.Collections.Generic;
using UnityEngine;
using ClipperLib;
using System.Runtime.InteropServices;

using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

public class Map : MonoBehaviour
{
    public GameObject maskPrefab;
    public GameObject meshMaskPrefab;

    private PolygonCollider2D m_Collider2D;
    private Paths m_ClipPaths;
    private Paths m_ColliderPaths;

    private float m_ColliderPointScale = 1000.0f;

    [DllImport("tri")]
    private static extern int triangulate_polygon(int ncontours, int[] cntr, double[,] vertices, int[,] triangles);


    private void Awake()
    {
        // Setting up references.
        m_Collider2D = GetComponent<PolygonCollider2D>();

        m_ClipPaths = new Paths();
        m_ClipPaths.Add(new Path());

        m_ColliderPaths = new Paths();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("GameController"))
        {
            return;
        }

        Vector2 collideWorldPos = collision.GetContact(0).point;
        Vector2 collidePoint = transform.InverseTransformPoint(collideWorldPos);

        m_ClipPaths[0].Clear();
        int x = (int)((collidePoint.x - 0.3f) * m_ColliderPointScale);
        int y = (int)((collidePoint.y + 0.57f) * m_ColliderPointScale);
        m_ClipPaths[0].Add(new IntPoint(x, y));
        x = (int)((collidePoint.x - 0.57f) * m_ColliderPointScale);
        y = (int)((collidePoint.y + 0.24f) * m_ColliderPointScale);
        m_ClipPaths[0].Add(new IntPoint(x, y));
        x = (int)((collidePoint.x - 0.57f) * m_ColliderPointScale);
        y = (int)((collidePoint.y - 0.24f) * m_ColliderPointScale);
        m_ClipPaths[0].Add(new IntPoint(x, y));
        x = (int)((collidePoint.x - 0.3f) * m_ColliderPointScale);
        y = (int)((collidePoint.y - 0.57f) * m_ColliderPointScale);
        m_ClipPaths[0].Add(new IntPoint(x, y));
        x = (int)((collidePoint.x + 0.3f) * m_ColliderPointScale);
        y = (int)((collidePoint.y - 0.57f) * m_ColliderPointScale);
        m_ClipPaths[0].Add(new IntPoint(x, y));
        x = (int)((collidePoint.x + 0.57f) * m_ColliderPointScale);
        y = (int)((collidePoint.y - 0.24f) * m_ColliderPointScale);
        m_ClipPaths[0].Add(new IntPoint(x, y));
        x = (int)((collidePoint.x + 0.57f) * m_ColliderPointScale);
        y = (int)((collidePoint.y + 0.24f) * m_ColliderPointScale);
        m_ClipPaths[0].Add(new IntPoint(x, y));
        x = (int)((collidePoint.x + 0.3f) * m_ColliderPointScale);
        y = (int)((collidePoint.y + 0.57f) * m_ColliderPointScale);
        m_ClipPaths[0].Add(new IntPoint(x, y));

        for (int i = m_ColliderPaths.Count; i < m_Collider2D.pathCount; i++)
        {
            m_ColliderPaths.Add(new Path());
        }

        for(int j = 0; j < m_ColliderPaths.Count; j++)
        {
            m_ColliderPaths[j].Clear();
        }

        for(int k = 0; k < m_Collider2D.pathCount; k++)
        {
            Vector2[] path = m_Collider2D.GetPath(k);
            Path clippedPath = m_ColliderPaths[k];
            for(int m = 0; m < path.Length; m++)
            {
                x = (int)(path[m].x * m_ColliderPointScale);
                y = (int)(path[m].y * m_ColliderPointScale);
                clippedPath.Add(new IntPoint(x, y));
            }
        }

        Paths result = new Paths();
        Clipper c = new Clipper();
        c.AddPaths(m_ColliderPaths, PolyType.ptSubject, true);
        c.AddPaths(m_ClipPaths, PolyType.ptClip, true);
        c.Execute(ClipType.ctDifference, result, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

        m_Collider2D.pathCount = result.Count;
        for(int n = 0; n < result.Count; n++)
        {
            Path resultPath = result[n];
            List<Vector2> colliderPath = new List<Vector2>();
            for (int l = 0; l < resultPath.Count; l++)
            {
                colliderPath.Add(new Vector2((float)resultPath[l].X / m_ColliderPointScale, (float)resultPath[l].Y / m_ColliderPointScale));
                //Debug.Log("新网格：( " + resultPath[l].X + ", " + resultPath[l].Y + " )");
            }
            m_Collider2D.SetPath(n, colliderPath.ToArray());
        }

        //Instantiate(maskPrefab, new Vector3(collideWorldPos.x, collideWorldPos.y, 0.0f), Quaternion.identity);
        Destroy(collision.gameObject);

        //int[] a = { 4 };
        //double[,] b = { { 0.0, 0.0 }, { 0.0, 0.0 }, { 1.0, 0.0 }, { 1.0, 1.0 }, { 0.0, 1.0 } }; // b[0, 0] and b[0, 1] no use
        //int[,] d = new int[100, 3];
        //triangulate_polygon(1, a, b, d);

        //Debug.Log(1);
        c.Execute(ClipType.ctIntersection, result, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
        for(int o = 0; o < result.Count; o++)
        {
            Path clipPath = result[o];
            int[] vertexCounts = { clipPath.Count };
            double[,] vertexs = new double[clipPath.Count + 1, 2];
            // vertexs[0, 0] and vertexs[0, 1] no use
            vertexs[0, 0] = 0.0;
            vertexs[0, 1] = 0.0;

            Vector3[] meshVertices = new Vector3[clipPath.Count];
            Vector2[] UVs = new Vector2[clipPath.Count];

            for (int p = 0; p < clipPath.Count; p++)
            {
                vertexs[p + 1, 0] = (double)clipPath[p].X / m_ColliderPointScale;
                vertexs[p + 1, 1] = (double)clipPath[p].Y / m_ColliderPointScale;

                meshVertices[p].x = (float)clipPath[p].X / m_ColliderPointScale;
                meshVertices[p].y = (float)clipPath[p].Y / m_ColliderPointScale;
                meshVertices[p].z = 0.0f;
                meshVertices[p] = transform.TransformPoint(meshVertices[p]);
                meshVertices[p].x -= collideWorldPos.x;
                meshVertices[p].y -= collideWorldPos.y;

                UVs[p].x = 0.0f;
                UVs[p].y = 0.0f;
            }
            int[,] resultTriangles = new int[100, 3];
            triangulate_polygon(1, vertexCounts, vertexs, resultTriangles);

            int triangleCount = 0;
            for(int index = 0; index < 100; index++)
            {
                if(resultTriangles[index, 0] == 0)
                {
                    triangleCount = index;
                    break;
                }
            }
            int[] triangles = new int[triangleCount * 3];
            for(int q = 0; q < triangleCount; q++)
            {
                triangles[q * 3 + 2] = resultTriangles[q, 0] - 1;
                triangles[q * 3 + 1] = resultTriangles[q, 1] - 1;
                triangles[q * 3 + 0] = resultTriangles[q, 2] - 1;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = meshVertices;
            mesh.uv = UVs;
            mesh.triangles = triangles;

            GameObject meshMask = Instantiate(meshMaskPrefab, new Vector3(collideWorldPos.x, collideWorldPos.y, -1.0f), Quaternion.identity);
            meshMask.GetComponent<MeshFilter>().mesh = mesh;
        }
    }
}
