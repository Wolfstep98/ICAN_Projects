using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;

public class ShowVolonoi : MonoBehaviour {

    [Header("Properties")]
    [Tooltip("The numbers of sites/points we want")]
    public int sitesNumber = 500; // The numbers of sites/points we want

    public int width = 512;
    public int height = 512;

    public int llyodIteration = 2;

    private Dictionary<Vector2f, Site> sites;
    private List<Edge> edges;

    private void Start()
    {
        List<Vector2f> vertices = CreateRandomPoints();
        Rectf bounds = new Rectf(0, 0, width, height);

        Voronoi voronoi = new Voronoi(vertices, bounds, llyodIteration);

        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges; 

        DisplayVoronoiDiagram();
    }

    private List<Vector2f> CreateRandomPoints()
    {
        List<Vector2f> vertices = new List<Vector2f>();
        for (int i = 0; i < sitesNumber; i++)
        {
            vertices.Add(new Vector2f(Random.Range(0, width), Random.Range(0, height)));
        }
        return vertices;
    }

    private void DisplayVoronoiDiagram()
    {
        Texture2D tx = new Texture2D(512, 512);
        foreach (KeyValuePair<Vector2f, Site> kv in sites)
        {
            tx.SetPixel((int)kv.Key.x, (int)kv.Key.y, Color.red);
        }
        foreach (Edge edge in edges)
        {
            // if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
            if (edge.ClippedEnds == null) continue;

            DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, Color.black);
        }
        tx.Apply();

        this.GetComponent<Renderer>().material.mainTexture = tx;
    }

    // Bresenham line algorithm
    private void DrawLine(Vector2f p0, Vector2f p1, Texture2D tx, Color c, int offset = 0)
    {
        int x0 = (int)p0.x;
        int y0 = (int)p0.y;
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            tx.SetPixel(x0 + offset, y0 + offset, c);

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
}
