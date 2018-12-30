using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;

public class ApplyScreenShader : MonoBehaviour
{
    public Shader imageShader;
    public Vector4[] voronoiPoints; //although only x and y will be used, we need all 4 coordinates to make this shit work

    void Start()
    {
        for (int i = 0; i < voronoiPoints.Length; i++)
        {
            voronoiPoints[i] = new Vector4(Random.Range(0, Screen.width), Random.Range(0, Screen.height));
        }
    }


    void OnRenderImage(RenderTexture input, RenderTexture output)
    {
        //creating the Voronoi material, seinding it the points, and printing to screen!
        Material voronoiMaterial = new Material(imageShader);
        voronoiMaterial.SetInt("_Length", voronoiPoints.Length);
        voronoiMaterial.SetVectorArray("_Points", voronoiPoints);
        Graphics.Blit(input, output, voronoiMaterial);
    }
}
