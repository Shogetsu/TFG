using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AnimalsSpawner : NetworkBehaviour {

    [SerializeField]
    GameObject animalPrefab;

    [SerializeField]
    GameObject animalSpawn;

    int counter;

    [SerializeField]
    int numberOfAnimals = 1;

    public override void OnStartServer()
    {
       /* for(int i=0; i<numberOfAnimals; i++)
        {
            SpawnAnimals();
        }*/
    }

    void SpawnAnimals()
    {
        Vector3 origen = new Vector3(Random.Range(2,1024), 0, Random.Range(2, 1024));
        //animalSpawn.transform.position
        GameObject go = GameObject.Instantiate(animalPrefab, origen, Quaternion.identity) as GameObject;
       NetworkServer.Spawn(go);
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
