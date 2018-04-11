using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour {

    public int respawnTime = 5; //Para asignar un tiempo distinto de reaparicion a cada tipo de fruta

     void OnCollisionEnter() //con rigidbody
     {
         //Al colisionar con la fruta, desaparece su maya y colision
         //this.GetComponent<SphereCollider>().enabled = false;
         this.GetComponent<MeshRenderer>().enabled = false;
        
         Invoke("BoomRespawn", respawnTime); //A los 5 seg, reaparece
     }

    void OnTriggerEnter(Collider other) //con character controller
    {
        other.gameObject.GetComponent<Pickup>().PickItem(GetComponent<Collider>()); //el personaje coge el objeto


        //Al colisionar con la fruta, desaparece su maya y colision
        this.GetComponent<SphereCollider>().enabled = false;
        this.GetComponent<MeshRenderer>().enabled = false;

        Invoke("BoomRespawn", respawnTime); //A los 5 seg, reaparece
    }

    void BoomRespawn()
    {
        //this.GetComponent<SphereCollider>().enabled = true;
        this.GetComponent<MeshRenderer>().enabled = true;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
