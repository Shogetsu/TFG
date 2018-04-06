using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ColorLevel : NetworkBehaviour {
    [SyncVar]
    public int colorLevel;

    public RectTransform colorLevelBar;

    // Use this for initialization
    void Start () {
        if (!isLocalPlayer)
            return;

        //if(GetComponent<PjControl>().isGrounded) //En el momento en el que el jugador toque el suelo, empieza a perder nivel de color | Esto no parece que funcione
            CmdStartCoroutine();
    }

    // Update is called once per frame
    void Update () {

    }

    [Command]
    void CmdStartCoroutine()
    {
        //Las co-rutinas deben ser ejecutadas a traves del servidor
        StartCoroutine(CmdLosingColorLevel());
    }

    IEnumerator CmdLosingColorLevel()
    {
        for (; ; )
        {
            if (colorLevel > 0)
            {
                colorLevel = colorLevel - 1;
                RpcUpdateColorLevelBar();
            }
            else
            {
                RpcUpdateColorLevelBar();
                StopCoroutine(CmdLosingColorLevel());
                print("Stopped " + Time.time);
                GetComponent<Health>().CmdStartLosingHealth(); //Cuando el nivel de color llega a 0, se empieza a perder vida
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    [ClientRpc]
    void RpcUpdateColorLevelBar()
    {
        if (colorLevelBar != null)
        {
            colorLevelBar.sizeDelta = new Vector2(colorLevel, colorLevelBar.sizeDelta.y);
        }
    }
}
