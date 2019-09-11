using System.Collections.Generic;
using UnityEngine;
using ClipperLib;
using System.Runtime.InteropServices;

using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

public class Map : MonoBehaviour
{
    public bool oIsSpriteMask;

    [SerializeField] private GameObject oMaskPrefab = null;
    [SerializeField] private GameObject oMeshMaskPrefab = null;
    [SerializeField] private GameObject oClipTpl = null;

    private PolygonCollider2D m_Collider2D;
    private Paths m_ClipPaths;
    private Paths m_ColliderPaths;
    private Vector2[] m_ClipVertices;

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

        GameObject temp = Instantiate(oClipTpl);
        m_ClipVertices = temp.GetComponent<PolygonCollider2D>().GetPath(0);
        Destroy(temp);
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
        //int x = (int)((collidePoint.x - 0.3f) * m_ColliderPointScale);
        //int y = (int)((collidePoint.y + 0.57f) * m_ColliderPointScale);
        //m_ClipPaths[0].Add(new IntPoint(x, y));
        //x = (int)((collidePoint.x - 0.57f) * m_ColliderPointScale);
        //y = (int)((collidePoint.y + 0.24f) * m_ColliderPointScale);
        //m_ClipPaths[0].Add(new IntPoint(x, y));
        //x = (int)((collidePoint.x - 0.57f) * m_ColliderPointScale);
        //y = (int)((collidePoint.y - 0.24f) * m_ColliderPointScale);
        //m_ClipPaths[0].Add(new IntPoint(x, y));
        //x = (int)((collidePoint.x - 0.3f) * m_ColliderPointScale);
        //y = (int)((collidePoint.y - 0.57f) * m_ColliderPointScale);
        //m_ClipPaths[0].Add(new IntPoint(x, y));
        //x = (int)((collidePoint.x + 0.3f) * m_ColliderPointScale);
        //y = (int)((collidePoint.y - 0.57f) * m_ColliderPointScale);
        //m_ClipPaths[0].Add(new IntPoint(x, y));
        //x = (int)((collidePoint.x + 0.57f) * m_ColliderPointScale);
        //y = (int)((collidePoint.y - 0.24f) * m_ColliderPointScale);
        //m_ClipPaths[0].Add(new IntPoint(x, y));
        //x = (int)((collidePoint.x + 0.57f) * m_ColliderPointScale);
        //y = (int)((collidePoint.y + 0.24f) * m_ColliderPointScale);
        //m_ClipPaths[0].Add(new IntPoint(x, y));
        //x = (int)((collidePoint.x + 0.3f) * m_ColliderPointScale);
        //y = (int)((collidePoint.y + 0.57f) * m_ColliderPointScale);
        //m_ClipPaths[0].Add(new IntPoint(x, y));
        for(int i = 0; i < m_ClipVertices.Length; i++)
        {
            int x = (int)((collidePoint.x - m_ClipVertices[i].x) * m_ColliderPointScale);
            int y = (int)((collidePoint.y - m_ClipVertices[i].y) * m_ColliderPointScale);
            m_ClipPaths[0].Add(new IntPoint(x, y));
        }

        for (int i = m_ColliderPaths.Count; i < m_Collider2D.pathCount; i++)
        {
            m_ColliderPaths.Add(new Path());
        }

        for(int i = 0; i < m_ColliderPaths.Count; i++)
        {
            m_ColliderPaths[i].Clear();
        }

        for(int i = 0; i < m_Collider2D.pathCount; i++)
        {
            Vector2[] path = m_Collider2D.GetPath(i);
            Path clippedPath = m_ColliderPaths[i];
            for(int j = 0; j < path.Length; j++)
            {
                int x = (int)(path[j].x * m_ColliderPointScale);
                int y = (int)(path[j].y * m_ColliderPointScale);
                clippedPath.Add(new IntPoint(x, y));
            }
        }

        Paths result = new Paths();
        Clipper c = new Clipper();
        c.AddPaths(m_ColliderPaths, PolyType.ptSubject, true);
        c.AddPaths(m_ClipPaths, PolyType.ptClip, true);
        c.Execute(ClipType.ctDifference, result, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

        m_Collider2D.pathCount = result.Count;
        for(int i = 0; i < result.Count; i++)
        {
            Path resultPath = result[i];
            List<Vector2> colliderPath = new List<Vector2>();
            for (int j = 0; j < resultPath.Count; j++)
            {
                colliderPath.Add(new Vector2((float)resultPath[j].X / m_ColliderPointScale, (float)resultPath[j].Y / m_ColliderPointScale));
                //Debug.Log("新网格：( " + resultPath[j].X + ", " + resultPath[j].Y + " )");
            }
            m_Collider2D.SetPath(i, colliderPath.ToArray());
        }

        Destroy(collision.gameObject);

        if (oIsSpriteMask)
        {
            Instantiate(oMaskPrefab, new Vector3(collideWorldPos.x, collideWorldPos.y, 0.0f), Quaternion.identity);
        }
        else
        {
            //int[] a = { 4 };
            //double[,] b = { { 0.0, 0.0 }, { 0.0, 0.0 }, { 1.0, 0.0 }, { 1.0, 1.0 }, { 0.0, 1.0 } }; // b[0, 0] and b[0, 1] no use
            //int[,] d = new int[100, 3];
            //triangulate_polygon(1, a, b, d);

            //Debug.Log(1);
            c.Execute(ClipType.ctIntersection, result, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
            for (int i = 0; i < result.Count; i++)
            {
                Path clipPath = result[i];
                int[] vertexCounts = { clipPath.Count };
                double[,] vertexs = new double[clipPath.Count + 1, 2];
                // vertexs[0, 0] and vertexs[0, 1] no use
                vertexs[0, 0] = 0.0;
                vertexs[0, 1] = 0.0;

                Vector3[] meshVertices = new Vector3[clipPath.Count];
                Vector2[] UVs = new Vector2[clipPath.Count];

                for (int j = 0; j < clipPath.Count; j++)
                {
                    vertexs[j + 1, 0] = (double)clipPath[j].X / m_ColliderPointScale;
                    vertexs[j + 1, 1] = (double)clipPath[j].Y / m_ColliderPointScale;

                    meshVertices[j].x = (float)clipPath[j].X / m_ColliderPointScale;
                    meshVertices[j].y = (float)clipPath[j].Y / m_ColliderPointScale;
                    meshVertices[j].z = 0.0f;
                    meshVertices[j] = transform.TransformPoint(meshVertices[j]);
                    meshVertices[j].x -= collideWorldPos.x;
                    meshVertices[j].y -= collideWorldPos.y;

                    UVs[j].x = 0.0f;
                    UVs[j].y = 0.0f;
                }
                int[,] resultTriangles = new int[100, 3];
                triangulate_polygon(1, vertexCounts, vertexs, resultTriangles);

                int triangleCount = 0;
                for (int index = 0; index < 100; index++)
                {
                    if (resultTriangles[index, 0] == 0)
                    {
                        triangleCount = index;
                        break;
                    }
                }
                int[] triangles = new int[triangleCount * 3];
                for (int j = 0; j < triangleCount; j++)
                {
                    triangles[j * 3 + 2] = resultTriangles[j, 0] - 1;
                    triangles[j * 3 + 1] = resultTriangles[j, 1] - 1;
                    triangles[j * 3 + 0] = resultTriangles[j, 2] - 1;
                }

                Mesh mesh = new Mesh();
                mesh.vertices = meshVertices;
                mesh.uv = UVs;
                mesh.triangles = triangles;

                GameObject meshMask = Instantiate(oMeshMaskPrefab, new Vector3(collideWorldPos.x, collideWorldPos.y, -1.0f), Quaternion.identity);
                meshMask.GetComponent<MeshFilter>().mesh = mesh;
            }
        }
    }
}
