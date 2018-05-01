using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ColorLevelRefuge : NetworkBehaviour {

    [SerializeField]
    [SyncVar]
    int colorLevel;

    [SyncVar]
    bool isGrounded;

    // Use this for initialization
    void Start () {
        if (!isServer) return;

        FabricableItem refuge = GetComponent<ItemPickup>().item as FabricableItem;
        colorLevel = refuge.colorLevel;
        isGrounded = false;

    }

    // Update is called once per frame
    void Update () {

	}

    private void OnParticleCollision(GameObject other)
    {
        if (!isServer) return;

        if (other.name.Equals("Rain") && colorLevel > 0)
        {
            //Debug.Log("Lluvia duele");
            colorLevel = colorLevel - 1;
        }

        if (colorLevel <= 0)
            NetworkServer.Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isServer) return;

        if (collision.gameObject.CompareTag("Refuge") && isGrounded)
        {
            NetworkServer.Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
        }
    }

}
