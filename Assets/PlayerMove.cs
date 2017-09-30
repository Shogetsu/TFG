using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking; //LIBRERIA IMPORTANTE PARA ACTUALIZAR LAS TRANSFORMACIONES EN CADA CLIENTE DE FORMA INDIVIDUAL
using UnityEngine;

public class PlayerMove : NetworkBehaviour { //IMPORTANTE heredar de NetworkBehavior para emplear nuevos metodos para ONLINE

    public GameObject bulletPrefab;

    public override void OnStartLocalPlayer() //Se sobrescribe un metodo el cual SOLO se ejecuta en su propio cliente
    {
        /*Esto sirve, por ejemplo, para diferenciar a tu jugador/personaje LOCAL del resto*/
        GetComponent<MeshRenderer>().material.color = Color.red;

        /*Es en este punto donde harias que la camara siguiera a tu personaje local*/
    }
    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer) //Metodo de NetworkBehavior - Si no es el local player, se destruye el script y no habra acceso a las teclas
            return;

        float x = Input.GetAxis("Horizontal")* 0.1f;
        float z = Input.GetAxis("Vertical")* 0.1f;

        transform.Translate(x, 0, z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire(); //prefijo Cmd importante, sin esto las balas solo las verian cada cliente individualmente
        }
    }

    [Command] //Command (Cmd) el cliente le dice al servidor que ejecute el metodo, el metodo SOLO se ejecuta en el server
    void CmdFire()
    {
        GameObject bullet = (GameObject)Instantiate(
                                bulletPrefab,
                                transform.position - transform.forward, //empieza un poco atras
                                Quaternion.identity);

        bullet.GetComponent<Rigidbody>().velocity = -transform.forward * 4.0f; //velocidad inicial= 4 (velocidad constante)

        NetworkServer.Spawn(bullet); //IMPORTANTE al ejecutar el metodo el servidor instancia la bala en cada uno de los clientes
        //Si no se hiciese esto ultimo, solo se verian las balas en el servidor

        Destroy(bullet, 10.0f); //Destruccion temporizada... tiempo de destruccion= 10

        /*El comportamiento de disparo deberia de estar en otro script, de tal manera que seria facil deshabilitar el disparo de un jugador (cooldown)*/
        //El tiempo de destruccion y la velocidad inicial serian variables de ese script
    }
}
