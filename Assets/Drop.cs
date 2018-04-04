using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Drop : NetworkBehaviour {

    public GameObject item;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       /* if (this.gameObject.GetComponent<Health>().vit <= 0)
        {
            CmdDestroyGameObject();
        }*/
	}

   /* [Command]
    void CmdDestroyGameObject()
    {
        NetworkServer.Destroy(this.gameObject);
    }*/

    public void DropItem()
    {
        GameObject itemDropped = Instantiate(item, this.gameObject.transform.position, Quaternion.identity);
        NetworkServer.Spawn(itemDropped);
    }
}
