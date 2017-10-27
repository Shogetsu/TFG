
using UnityEngine;
using System.Collections;

public class TerrainGeneratorPRO : MonoBehaviour
{

    public float Length = 50f;
    public float Height = 50f;

    public int Resolution = 129;


    public Texture2D GrassTexture;
    public Texture2D RockTexture;

    public GameObject WaterPrefab;


    // Use this for initialization
    void Start()
    {
        Generate();
        PlaceWaterPrefab();
    }


    public void PlaceWaterPrefab()
    {
        GameObject WaterObject = Instantiate(WaterPrefab);
        WaterObject.transform.localScale = new Vector3(Length / 10f, 0f, Length / 10f);
        WaterObject.transform.position = new Vector3(Length / 2f, 18f, Length / 2f);
    }


    public void Generate()
    {
        TerrainData TerrainData = new TerrainData();
        TerrainData.alphamapResolution = Resolution;

        //Set heights
        TerrainData.heightmapResolution = Resolution;
        TerrainData.SetHeights(0, 0, MakeHeightmap());
        TerrainData.size = new Vector3(Length, Height, Length);

        //Set textures
        SplatPrototype Grass = new SplatPrototype();
        SplatPrototype Rock = new SplatPrototype();

        Grass.texture = GrassTexture;
        Grass.tileSize = new Vector2(4f, 4f);
        Rock.texture = RockTexture;
        Rock.tileSize = new Vector2(4f, 4f);

        TerrainData.splatPrototypes = new SplatPrototype[] { Grass, Rock };
        TerrainData.RefreshPrototypes();
        TerrainData.SetAlphamaps(0, 0, MakeSplatmap(TerrainData));


        //Create terrain
        GameObject TerrainObject = Terrain.CreateTerrainGameObject(TerrainData);
        TerrainObject.GetComponent<Terrain>().Flush();
    }


    public float[,] MakeHeightmap()
    {
        float[,] Heightmap = new float[Resolution, Resolution];

        for (int x = 0; x < Resolution; x++)
            for (int z = 0; z < Resolution; z++)
            {
               // Heightmap[x, z] = Mathf.Pow(GetNormalizedHeight((float)x, (float)z), 20);

               // Heightmap[x, z] = Mathf.Round(GetNormalizedHeight((float)x, (float)z) * 21) / 21;

                Heightmap[x, z] = GetNormalizedHeight((float)x, (float)z);
            }

        return Heightmap;
    }


    public float[,,] MakeSplatmap(TerrainData TerrainData)
    {
        float[,,] Splatmap = new float[Resolution, Resolution, 2];

        for (int x = 0; x < Resolution; x++)
            for (int z = 0; z < Resolution; z++)
            {
                float NormalizedX = (float)x / ((float)Resolution - 1f);
                float NormalizedZ = (float)z / ((float)Resolution - 1f);

                float Steepness = TerrainData.GetSteepness(NormalizedX, NormalizedZ) / 90f;

                Splatmap[z, x, 0] = 1f - Steepness;
                Splatmap[z, x, 1] = Steepness;
            }

        return Splatmap;
    }


    public float GetNormalizedHeight(float x, float z)
    {
       return Mathf.Clamp(Mathf.PerlinNoise(x * 0.05f, z * 0.05f), 0f, 0.4f) * 0.95f + Mathf.PerlinNoise(x * 0.1f, z * 0.1f) * 0.05f;

    }


    // Update is called once per frame
    void Update()
    {

    }
}