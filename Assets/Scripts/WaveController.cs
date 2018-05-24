using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WaveController : NetworkBehaviour {

    public Renderer water;
  //  public float scale = 0.1f;
    public float speed;
    public float noiseStrength;
    public float noiseWalk;

    private Vector3[] baseHeight;

    [SyncVar]
    float time; //Sincronizando el tiempo se sincroniza la animacion que se realiza al establecer la posicion de los vertices del plano en el eje Y entre todos los jugadores

    void Update()
    {
        if (isServer) time = Time.time;

        water.material.mainTextureOffset = new Vector2(Time.time / 50, Time.time / 70); // antes 100
        water.material.SetTextureOffset("_DetailAlbedoMap", new Vector2(0, Time.time / 80)); // antes 80

        Mesh mesh = GetComponent<MeshFilter>().mesh;

        if (baseHeight == null)
            baseHeight = mesh.vertices;

        Vector3[] vertices = new Vector3[baseHeight.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = baseHeight[i];
            vertex.y += Mathf.PerlinNoise(baseHeight[i].x + noiseWalk, baseHeight[i].y + Mathf.Sin(time * 0.1f)) * noiseStrength;
            vertices[i] = vertex;
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
