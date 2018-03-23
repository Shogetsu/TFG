using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class textureRandom : NetworkBehaviour {

    public bool tree;
    public bool palm;

    public Texture green;
    public Texture blue;

    [SyncVar]
    int rnd;

    Texture mainTex;
    Renderer m_Renderer;
    // Use this for initialization
    void Start () {
        if (!isServer)
        {
            return;
        }

        mainTex = getTextureRandom();

        transform.GetChild(0).GetComponent<Renderer>().material.SetTexture("_MainTex", mainTex);
        transform.GetChild(1).GetComponent<Renderer>().material.SetTexture("_MainTex", mainTex);
        Debug.Log("Texturas asignadas");
        
       
    }


    Texture getTextureRandom()
    {
        Texture[] textures = {
            green, blue
        };

        rnd = Random.Range(0, 2);
        return textures[rnd];
    }
	// Update is called once per frame
	void Update () {
		
	}
}
