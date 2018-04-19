using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupAnimalPlant : NetworkBehaviour {

    string[] colorsString = new string[] { "Magenta", "Red", "Cyan", "Blue", "Green", "Yellow" };
    Color[] colors = new Color[] { Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow };

    public string colorString;
    Color color;
    public string material;

    [SyncVar]
    int randomInt;

    // Use this for initialization
    void Start()
    {
        //ESTO NO FUNCIONA, EL COLOR SOLO SE ASIGNA EN EL HOST PERO LOS CLIENTES LO VEN SIN COLOR!!!!!!!!
        CmdSetRandomColor();
    }

    [Command]
    void CmdSetRandomColor()
    {
        randomInt = Random.Range(0, colors.Length);
        RpcSetColor(randomInt);
    }

    [ClientRpc]
    void RpcSetColor(int randomInt)
    {
        color = colors[randomInt];
        colorString = colorsString[randomInt];

        Renderer[] rends = GetComponentsInChildren<Renderer>(); // Se obtienen todos los renders del GameObject
        foreach (Renderer r in rends) //Se recorren todos los renders y se les asigna el color del jugador en cuestion
            r.material.color = color;
    }
}
