using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Combat : NetworkBehaviour {
    /*LA VIDA DEBE DE ESTAR SINCRONIZADA!!!
     * Ejemplos: mana, aturdido (boolean)...
     Sino pueden mostrarse diferentes vidas en los diferentes clientes*/
     //El servidor es quien debe tocar las variables y decidir que hacer, el cliente SOLO envia la peticion
    public const int maxHealth = 100;
    
    [SyncVar] //Sincronizacion en todos los clientes al mismo tiempo! pasa variable al servidor
    public int health = maxHealth;

    public void TakeDamage(int amount)
    {
        if (!isServer) //Este script SOLO lo ejecutara el servidor, sino el script muere
            return;


        health -= amount;
        if (health <= 0)
        {
            health = maxHealth;
            RpcRespawn();
        }
    }

    [ClientRpc] //El servidor envia un mensaje a todos los clientes, pero queremos que solo 1 respawnee
    void RpcRespawn()//Todos van a llamar a este metodo...
    {
        if (isLocalPlayer) //...pero solo el 1 lo ejecutara
        {
            transform.position = Vector3.zero;
        }
    }

}
