using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Hit : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
            Debug.Log("Golpeo a " + other.gameObject.name);
            CmdHitted(other.gameObject); //IMPORTANTE - SOLO los gameObject con NetworkId ya instanciados en el servidor son los unicos gameObject que se pueden pasar por parametro en un Command o Rcp
    }

    [Command]
    void CmdHitted(GameObject other)
    {
        if (other.gameObject.GetComponents<Health>() != null)
        {
            other.gameObject.GetComponent<Health>().TakeDamage();
            if (other.gameObject.GetComponent<Health>().vit <= 0)
            {
                other.GetComponent<Drop>().DropItem();
                NetworkServer.Destroy(other);
            }
        }
    }
}
