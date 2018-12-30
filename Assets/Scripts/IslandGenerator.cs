using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;

public class IslandGenerator : MonoBehaviour
{
    ///<summary> the GPU shader used to give each polyogn a unique color </summary>
    Shader voronoiShader;

    ///<summary> the amount of unique, individual polygons in the voronoi diagram </summary>
    public int polygonCount; 
    public int resolution;
    public Dictionary<Vector2f, Site> sitesList;
    public List<Edge> edgesList;
    public Voronoi voronoiDiagram;
    public int lloydIterations;

    List<Vector4> centersList = new List<Vector4>();
    Rect bounds;

    public Texture2D firstStep;
    public Texture2D secondStep;

    void Start()
    {
        voronoiShader = Shader.Find("Image/Voronoi");
        GenerateIsland();

        //testing out first step
        GetComponent<Renderer>().material.mainTexture = firstStep;
    }

    public void GenerateIsland()
    {
        //first step: render initial Voronoi diagram.
        List<Vector2f> voronoiInitialPoints = CreateRandomPoints(polygonCount);
        bounds = new Rect(0,0,resolution,resolution);
        voronoiDiagram = new Voronoi(voronoiInitialPoints, bounds, lloydIterations);

        //taking edgesList and centersList from the diagram
        foreach(Vector2f point in voronoiDiagram.SiteCoords()) //we have to do this because List<> does not recognize an implicit convertion function
            centersList.Add((Vector2) point);
        edgesList = voronoiDiagram.Edges;

        //convert diagram to texture
        firstStep = new Texture2D(resolution,resolution);
        firstStep = ColorPolygons(firstStep);
        /*foreach(Vector4 point in centersList)
            firstStep.SetPixel((int)point.x, (int)point.y, Color.yellow);
        foreach(Edge edge in edgesList)
        {
            if(edge.ClippedEnds != null)
                DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], firstStep, Color.black);
        }
        firstStep.Apply();*/

        //second step: assign land/sea to each polygon
        secondStep = firstStep;
    }

    private Texture2D ColorPolygons(Texture2D tx)
    {
        Vector4[] pointsArray = centersList.ToArray();

        //generates material from shader and applies it
        Texture2D output = tx;
        Material voronoiMaterial = new Material(voronoiShader);
        voronoiMaterial.SetInt("_Length", pointsArray.Length);
        voronoiMaterial.SetVectorArray("_Points", pointsArray);
        RenderTexture rt = new RenderTexture(resolution,resolution,24);
        RenderTexture.active = rt;
        Graphics.Blit(output, rt, voronoiMaterial);
        output.ReadPixels(new Rect(0,0, resolution, resolution), 0, 0);
        output.Apply();
        RenderTexture.active = null;
        return output;
    }

    private List<Vector2f> CreateRandomPoints(int amount) {
        // Using Vector2f instead of Vector2 since csDelunay uses Vector2
        List<Vector2f> points = new List<Vector2f>();
        for (int i = 0; i < amount; i++) {
            points.Add(new Vector2f(Random.Range(0,resolution), Random.Range(0,resolution)));
        }
 
        return points;
    }

    private Texture2D DrawLine(Vector2f p0, Vector2f p1, Texture2D tx, Color c, int offset = 0) {
        Texture2D newTexture = tx;

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
            newTexture.SetPixel(x0+offset,y0+offset,c);
           
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

        newTexture.Apply();
        return newTexture;
    }

    public static void ShaderOnTexture(Material mat, Texture2D tex)
    {
        
    }
}
