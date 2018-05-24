using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    [SerializeField]
    [SyncVar]
    int vit;

    [SerializeField]
    [SyncVar]
    int def = 0;


   /* [SyncVar]
    bool coroutineIsRunning;*/

    // public int vitMAX = 150;

    public RectTransform healthBar;



    void Start () {
        if (!isLocalPlayer)
            return;
       // CmdSetVITMAX();
    }
	
   /* [Command]
    public void CmdSetVITMAX()
    {
        vit = vitMAX;
    }
    */

    [Command]
    public void CmdSetDef(int newDef)
    {
        def = newDef;
    }
    
    public int GetVit()
    {
        return vit;
    }

    public void TakeDamage(int damage)
    {
        //Este metodo siempre se ejecutara en el lado del servidor (command o con if(isServer))

        int newDamage = damage - def; //Se aplica la defensa adicional del jugador debido a la armadura equipada (si def=0 el damage sera el mismo, en caso de plantas y animales siempre sera asi)

        //Este metodo solo se ejecuta a traves de un Command que envia un jugador con autoridad
        Debug.Log("Bajando la vida... damage: "+newDamage);
        vit = vit - newDamage;

        if (GetComponent<Inventory>() != null) //Solo los jugadores pueden tener inventario y, por tanto, armaduras, en caso contrario se trata de un animal o planta
        {
            GetComponent<Inventory>().LosingColorLevelArmor(damage); //Si el jugador que sufre danyo lleva armadura equipada, esta se vera danyada disminuyendo su colorLevel
        }

        if (healthBar != null) //healthBar NO es null cuando se trata de un jugador
        {
            RpcUpdateHealthBar();
        }

        if (vit <= 0 && healthBar==null) //healthBar es null cuando se trata de cualquier elemento que no sea un jugador...
        {
            //... por lo que soltaran objetos y se destruiran posteriormente
            GetComponent<Drop>().DropItem(); 
            NetworkServer.Destroy(this.gameObject);
        }

        if (GetComponent<AnimalIA>() != null) //Si se trata de un animal, este entrara en el estado de huida al sufrir danyo
        {
            GetComponent<AnimalIA>().SetState("runAway");
        }
    }

    [ClientRpc]
    void RpcUpdateHealthBar()
    {
        //Debug.Log("Me queda: " + vit + " de vida");
        healthBar.sizeDelta = new Vector2(vit, healthBar.sizeDelta.y);
    }

    [Command]
    public void CmdStartLosingHealth()
    {
        //coroutineIsRunning = true;
        StartCoroutine(CmdLosingHealth());
    }

    /*public void StopLosingHealth()
    {
        StopCoroutine(CmdLosingHealth());
    }*/

    IEnumerator CmdLosingHealth()
    {
        for (; ; )
        {
            if (vit > 0 && GetComponent<ColorLevel>().GetColorLevel()<=0)
            {
                vit = vit - 1;
                RpcUpdateHealthBar();
            }
            else
            {
                RpcUpdateHealthBar();
                StopCoroutine(CmdLosingHealth());
                print("Stopped " + Time.time);
                //coroutineIsRunning = false;

                if(GetComponent<ColorLevel>().GetColorLevel()>0)
                    GetComponent<ColorLevel>().CmdStartCoroutine();

                break;
            }
        yield return new WaitForSeconds(0.1f); //PONER 1f!!!!
        }
    }

    [Command]
    public void CmdHeal(int heal)
    {
        vit = vit + heal;
        RpcUpdateHealthBar();
    }

    /*public bool CheckCoroutine()
    {
        return coroutineIsRunning;
    }
    */
}
