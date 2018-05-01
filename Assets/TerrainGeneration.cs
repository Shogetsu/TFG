using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

//public class SyncListH : SyncListStruct<float> { }

public class TerrainGeneration : NetworkBehaviour
{
    Terrain terrain;

    // public SyncListFloat SyncListHeightsClient = new SyncListFloat();

    float[,] heights;

    public int depth = 20;


    public int width = 256;


    public int height = 256;


    public float scale = 20f;

    /*Sincronizacion de las variables aleatorias del terreno, asi todos los jugadores veran el mismo terreno estando en la misma partida*/
    [SyncVar]
    public float offsetX = 100f;
    [SyncVar]
    public float offsetY = 100f;

    [SyncVar (hook ="BuildTerrain")]
    bool offserValuesSetted;

    bool MapCreated;

    [Header("Tree Settings")]
    public GameObject treePrefab;
    public GameObject treePrefab2;
    public int numForest;
    public int numTrees;
    [Range(3f, 50f)]
    public float maxForestRadius;


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
    [Range(0.00001f, 0.05f)]
    public float digDepth;
    [Range(0.00001f, 1.0f)]
    public float maxDepth;
    [Range(0.00001f, 0.05f)]
    public float bankSlope;

    Vector3 RandomCircle(Vector3 center, float radius, int start, int overlap)
    {
        Vector3 pos = new Vector3(0, 0, 0);
        bool posicionado = false;

        while (posicionado == false)
        {
            float ang = Random.value * 360;
            float radiusRandom = Random.Range(1, radius); //minimo 1, maximo el radio indicado

              pos.x = center.x + radiusRandom * Mathf.Sin(ang * Mathf.Deg2Rad); // pos x aleatoria
                                                                                // pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
              pos.z = center.z + radiusRandom * Mathf.Cos(ang * Mathf.Deg2Rad); // pos z aleatoria
              pos.y = terrain.SampleHeight(new Vector3(pos.x, 0, pos.z)); //Cada arbol se pone a la altura que le corresponda segun el terreno (NO aleatoria)

           
            if (pos.y >= start - overlap && pos.y <= start + overlap) //Se controla la altura en la que se estan creando, siempre deben de crearse en la altura que corresponde
            {
                posicionado = true;
            }
        }

        return pos;
    }

    [Command]
    void CmdSpawnTree()
    {
        int auxForest = 0;
        //Se establece el rango de altura donde se van a generar los arboles
        int start = GetComponent<PaintTerrain2>().splatHeights[2].startingheight; //inicio
        int overlap = GetComponent<PaintTerrain2>().splatHeights[2].overlap; //solapamiento
        while (auxForest<numForest) //Se crean los bosques
        {

            //Posicion aleatoria del bosque
            float xpos = Random.Range(2, terrain.terrainData.alphamapWidth - 10);
            float zpos = Random.Range(2, terrain.terrainData.alphamapHeight - 10);
            float ypos = terrain.SampleHeight(new Vector3(xpos, 0, zpos)); //Importante situar el bosque a la altura correspondiente del terreno

            Vector3 center = new Vector3(xpos, ypos, zpos); //Se obtiene el centro del terreno
           if (ypos>=start-overlap && ypos <= start+overlap) // Se comprueba si el bosque se esta situando dentro del rango de altura donde se deben generar los bosques
            {
                int t = 0;
                while(t < numTrees) //Se crean los arboles de cada bosque
                {
                    //float r = Random.Range(3, maxForestRadius);
                    Vector3 treePos = RandomCircle(center, maxForestRadius, start, overlap); //Con el centro y un radio, se "traza" un circulo sobre el terreno y se obtienen posiciones aleatorias dentro de ese circulo
                    if (!Physics.CheckSphere(treePos, 0.5f))
                    {
                        //  Debug.Log("Instancio el arbol...");

                        GameObject treeGO = treePrefab as GameObject;
                        /*
                        GameObject[] trees =
                        {
                            treePrefab, treePrefab2
                        };
                        int rnd = Random.Range(0, 2);
                       // Debug.Log(rnd);
                        float rndscale = Random.Range(2f, 5f);
                        trees[rnd].transform.localScale = new Vector3(rndscale, rndscale, rndscale);

                        float rndrota = Random.Range(0f, 5f);
                        trees[rnd].transform.Rotate(new Vector3(rndrota, 0, 0));*/

                        GameObject tree = Instantiate(treeGO, treePos, Quaternion.identity);
                       // Debug.Log("...Instanciado con exito");

                        NetworkServer.Spawn(tree);
                        //Debug.Log("Arbol spawneado");
                        t++;
                    }                
                }
                auxForest++;
            }
        }
    }

    void RiverCrawler(int x, int y, float height, float slope)
    {//FUNCION RECURSIVA
        if (x < 0 || x >= terrain.terrainData.alphamapWidth) return;
        if (y < 0 || y >= terrain.terrainData.alphamapHeight) return;
        if (height <= maxDepth) return;
        if (heights[x, y] <= height) return;

        heights[x, y] = height;

        RiverCrawler(x + 1, y, height + Random.Range(slope, slope + 0.01f), slope);
        RiverCrawler(x - 1, y, height + Random.Range(slope, slope + 0.01f), slope);
        RiverCrawler(x + 1, y + 1, height + Random.Range(slope, slope + 0.01f), slope);
        RiverCrawler(x - 1, y + 1, height + Random.Range(slope, slope + 0.01f), slope);
        RiverCrawler(x, y - 1, height + Random.Range(slope, slope + 0.01f), slope);
        RiverCrawler(x, y + 1, height + Random.Range(slope, slope + 0.01f), slope);
    }

    void ApplyRiver()
    {
        for (int i = 0; i < numRivers; i++)
        {
            //Posicion aleatoria del inicio del rio
            int cx = Random.Range(10, terrain.terrainData.alphamapWidth - 10);
            int cy = Random.Range(10, terrain.terrainData.alphamapHeight - 10);
            //Direccion inicial aleatoria del rio
            int xdir = Random.Range(-1, 2);
            int ydir = Random.Range(-1, 2);
            //Mientras que el rio no se salga del limite del mapa...
            while (cy >= 0 && cy < terrain.terrainData.alphamapHeight && cx > 0 && cx < terrain.terrainData.alphamapWidth)
            {
                //Metodo para crear el rio, similar al de los hoyos
                RiverCrawler(cx, cy, heights[cx, cy] - digDepth, bankSlope); //digDepth = profundidad del rio ; bankSlope = pendiente de la orilla

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
    void Mountain(int x, int y, float height, float slope)
    {
        //Si x o y se van fuera del mapa, se finaliza la funcion
        if (x <= 0 || x >= terrain.terrainData.alphamapWidth) return;
        if (y <= 0 || y >= terrain.terrainData.alphamapHeight) return;
        //Si la altura es menor que 0, tambien se finaliza
        if (height <= 0) return;
        //Si los valores del array son mayores que la altura, tambien finaliza
        if (heights[x, y] >= height) return;

        //ENTONCES, si no se ha finalizado hasta llegar aqui, se le asigna la nueva altura al terreno (nueva montanya creada)
       heights[x, y] = height;

        /*Construir montanya desde 4 direcciones... estamos en una funcion RECURSIVA*/
        Mountain(x - 1, y, height - Random.Range(0.001f, slope), slope);
        Mountain(x + 1, y, height - Random.Range(0.001f, slope), slope);
        Mountain(x, y - 1, height - Random.Range(0.001f, slope), slope);
        Mountain(x, y + 1, height - Random.Range(0.001f, slope), slope);

    }
    void ApplyMountains()
    {
        for (int i = 0; i < numMountains; i++)
        {
            //Se elige una posicion aleatoria del terreno (a 10 de distancia de los bordes del mapa)
            int xpos = Random.Range(10, terrain.terrainData.alphamapWidth - 10);
            int ypos = Random.Range(10, terrain.terrainData.alphamapHeight - 10);

            float newHeight = heights[xpos, ypos] + heightChange; //Se asigna nueva altura al terreno en la posicion aleatoria
            Mountain(xpos, ypos, newHeight, sideSlope); //Se crea la montanya
        }
    }

    void Hole(int x, int y, float height, float slope)
    {
        //Si x o y se van fuera del mapa, se finaliza la funcion
        if (x <= 0 || x >= terrain.terrainData.alphamapWidth) return;
        if (y <= 0 || y >= terrain.terrainData.alphamapHeight) return;
        //Si la altura es menor que la profundidad maxima de los hoyos, finaliza el metodo
        if (height <= holeDepth) return;
        //Si los valores del array son menores que la altura del terreno, tambien finaliza
        if (heights[x, y] <= height) return;

        //Nueva altura asignada
        heights[x, y] = height;

        /*Construir hoyo desde 4 direcciones... estamos en una funcion RECURSIVA*/
        Hole(x - 1, y, height + Random.Range(slope, slope + 0.01f), slope);
        Hole(x + 1, y, height + Random.Range(slope, slope + 0.01f), slope);
        Hole(x, y - 1, height + Random.Range(slope, slope + 0.01f), slope);
        Hole(x, y + 1, height + Random.Range(slope, slope + 0.01f), slope);


    }
    void ApplyHoles()
    { //Este metodo es lo opuesto al metodo de las montanyas
        for (int i = 0; i < numHoles; i++)
        {
            //Se elige una posicion aleatoria del terreno (a 10 de distancia de los bordes del mapa)
            int xpos = Random.Range(10, terrain.terrainData.alphamapWidth - 10);
            int ypos = Random.Range(10, terrain.terrainData.alphamapHeight - 10);

            float newHeight = heights[xpos, ypos] - holeChange; //Se asigna nueva altura al terreno en la posicion aleatoria, al ser un hoyo hay que restar la altura
            Hole(xpos, ypos, newHeight, sideSlope); //Se crea el hoyo
        }
    }

    void Start()
    {
        /*if (isClient && isServer)
        {
        }*/
        /*if (isServer)
        {
            offsetX = Random.Range(0, 9999f);
            offsetY = Random.Range(0, 9999f);
        }
        else if (isClient)
        {
            Debug.Log("Soy solo un cliente");
            Debug.Log("Creacion de terreno en cliente con: " + offsetX + ", " + offsetY);
        }

       
            
        
       // RpcTerrain(offsetX, offsetY);
        terrain = GetComponent<Terrain>(); //Accedemos al objeto Terreno
        //Crearemos un nuevo terreno a partir de uno nuevo 
        if (GenerateTerrain(terrain.terrainData) != null)
            Debug.Log("Terreno creado");  
         //terrain.terrainData.SetHeights(0, 0, heights); //Se asigna el array de alturas al terreno  

        if (isClient)
        {
            CmdSpawnTree();
        }
       
       Debug.Log("Árboles creados");*/

    }

    public override void OnStartClient()
    {
        
        if (isServer)
        {
            offserValuesSetted = SetOffsetValues();
        }
        else if (isClient)
        {
            Debug.Log("Soy solo un cliente");
            Debug.Log("Creacion de terreno en cliente con: " + offsetX + ", " + offsetY);
        }
    }

    void Update()
    { /*ESTO ES SOLO PARA TESTEAR EN TIEMPO REAL!!!!!!!!!*/
        /*Terrain terrain = GetComponent<Terrain>(); //Accedemos al objeto Terreno
        terrain.terrainData = GenerateTerrain(terrain.terrainData); //Crearemos un nuevo terreno a partir de un nuevo metodo*/
    }

    bool SetOffsetValues()
    {
        offsetX = Random.Range(0, 9999f);
        offsetY = Random.Range(0, 9999f);
        return true;
    }
    
    void BuildTerrain(bool offserValuesSetted)
    {
        if (!offserValuesSetted) return;

        terrain = GetComponent<Terrain>(); //Accedemos al objeto Terreno

        //Crearemos un nuevo terreno a partir de uno nuevo 
        if (GenerateTerrain(terrain.terrainData) != null)
        {
            Debug.Log("Terreno creado");
        }

        CmdSpawnTree();

        Debug.Log("Árboles creados");

        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

  /* [ClientRpc] //El servidor envia un mensaje a todos los clientes, pero queremos que solo 1 respawnee
    void RpcTerrain(float offX, float offY)//Todos van a llamar a este metodo...
    {
        if (isLocalPlayer)
        {
            offsetX = offX;
            offsetY = offY;
        }*/
       

      /*  if (isLocalPlayer) //...pero solo el 1 lo ejecutara
        {
          Debug.Log("Creacion de terreno en cliente con: " + offsetX + ", " + offsetY);
            offsetX = offX;
            offsetY = offY;
            heights = new float[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float perlinValue;

                    float distance_x = Mathf.Abs(x - width * 0.5f);
                    float distance_y = Mathf.Abs(y - height * 0.5f);
                    float distance = Mathf.Max(distance_x, distance_y); 

                    float max_width = width * 0.5f - 10.0f;
                    float delta = distance / max_width;
                    float gradient = delta * delta;

                    float xCoord = (float)x / width * scale + offsetX;
                    float yCoord = (float)y / height * scale + offsetY;
                    perlinValue = (Mathf.PerlinNoise(xCoord * 4, yCoord * 4) + 0.5f * Mathf.PerlinNoise(xCoord * 8, yCoord * 8) + 0.25f * Mathf.PerlinNoise(xCoord * 16, yCoord * 16)) * Mathf.Max(0.0f, 1.0f - gradient);

                    heights[x, y] = Mathf.Pow(Mathf.Round(perlinValue * 32) / 32, 3);
                }
            }
            terrain = GetComponent<Terrain>();
            terrain.terrainData.heightmapResolution = width + 1;
            terrain.terrainData.size = new Vector3(width, depth, height);
            terrain.terrainData.SetHeights(0, 0, heights);
            
        }
    }*/

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeights(0, 0, GenerateHeights());
        //spawnTree();
        return terrainData;
    }


    float[,] GenerateHeights()
    {
        //Para las alturas, este metodo devuelve un array de dos floats correspondientes al ruido

        heights = new float[width, height]; //Tenemos una cuadricula de puntos, en los cuales cada punto tiene asociado un float que determina su altura, usando Perlin Noise
        //Debug.Log(Random.Range(0, 10));
        //Hacemos un bucle para cada uno de estos puntos
        for (int x=0; x < width; x++)
        {
            for (int y=0; y<height; y++)
            {

                //heights[x, y] = Mathf.Round(CalculateHeights(x, y) * 21) / 21; //Para crear pequenyas terrazas, sirve para escalar montanyas
               
                heights[x, y] = Mathf.Pow(Mathf.Round(CalculateHeights(x, y) * 128) / 128,3);
               // heights[x, y] = Mathf.Pow(CalculateHeights(x, y), 3);

                // Debug.Log("x: " + x +"| y: "+y+ "| altura: "+heights[x,y] );
                /*  if(x==360 && (y >= 360 && y<=365))
                  {
                      SyncListHeightsClient.Add(heights[x, y]);
                      Debug.Log("NANI??? "+ SyncListHeightsClient.IndexOf(y));
                  }*/
                // Debug.Log("indice: "+heightsClient.IndexOf(y) + " | altura: "+ heightsClient.GetItem(y));
                //heights[x, y] = Mathf.Round(CalculateHeights(x, y) * 12) / 12;
                //heights[x, y] = Mathf.Pow(CalculateHeights(x, y),3);
                //e = e + a - b*d^c  ...... a=0.05 b=1 c=2.4
                // d = 2*max(abs(nx), abs(ny))

                //e = (e + 0.05 ) * (1 - 1.00 *pow(d, 2.00 ))

            }
        }

        // ApplyMountains(); *******************************************************************************************************************************************************************************
        return heights;
    }

    float CalculateHeights(int x, int y)
    {
        /*float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;
        
        return Mathf.PerlinNoise(xCoord, yCoord);*/
        float perlinValue;

        float distance_x = Mathf.Abs(x - width * 0.5f);
        float distance_y = Mathf.Abs(y - height * 0.5f);
       // float distance = Mathf.Sqrt(distance_x * distance_x + distance_y * distance_y); // circular mask
        float distance = Mathf.Max(distance_x, distance_y); // square mask

        float max_width = width * 0.5f - 10.0f;
        float delta = distance / max_width;
        float gradient = delta * delta;

        //noise *= Mathf.Max(0.0f, 1.0f - gradient);
        /***ESTE ES EL BUENO***/
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;
        //        return Mathf.PerlinNoise(xCoord * 4, yCoord * 4) + 0.5f * Mathf.PerlinNoise(xCoord * 8, yCoord * 8) + 0.25f * Mathf.PerlinNoise(xCoord * 16, yCoord * 16);
        perlinValue = (Mathf.PerlinNoise(xCoord * 4, yCoord * 4) + 0.5f * Mathf.PerlinNoise(xCoord * 8, yCoord * 8) + 0.25f * Mathf.PerlinNoise(xCoord * 16, yCoord * 16)) * Mathf.Max(0.0f, 1.0f - gradient);

       // SyncListHeightsClient.Add(perlinValue);
        //if (x==360 && (y >= 360 && y<=370))
        //{
           // SyncListHeightsClient.Add(perlinValue);
           // Debug.Log("SIN LISTA: "+perlinValue);
            //Debug.Log(SyncListHeightsClient[0]);
       // }
        return perlinValue;

       
    }

   

}

