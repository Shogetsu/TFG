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
        //Debug.Log("Duele");
        if (healthBar != null)
        {
            Debug.Log("Me queda: " + vit + " de vida");
            healthBar.sizeDelta = new Vector2(vit, healthBar.sizeDelta.y);
        }
    }

    public void CmdStartLosingHealth()
    {
        //Esta co-rutina se ejecuta a traves de un Command en ColorLevel
        StartCoroutine(CmdLosingHealth());
    }

    public void StopLosingHealth()
    {
        StopCoroutine(CmdLosingHealth());
    }

    IEnumerator CmdLosingHealth()
    {
        for (; ; )
        {
            if (vit > 0)
            {
                vit = vit - 1;
                RpcUpdateHealthBar();
            }
            else
            {
                RpcUpdateHealthBar();
                StopCoroutine(CmdLosingHealth());
                print("Stopped " + Time.time);
                break;
            }
        yield return new WaitForSeconds(0.1f);
        }
    }

}
