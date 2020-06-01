using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;


public class GenerateVolonoi : MonoBehaviour {

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

    }

    private List<Vector2f> CreateRandomPoints()
    {
        List<Vector2f> vertices = new List<Vector2f>();
        for(int i = 0; i < sitesNumber; i++)
        {
            vertices.Add(new Vector2f(Random.Range(0, width), Random.Range(0, height)));
        }
        return vertices;
    }
}
