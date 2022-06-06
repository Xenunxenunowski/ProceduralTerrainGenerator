using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Slider = UnityEngine.UI.Slider;

public class MeshGen : MonoBehaviour
{
    [SerializeField] private int sizeyX=64, sizeyY=64;
    [SerializeField] private float noiseScale;
    [SerializeField] private float valueMultipl =50;
    [SerializeField] private float coordinateOffset=1.5f;
    [SerializeField] private int seed =0;

    [SerializeField] private Slider firstNoiseScale, secondNoiseScale, thirdNoiseScale;
    [SerializeField] private Slider firstNoiseValueMultiplVal, secondNoiseValueMultiplVal, thirdNoiseValueMultiplVal;
    [SerializeField] private Slider updatesPerSecond;
    [SerializeField] private float secondNoisePassScale;
    [SerializeField] private float secondNoiseValueMultipl =50;
    [SerializeField] private TMP_InputField seedInput;
    [SerializeField] private float thirdNoisePassScale;
    [SerializeField] private float thirdNoiseValueMultipl;
    private Vector3[] vertices;
    private Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        StartCoroutine(Well());
        GetComponent<MeshFilter>().sharedMesh = mesh;
        seedInput.text = 0.ToString();
    }

    IEnumerator Well()
    {
        while (true)
        {
            try
            {
                GetValues();
                Gen(sizeyX,sizeyY);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            yield return new WaitForSeconds(1f/updatesPerSecond.value);
        }
        
    }

    private void GetValues()
    {
        
         noiseScale = firstNoiseScale.value;
         valueMultipl = firstNoiseValueMultiplVal.value;

         secondNoisePassScale = secondNoiseScale.value;
         secondNoiseValueMultipl = secondNoiseValueMultiplVal.value;

         thirdNoisePassScale = thirdNoiseScale.value;
         thirdNoiseValueMultipl = thirdNoiseValueMultiplVal.value;
         
         seed = Int32.Parse(seedInput.text);
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Gen(int sizeX,int sizeY)
    {
        vertices= new Vector3[sizeX*sizeY];
        int[] triangles = new int[(sizeX + 1)*(sizeY+1)*6];
        //int[] triangles = new int[sizeX * sizeY*3];
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        FastNoiseLite noise = new FastNoiseLite(seed);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        
        // Gather noise data
        int xy=0;
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                vertices[xy] = new Vector3(x*coordinateOffset, noise.GetNoise(x * noiseScale, y * noiseScale) * valueMultipl +noise.GetNoise(x * secondNoisePassScale, y * secondNoisePassScale) *secondNoiseValueMultipl + noise.GetNoise(x * thirdNoisePassScale, y * thirdNoisePassScale) *thirdNoiseValueMultipl, y*coordinateOffset);
                
                xy++;
            }
        }
        Debug.Log(vertices[2]);
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
        stopwatch.Start();
        Debug.Log("It tooK"+stopwatch.Elapsed);
        
    }

    public void OnDrawGizmosSelected()
    {
        
    }
}
