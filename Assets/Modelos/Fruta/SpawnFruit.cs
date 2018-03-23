using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class SpawnFruit : NetworkBehaviour {

    public GameObject fruit;
    int spawnNum = 8;

    // Use this for initialization
    void Start()
    {
        if (isServer)
        {
            CmdSpawn();
        }
    }

    [Command]
    void CmdSpawn()
    {
        for(int i=0; i<spawnNum; i++)
        {
            //La posicion de la fruta va en funcion del arbol/arbusto (this) con un pequenyo valor aleatorio
            Vector3 fruitPos = new Vector3(this.transform.position.x + Random.Range(-1.0f, 1.0f), this.transform.position.y + Random.Range(0.0f, 2.0f), this.transform.position.z + Random.Range(-1.0f, 1.0f));
            GameObject fruitSpawn = Instantiate(fruit, fruitPos, Quaternion.identity);
            NetworkServer.Spawn(fruitSpawn);
        }
    }



}
