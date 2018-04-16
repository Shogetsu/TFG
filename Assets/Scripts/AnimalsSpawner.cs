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
    int numberOfAnimals = 3;

    public override void OnStartServer()
    {
        for(int i=0; i<numberOfAnimals; i++)
        {
            SpawnAnimals();
        }
    }

    void SpawnAnimals()
    {
        //counter

       GameObject go = GameObject.Instantiate(animalPrefab, animalSpawn.transform.position, Quaternion.identity) as GameObject;
       NetworkServer.Spawn(go);
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
