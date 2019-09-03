using System.Collections.Generic;
using UnityEngine;
using ClipperLib;

using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

public class Map : MonoBehaviour
{
    public GameObject maskPrefab;

    private PolygonCollider2D m_Collider2D;
    private Paths m_ClipPaths;
    private Paths m_ColliderPaths;

    private float m_ColliderPointScale = 1000.0f;


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

        Instantiate(maskPrefab, new Vector3(collideWorldPos.x, collideWorldPos.y, 0.0f), Quaternion.identity);
        Destroy(collision.gameObject);
    }
}
