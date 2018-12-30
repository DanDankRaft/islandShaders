using UnityEngine;
using System.Collections.Generic;
 
using csDelaunay;
 
public class csVoronoi : MonoBehaviour {
 
    // The number of polygons/sites we want
    public int polygonNumber = 200;
    public int resolution = 512;
    public Shader voronoiShader;
    // This is where we will store the resulting data
    public Dictionary<Vector2f, Site> sites;
    public List<Edge> edges;
    public Dictionary<Vector2f, int> polygonAssociations;
 
    void Start() {
        // Create your sites (lets call that the center of your polygons)
        List<Vector2f> points = CreateRandomPoint();
       
        // Create the bounds of the voronoi diagram
        // Use Rectf instead of Rect; it's a struct just like Rect and does pretty much the same,
        // but like that it allows you to run the delaunay library outside of unity (which mean also in another tread)
        Rectf bounds = new Rectf(0,0,resolution,resolution);
       
        // There is a two ways you can create the voronoi diagram: with or without the lloyd relaxation
        // Here I used it with 2 iterations of the lloyd relaxation
        Voronoi voronoi = new Voronoi(points,bounds,5);
        // But you could also create it without lloyd relaxtion and call that function later if you want
        //Voronoi voronoi = new Voronoi(points,bounds);
        //voronoi.LloydRelaxation(5);
 
        // Now retreive the edges from it, and the new sites position if you used lloyd relaxtion
        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges;
 
        DisplayVoronoiDiagram();
    }


    private Texture2D AssociatePolygonsWithShader(Texture2D tx)
    {
        //creates the array of points neccessary for the material
        List<Vector4> voronoiPointsList = new List<Vector4>();
        foreach(KeyValuePair<Vector2f, Site> site in sites)
        {
            voronoiPointsList.Add(new Vector4(site.Key.x, site.Key.y));
        }
        Vector4[] voronoiPoints = voronoiPointsList.ToArray();

        //creates the material and applys it to the shader
        Texture2D output = tx;
        Material voronoiMaterial = new Material(voronoiShader);
        voronoiMaterial.SetInt("_Length", voronoiPoints.Length);
        voronoiMaterial.SetVectorArray("_Points", voronoiPoints);
        RenderTexture rt = new RenderTexture(resolution,resolution,24);
        RenderTexture.active = rt;
        Graphics.Blit(output, rt, voronoiMaterial);
        output.ReadPixels(new Rect(0,0, resolution, resolution), 0, 0);
        output.Apply();
        RenderTexture.active = null;
        return output;
    }

    private List<Vector2f> CreateRandomPoint() {
        // Use Vector2f, instead of Vector2
        // Vector2f is pretty much the same than Vector2, but like you could run Voronoi in another thread
        List<Vector2f> points = new List<Vector2f>();
        for (int i = 0; i < polygonNumber; i++) {
            points.Add(new Vector2f(Random.Range(0,resolution), Random.Range(0,resolution)));
        }
 
        return points;
    }
 
    // Here is a very simple way to display the result using a simple bresenham line algorithm
    // Just attach this script to a quad
    private void DisplayVoronoiDiagram() {
        Texture2D tx = new Texture2D(resolution,resolution);
        tx = AssociatePolygonsWithShader(tx);
        foreach (KeyValuePair<Vector2f,Site> kv in sites) {
            tx.SetPixel((int)kv.Key.x, (int)kv.Key.y, Color.yellow);
        }
        foreach (Edge edge in edges) {
            // if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
            if (edge.ClippedEnds == null) continue;
 
            DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, Color.black);
        }
        tx.Apply();
 
        GetComponent<Renderer>().material.mainTexture = tx;
    }
 
    // Bresenham line algorithm
    private void DrawLine(Vector2f p0, Vector2f p1, Texture2D tx, Color c, int offset = 0) {
        int x0 = (int)p0.x;
        int y0 = (int)p0.y;
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;
       
        int dx = Mathf.Abs(x1-x0);
        int dy = Mathf.Abs(y1-y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx-dy;
       
        while (true) {
            tx.SetPixel(x0+offset,y0+offset,c);
           
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2*err;
            if (e2 > -dy) {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx) {
                err += dx;
                y0 += sy;
            }
        }
    }
}   
