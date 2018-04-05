using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    [SyncVar]
    public int vit;

    public RectTransform healthBar;

	void Start () {

    }
	
    public void TakeDamage()
    {
        //Este metodo solo se ejecuta a traves de un Command que envia un jugador con autoridad
        Debug.Log("Bajando la vida...");
        vit = vit - 1;
        RpcUpdateHealthBar();
    }

    [ClientRpc]
    void RpcUpdateHealthBar()
    {
        Debug.Log("Duele");
        if (healthBar != null)
        {
            healthBar.sizeDelta = new Vector2(vit, healthBar.sizeDelta.y);
        }
    }

}
