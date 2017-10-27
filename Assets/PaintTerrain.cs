using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintTerrain : MonoBehaviour {

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

    TerrainData terrainData;
    float[,] newHeightData;

    public SplatHeights[] splatHeights; //Aqui se almacenan los valores ALFA de las texturas almacenadas

    public float scale = 20f;

    public float offsetX = 100f;
    public float offsetY = 100f;
   
    /*Sliders*/
    [Header("Perlin Noise Settings")]
    [Range(0.000f, 0.01f)]
    public float bumpiness;
    [Range(0.00f, 1.000f)]
    public float damp;

    [Header("Mountain Settings")]
    public int numMountains;
    [Range(0.001f, 0.5f)]
    public float heightChange;
    [Range(0.0001f, 0.05f)]
    public float sideSlope; //pendiente lateral

    [Header("Hole Settings")]
    public int numHoles;
    [Range(0.0f, 1.0f)]
    public float holeDepth;
    [Range(0.001f, 0.5f)]
    public float holeChange;
    [Range(0.0001f, 0.05f)]
    public float holeSlope;

    [Header("River Settings")]
    public int numRivers;
    [Range(0.001f, 0.05f)]
    public float digDepth;
    [Range(0.001f, 1.0f)]
    public float maxDepth;
    [Range(0.0001f, 0.05f)]
    public float bankSlope;

    [Header("Rough Settings")]
    [Range(0.000f, 0.05f)]
    public float roughAmount;
    [Range(0, 5)]
    [Header("Smooth Settings")]
    public int smoothAmount;

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

    void Mountain(int x, int y, float height, float slope)
    {
        //Si x o y se van fuera del mapa, se finaliza la funcion
        if (x <= 0 || x >= terrainData.alphamapWidth) return;
        if (y <= 0 || y >= terrainData.alphamapHeight) return;
        //Si la altura es menor que 0, tambien se finaliza
        if (height <= 0) return;
        //Si los valores del array son mayores que la altura, tambien finaliza
        if (newHeightData[x, y] >= height) return;

        //ENTONCES, si no se ha finalizado hasta llegar aqui, se le asigna la nueva altura al terreno (nueva montanya creada)
        newHeightData[x, y] = height;

        /*Construir montanya desde 4 direcciones... estamos en una funcion RECURSIVA*/
        Mountain(x - 1, y, height - Random.Range(0.001f, slope), slope);
        Mountain(x + 1, y, height - Random.Range(0.001f, slope), slope);
        Mountain(x, y-1, height - Random.Range(0.001f, slope), slope);
        Mountain(x, y+1, height - Random.Range(0.001f, slope), slope);

    }
    void RiverCrawler(int x, int y, float height, float slope)
    {//FUNCION RECURSIVA
        if (x < 0 || x >= terrainData.alphamapWidth) return;
        if (y < 0 || y >= terrainData.alphamapHeight) return;
        if (height <= maxDepth) return;
        if (newHeightData[x, y] <= height) return;

        newHeightData[x, y] = height;

        RiverCrawler(x + 1, y, height + Random.Range(slope, slope + 0.01f), slope);
        RiverCrawler(x - 1, y, height + Random.Range(slope, slope + 0.01f), slope);
        RiverCrawler(x + 1, y+1, height + Random.Range(slope, slope + 0.01f), slope);
        RiverCrawler(x - 1, y+1, height + Random.Range(slope, slope + 0.01f), slope);
        RiverCrawler(x, y-1, height + Random.Range(slope, slope + 0.01f), slope);
        RiverCrawler(x, y+1, height + Random.Range(slope, slope + 0.01f), slope);
    }

    void Hole(int x, int y, float height, float slope)
    {
        //Si x o y se van fuera del mapa, se finaliza la funcion
        if (x <= 0 || x >= terrainData.alphamapWidth) return;
        if (y <= 0 || y >= terrainData.alphamapHeight) return;
        //Si la altura es menor que la profundidad maxima de los hoyos, finaliza el metodo
        if (height <= holeDepth) return;
        //Si los valores del array son menores que la altura del terreno, tambien finaliza
        if (newHeightData[x, y] <= height) return;
        
        //Nueva altura asignada
        newHeightData[x, y] = height;

        /*Construir hoyo desde 4 direcciones... estamos en una funcion RECURSIVA*/
        Hole(x - 1, y, height + Random.Range(slope,slope+0.01f), slope);
        Hole(x + 1, y, height + Random.Range(slope, slope + 0.01f), slope);
        Hole(x, y-1, height + Random.Range(slope, slope + 0.01f), slope);
        Hole(x, y+1, height + Random.Range(slope, slope + 0.01f), slope);


    }

    void ApplyRiver()
    {
        for(int i = 0; i < numRivers; i++)
        {
            //Posicion aleatoria del inicio del rio
            int cx = Random.Range(10, terrainData.alphamapWidth - 10);
            int cy = Random.Range(10, terrainData.alphamapHeight - 10);
            //Direccion inicial aleatoria del rio
            int xdir = Random.Range(-1, 2);
            int ydir = Random.Range(-1, 2);
            //Mientras que el rio no se salga del limite del mapa...
            while(cy >= 0 && cy < terrainData.alphamapHeight && cx > 0 && cx < terrainData.alphamapWidth)
            {
                //Metodo para crear el rio, similar al de los hoyos
                RiverCrawler(cx, cy, newHeightData[cx, cy] - digDepth, bankSlope); //digDepth = profundidad del rio ; bankSlope = pendiente de la orilla
               
                //La direccion y posicion del rio va cambiando en cada iteracion
                if (Random.Range(0, 50) < 5) // Cada 5 veces de 50 cambiara de direccion
                    xdir = Random.Range(-1, 2);
                if (Random.Range(0, 50) < 5)
                    ydir = Random.Range(0, 2);

                cx = cx + xdir;
                cy = cy + ydir;
            }

        }
    }
    void ApplyHoles()
    { //Este metodo es lo opuesto al metodo de las montanyas
        for(int i = 0; i < numHoles; i++)
        {
            //Se elige una posicion aleatoria del terreno (a 10 de distancia de los bordes del mapa)
            int xpos = Random.Range(10, terrainData.alphamapWidth - 10);
            int ypos = Random.Range(10, terrainData.alphamapHeight - 10);

            float newHeight = newHeightData[xpos, ypos] - holeChange; //Se asigna nueva altura al terreno en la posicion aleatoria, al ser un hoyo hay que restar la altura
            Hole(xpos, ypos, newHeight, sideSlope); //Se crea el hoyo
        }
    }
    void ApplyMountains()
    {
        for(int i=0; i < numMountains; i++)
        {
            //Se elige una posicion aleatoria del terreno (a 10 de distancia de los bordes del mapa)
            int xpos = Random.Range(10, terrainData.alphamapWidth - 10);
            int ypos = Random.Range(10, terrainData.alphamapHeight - 10);

            float newHeight = newHeightData[xpos, ypos] + heightChange; //Se asigna nueva altura al terreno en la posicion aleatoria
            Mountain(xpos, ypos, newHeight, sideSlope); //Se crea la montanya
        }
    }
    void RoughTerrain() // Hacer el terreno mas aspero
    {
        for(int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for(int x = 0; x < terrainData.alphamapWidth; x++)
            {
                newHeightData[x, y] += Random.Range(0, roughAmount); // A los vertices se les suma un valor aleatorio
            }
        }
    }

    void SmoothTerrain() //Suavizar terreno
    {
        //Los bucles empiezan en 1 porque se tienen en cuenta el valor de los vecinos, si se empezase en 0 hay vecinos que no existirian
        for(int y=1; y < terrainData.alphamapHeight - 1; y++)
        {
            for(int x=1; x < terrainData.alphamapWidth - 1; x++)
            {
                //A la altura de cada vertice se le suma el valor de los vertices vecinos, y se hace la media
                float avgheight = (newHeightData[x, y] +
                                    newHeightData[x + 1, y] +
                                    newHeightData[x - 1, y] +
                                    newHeightData[x + 1, y + 1] +
                                    newHeightData[x - 1, y - 1] +
                                    newHeightData[x + 1, y - 1] +
                                    newHeightData[x - 1, y + 1] +
                                    newHeightData[x, y + 1] +
                                    newHeightData[x, y - 1]) / 9.0f;

                newHeightData[x, y] = avgheight;
            }
        }
    }
    void ApplyPerlin()
    {
        for(int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for(int x = 0; x < terrainData.alphamapWidth; x++)
            { // Perlin Noise devuelve valores entre 0 y 1 que se almacenan en el array de alturas
              //Variables bumpiness para agregar suavidad
              //Variable damp para dar altura de forma suave


             newHeightData[x, y] = Mathf.PerlinNoise(x * bumpiness, y * bumpiness)*damp;


            }
        }
    }
    float CalculateHeights(int x, int y)
    {
        /*float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;
        
        return Mathf.PerlinNoise(xCoord, yCoord);*/
        float distance_x = Mathf.Abs(x - terrainData.alphamapWidth * 0.5f);
        float distance_y = Mathf.Abs(y - terrainData.alphamapHeight * 0.5f);
        float distance = Mathf.Sqrt(distance_x * distance_x + distance_y * distance_y); // circular mask

        float max_width = terrainData.alphamapWidth * 0.5f - 10.0f;
        float delta = distance / max_width;
        float gradient = delta * delta;

        //noise *= Mathf.Max(0.0f, 1.0f - gradient);
        /***ESTE ES EL BUENO***/
        float xCoord = (float)x / terrainData.alphamapWidth * scale + offsetX;
        float yCoord = (float)y / terrainData.alphamapHeight * scale + offsetY;

        return (Mathf.PerlinNoise(xCoord * 4, yCoord * 4) + 0.5f * Mathf.PerlinNoise(xCoord * 8, yCoord * 8) + 0.25f * Mathf.PerlinNoise(xCoord * 16, yCoord * 16)) * Mathf.Max(0.0f, 1.0f - gradient);

        //
    }

    // Use this for initialization
    public void Start () {

        //Primero, obtenemos los valores de altura del terreno
        terrainData = Terrain.activeTerrain.terrainData;
        
        //Matriz tridimensional, donde se almacena la anchura del terreno, la altura, y el numero de capas del terreno (texturas)
        float[,,] splatmapData = new float[terrainData.alphamapWidth,
                                            terrainData.alphamapHeight,
                                            terrainData.alphamapLayers];

        //Array que guarda todos los valores de alturas del terreno
        newHeightData = new float[terrainData.alphamapWidth,
                                terrainData.alphamapHeight];

        ApplyPerlin();//Se aplica el perlin noise y se almacenan en el array anterior
        RoughTerrain();
        ApplyMountains();
        ApplyHoles();
        ApplyRiver();
        for (int i = 0; i < smoothAmount; i++)
            SmoothTerrain();
        terrainData.SetHeights(0, 0, newHeightData); //Se asigna el array de alturas al terreno
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
	
	// Update is called once per frame
	void Update () {
		
	}
}
