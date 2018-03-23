using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Prueba : NetworkBehaviour{

	// Use this for initialization
	void Start () {
        Debug.Log(Network.connections.Length);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
