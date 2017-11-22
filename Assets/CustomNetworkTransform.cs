using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkTransform : NetworkBehaviour {

    //Se sincronizan posicion y rotacion en el servidor
    [SyncVar]
    Vector3 SyncPos; //Posicion
    [SyncVar]
    Quaternion SyncRot; //Rotacion

    public Transform myTransform;
    public float LerpRate = 15f;

    private void FixedUpdate()
    {
        TransmitirTransform(); //Se transmite la posicion/rotacion al cliente, y se lo envia al servidor con el Cmd
        Lerp(); //Interpolacion de los jugadores que no sean el jugador local, para suavizar
    }

    void Lerp()
    {
        if (!isLocalPlayer)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, SyncPos, Time.deltaTime * LerpRate);
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, SyncRot, Time.deltaTime * LerpRate);
        }
    }

    //Se envia la informacion al servidor
    [Command]
    void CmdTransformAlServidor(Vector3 pos, Quaternion rot)
    {
        SyncPos = pos;
        SyncRot = rot;
    }

    //Se ejecuta solo en los clientes
    [ClientCallback]
    void TransmitirTransform()
    {
        if (isLocalPlayer)
        {
            CmdTransformAlServidor(transform.position, transform.rotation);
        }
    }
}
