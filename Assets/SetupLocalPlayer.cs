using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class SetupLocalPlayer : NetworkBehaviour {

    [SyncVar]
    public string pname = "player";

    [SyncVar]
    public Color playerColor = Color.white;

    public string colorString;

    private void OnGUI()
    {
        if (isLocalPlayer)
        {
           /* pname = GUI.TextField(new Rect(25, Screen.height - 40, 100, 30), pname);
            if(GUI.Button(new Rect(130, Screen.height - 40, 80, 30), "Change"))
            {
                CmdChangeName(pname);
            }*/
        }
    }

    /*Esto es para cambiar de nombre dentro de la partida.... NO INTERESA (de momento)*/
    [Command]
    void CmdChangeName(string newName)
    {
        pname = newName;
        //this.GetComponentsInChildren<TextMesh>()
    }

    private void Start()
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>(); // Se obtienen todos los renders del GameObject
        foreach (Renderer r in rends)//Se recorren todos los renders y se les asigna el color del jugador en cuestion
        {
            if(r.name.Equals("itemHand")==false)
                r.material.color = playerColor;
        }
            

        SetColorString(playerColor);
    }


    void SetColorString(Color playerColor)
    {
        if (playerColor == Color.magenta)
            colorString = "Magenta";
        if (playerColor == Color.red)
            colorString = "Red";
        if (playerColor == Color.cyan)
            colorString = "Cyan";
        if (playerColor == Color.blue)
            colorString = "Blue";
        if (playerColor == Color.green)
            colorString = "Green";
        if (playerColor == Color.yellow)
            colorString = "Yellow";
    }
}
