using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseTerrain : MonoBehaviour {

    public int mDivisions; //En cuantas caras esta dividido el mundo
    public float mSize; //Tamanyo del mundo
    public float mHeight; //Altura maxima del terreno

	// Use this for initialization
	void Start () {
        CreateTerrain();
	}
	
    void CreateTerrain()
    {
        //Esto es como la resolucion del terreno en los otros dos scripts (+1)
        int divPlusOne = mDivisions + 1;
        //Contador de vertices
        int vertCount = divPlusOne * divPlusOne;
        //Lo primero que necesitamos son los arrays que vamos a pasarle a nuestro mesh, 
        //los vertex array que se tienen en un triangulo

        Vector3[] verts = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int[] tris = new int[mDivisions*mDivisions*6];

        float halfSize = mSize * 0.5f;
        float divisionSize = mSize / mDivisions;

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int triOffset = 0;
        for(int i=0; i <= mDivisions; i++)
        {
            for (int j = 0; j <= mDivisions; j++)
            {

                float xCoord = (float)j / mDivisions;
                float yCoord = (float)i / mDivisions;
                /*
                * Si xCoord y yCoord no se multiplican por 4, terreno muy poco curvado, casi plano.
                * Si SI se multiplican por 4, terreno con mas curvas pero muy uniforme (PERFECTO PARA DESIERTO), PERO tendra MAS detalles que de la forma anterior
                * Para agregar MAS DETALLES en pequenyas secciones del terreno hay que calcular de nuevo el PerlinNoise (sumandole al ya existente) pero multiplicando
                  todo por un valor inferior a 1 (montanyas pequenyas juntas) y las variables xCoord y yCoord por valores MAS GRANDES (mas detalles... mas zonas montanyosas)
                */

                //float height = Mathf.PerlinNoise(xCoord * 4, yCoord * 4) + 0.25f * Mathf.PerlinNoise(xCoord * 16, yCoord * 16);

                float height = Mathf.PerlinNoise(xCoord * 4, yCoord * 4) + 0.5f * Mathf.PerlinNoise(xCoord * 8, yCoord * 8) + 0.25f * Mathf.PerlinNoise(xCoord * 16, yCoord * 16);

                //Esto hace que las montanyas altas sean mas pronunciadas, y las zonas bajas mas planas
                height = Mathf.Pow(height, 15);

                height *= mHeight; //Como PerlinNoise devuelve un valor entre 0 y 1, esto incrementara la altura del terreno (es para asignar la altura maxima)

                verts[i * divPlusOne + j] = new Vector3(-halfSize + j * divisionSize, height, halfSize - i * divisionSize);
                uvs[i * divPlusOne + j] = new Vector2((float)j / mDivisions, (float)i / mDivisions);

                if(i<mDivisions && j < mDivisions)
                {
                    int topLeft = i * divPlusOne + j;
                    int botLeft = (i + 1) * divPlusOne + j;

                    tris[triOffset] = topLeft;
                    tris[triOffset + 1] = topLeft + 1;
                    tris[triOffset + 2] = botLeft + 1;

                    tris[triOffset + 3] = topLeft;
                    tris[triOffset + 4] = botLeft + 1;
                    tris[triOffset + 5] = botLeft;

                    triOffset += 6;

                }
            }
        }

        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

    }
}
