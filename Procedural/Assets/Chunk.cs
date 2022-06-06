using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    //TODO: MAKE EM' CHUNKY!
    // Start is called before the first frame update
    [SerializeField] private int sizeyX=64, sizeyY=64;
    [SerializeField] private float noiseScale;
    [SerializeField] private float valueMultipl =50;
    [SerializeField] private float coordinateOffset=1.5f;
    [SerializeField] private int seed;
    private int2 chunkOffset;
    [SerializeField] private float secondNoisePassScale;
    [SerializeField] private float secondNoiseValueMultipl =50;
    
    private Vector3[] vertices;
    private Mesh mesh;
    private MeshCollider _meshCollider; 
    void Awake()
    {
        mesh = new Mesh(); 
        GetComponent<MeshFilter>().sharedMesh = mesh;
        _meshCollider = GetComponent<MeshCollider>();
    }
    public void Gen(int sizeX,int sizeY)
    {
        vertices= new Vector3[sizeX*sizeY];
        int[] triangles = new int[(sizeX + 1)*(sizeY+1)*6];
        FastNoiseLite noise = new FastNoiseLite(seed);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        
        // Gather noise data
        int xy=0;
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                vertices[xy] = new Vector3(x*coordinateOffset, 
                    //may be that chunk offset needs to be added with x/y instead of being added on top
                    //first pass noise
                    noise.GetNoise(x * noiseScale +chunkOffset.x*sizeX, y * noiseScale  +chunkOffset.y*sizeY) * valueMultipl
                                                               //second pass noise
                                                               +noise.GetNoise(x * secondNoisePassScale +chunkOffset.x*sizeX, y * secondNoisePassScale +chunkOffset.y*sizeY) *secondNoiseValueMultipl , y*coordinateOffset);
                xy++;
            }
        }
        int fy = 0;
        for (int y = 0; y < sizeY-1; y++)
        {
            for (int x = 0; x < sizeX-1; x++) 
            {
                //Left Triangle
                triangles[fy]= x +(y*sizeY);
                triangles[fy+1] = x+sizeX +(y*sizeY);
                triangles[fy+2]= x+1 +(y*sizeY);
                fy += 3;
                //Right Triangle
                triangles[fy]=  x + 1 +(y*sizeY);
                triangles[fy+2] =  x+sizeX + 1  +(y*sizeY);
                triangles[fy+1] = x+sizeX +(y*sizeY);
                fy += 3;
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        _meshCollider.sharedMesh = mesh;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
