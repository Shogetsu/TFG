using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Hit : NetworkBehaviour {

    [SyncVar]
    public int damage = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [Command]
    public void CmdSetDamage(int dmg)
    {
        damage = dmg;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer) return;

        if (other.gameObject.GetComponent<NetworkIdentity>() != null)
        {//IMPORTANTE - SOLO los gameObject con NetworkId ya instanciados en el servidor son los unicos gameObject que se pueden pasar por parametro en un Command o Rcp, de lo contrario seran NULL
            CmdHitted(other.gameObject, damage); 
        }   
    }

    [Command]
    void CmdHitted(GameObject other, int damage)
    {
        if (other == null) return;

        if (other.gameObject.GetComponent<Health>() != null)
        {
            Debug.Log(this.gameObject.name + " golpea a " + other.name);
            other.gameObject.GetComponent<Health>().TakeDamage(damage);

             if (GetComponent<Inventory>()!=null)
                GetComponent<Inventory>().LosingColorLevelWeapon();
        }
    }
}
