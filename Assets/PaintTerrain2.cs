using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintTerrain2 : MonoBehaviour {
   
    /*Clase publica.
     En esta clase vamos a almacenar el indice de la textura que estamos usando y la altura a la que la textura empieza a pintarse*/
    [System.Serializable]
    public class SplatHeights
    {
        /* Cada textura tiene un indice, y la altura en la que comienza */
        public int textureIndex; 
        public int startingheight;
        public int overlap; // Variable que indica la distancia que nuestras texturas permiten solaparse
    }

    public SplatHeights[] splatHeights; //Aqui se almacenan los valores ALFA de las texturas almacenadas


    //El metodo de normalizar sirve para que las texturas no se vean tan brillantes
    void normalize(float[] v) // Todos los valores del vector deben de sumar 1
    {
        float total = 0;
        for(int i = 0; i < v.Length; i++)
        {
            total += v[i];
        }

        for(int i=0; i < v.Length; i++)
        {
            v[i] /= total;
        }
    }
 
    //Esta funcion sirve para mapear los valores del Perlin Noise, y hace las transiciones de texturas aun mas realistas
    public float map(float value, float sMin, float sMAx, float mMin, float mMax)
    {
        return (value - sMin) * (mMax - mMin) / (sMAx - sMin) + mMin;
    }

	// Use this for initialization
	public void Start () {
        //Primero, obtenemos los valores de altura del terreno
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        
        //Matriz tridimensional, donde se almacena la anchura del terreno, la altura, y el numero de capas del terreno (texturas)
        float[,,] splatmapData = new float[terrainData.alphamapWidth,
                                            terrainData.alphamapHeight,
                                            terrainData.alphamapLayers];

        for(int y=0; y < terrainData.alphamapHeight; y++)
        {
            for(int x=0; x < terrainData.alphamapWidth; x++)
            {
                //Obtenemos la altura del mapa en cada uno de los puntos
                //IMPORTANTE ejes (y, x) invertidos 
                float terrainHeight = terrainData.GetHeight(y, x);
                //Array de floats para almacenar los diferentes valores alfa para cada textura, por lo que habran 5 floats en el array (porque tenemos de momento 5 texturas)
                float[] splat = new float[splatHeights.Length];

                //Bucle que se repite tantas veces como texturas haya
                for(int i=0; i < splatHeights.Length; i++)
                {

                    /*PARA EL SOLAPAMIENTO DE TEXTURAS*/
                    /*USAMOS PERLIN NOISE para hacer patrones random suaves de las texturas en los solapamientos, los cambios de textura son mas naturales*/
                    //A la altura inicial de la textura correspondiente se le resta la distancia de solapamiento para que empiece un poco mas abajo 

                    //Usamos metodo Clamp para limitar el valor del perlinNoise y que los patrones sean menos pronunciados
                   //float thisNoise = Mathf.Clamp(Mathf.PerlinNoise(x * 0.05f, y * 0.05f), 0.5f, 1);

                    float thisNoise = map(Mathf.PerlinNoise(x * 0.05f, y * 0.05f),0,1, 0.5f, 1);

                    float thisHeightStart = splatHeights[i].startingheight * thisNoise -
                        splatHeights[i].overlap * thisNoise;

                    //Solapamiento por encima de la textura
                    float nextHeightStart = 0;
                    if(i != splatHeights.Length - 1)
                    {
                        nextHeightStart = splatHeights[i + 1].startingheight * thisNoise +
                            splatHeights[i + 1].overlap * thisNoise;
                    }

                    if(i==splatHeights.Length-1 && terrainHeight >= thisHeightStart)//Si es la ultima textura
                    {
                        splat[i]=1;
                    }
                    //Sino si la altura del terreno actual es mayor o igual que la altura de la textura que estamos leyendo Y menor o igual que la siguiente textura...
                    else if (terrainHeight >= thisHeightStart && terrainHeight <= nextHeightStart)
                    {
                        splat[i] = 1; //El valor ALFA de esa textura se pone a 1 (Opacidad MAXIMA)
                    }
                }

                normalize(splat);

                for(int j=0; j < splatHeights.Length; j++)
                {
                    splatmapData[x, y, j] = splat[j]; // A cada posicion (x,y) y textura (j) se le pasa su valor alfa asignada en el bucle anterior
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, splatmapData); // Se asignan todos los valores alfa al terreno creado
	}

}
