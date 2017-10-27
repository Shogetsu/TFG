using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    public float Length = 50f;
    public float Height = 50f;

    //La resolucion del terreno: debe ser el valor de una potencia de 2 +1 (2^x+1)
    //Por ejemplo (2^7)+1=129
    //Esto sirve para dividir el terreno en trozos(Tiles)
    public int Resolution = 129;

    public Texture2D GrassTexture;
    public Texture2D RockTexture;

    public GameObject WaterPrefab;

    // Use this for initialization
    void Start () {
        Generate(); //El metodo de generar se ejecuta al inicio
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
        //Primero crearemos un objeto TerrainData
        TerrainData TerrainData = new TerrainData();
        //Dentro le asignamos cosas importantes de nuestro terreno
        TerrainData.alphamapResolution = Resolution; //resolucion mapa alfa

        //Asignar alturas
        TerrainData.heightmapResolution = Resolution; //resolucion mapa de altura
        TerrainData.SetHeights(0, 0, MakeHeightmap());
        TerrainData.size = new Vector3(Length, Height, Length); //Tamanyo mapa

        //Asignar texturas
        //Los SplatPrototype son texturas que se usan en objetos tipo TerrainData
        //Prototypes son assets que utilizan en terrenos
        SplatPrototype Grass = new SplatPrototype();
        SplatPrototype Rock = new SplatPrototype();

        Grass.texture = GrassTexture;
        Grass.tileSize = new Vector2(4f, 4f); //Asignar tamanyo
        Rock.texture = RockTexture;
        Rock.tileSize = new Vector2(4f, 4f); //Asignar tamanyo

        TerrainData.splatPrototypes = new SplatPrototype[] { Grass, Rock }; //Asignacion de texturas de terreno
        TerrainData.RefreshPrototypes();
        TerrainData.SetAlphamaps(0, 0, MakeSplatmap(TerrainData)); //Esto es para crear el efecto de combinacion de texturas

        /*CREACION DEL TERRENO*/
        //Hecho esto podemos spawnear gameObjects
        //usando CreateTerrainGameObject
        GameObject TerrainObject = Terrain.CreateTerrainGameObject(TerrainData); //GameObject del terreno!
        TerrainObject.GetComponent<Terrain>().Flush(); //Importante para que los cambios hagan efecto

    }

    /*Para las alturas necesitaremos un metodo especifico que devolvera un array
     de valores de ruido*/
    public float[,] MakeHeightmap()
    {
        //Variable que almacena valores para el bucle
        float[,] Heightmap = new float[Resolution, Resolution];
        
        //Doble bucle
        for(int x=0; x < Resolution; x++)
        {
            for(int z = 0; z<Resolution; z++)
            {
                Heightmap[x, z] = GetNormalizedHeight((float)x, (float)z);
            }
        }

        return Heightmap;
    }

    //Funcion que devuelve un array de Splatmap
    //Los splatmaps son una combinacion de texturas mediante alphamap (transparencia)
    public float [,,] MakeSplatmap(TerrainData TerrainData)
    {
        //En este metodo es importante tener en cuenta la pendiente del terreno
        float[,,] Splatmap = new float[Resolution, Resolution, 2];

        for(int x=0; x < Resolution; x++)
        {
            for(int z=0; z<Resolution; z++)
            {
                float NormalizedX = (float)x / ((float)Resolution - 1f);
                float NormalizedZ = (float)z / ((float)Resolution - 1f);

                //Obtencion de la pendiente del terreno
                //Este metodo Get devuelve el angulo en grados entre 0 y 90
                float Steepness = TerrainData.GetSteepness(NormalizedX, NormalizedZ) / 90f;

                Splatmap[z, x, 0] = 1f - Steepness;
                Splatmap[z, x, 1] = Steepness;
            }
        }
        return Splatmap;
    }
        
    //Funcion sencilla que devuelve el resultado del metodo Perlin Noise
    public float GetNormalizedHeight(float x, float z)
    {
        //El metodo Clamp es un metodo que sirve para limitar un valor dentro de 2 parametros (minimo y maximo)
        return Mathf.Clamp(Mathf.PerlinNoise(x * 0.05f, z * 0.05f), 0f, 0.4f) * 0.95f +Mathf.PerlinNoise(x * 0.1f, z * 0.1f) *0.05f;
           
    }


	// Update is called once per frame
	void Update () {
		
	}
}
