using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class spawnTerrain : NetworkBehaviour {
    public GameObject terrainPrefab;
	// Use this for initialization
	void Start () {
            CmdSpawnTerrain();
	}

    [Command] //Command (Cmd) el cliente le dice al servidor que ejecute el metodo, el metodo SOLO se ejecuta en el server
    void CmdSpawnTerrain()
    {
        GameObject terrain = (GameObject)Instantiate(
                                terrainPrefab,
                                terrainPrefab.transform.position, 
                                Quaternion.identity);


        NetworkServer.Spawn(terrain); 
    }
}
