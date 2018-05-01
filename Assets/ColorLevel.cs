using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ColorLevel : NetworkBehaviour {

    [SerializeField]
    [SyncVar]
    int colorLevel;

    public int colorLevelMAX = 150;

    public RectTransform colorLevelBar;

    // Use this for initialization
    void Start () {
        if (!isLocalPlayer)
            return;

        //if(GetComponent<PjControl>().isGrounded) //En el momento en el que el jugador toque el suelo, empieza a perder nivel de color | Esto no parece que funcione
        CmdSetColorLevelMAX();
        CmdStartCoroutine();
    }

    [Command]
    void CmdSetColorLevelMAX()
    {
        colorLevel = colorLevelMAX;
    }

    // Update is called once per frame
    void Update () {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!isServer) return;

        if (other.name.Equals("Rain") && colorLevel>0)
        {
            Debug.Log("Lluvia duele");
            colorLevel = colorLevel - 2;
            RpcUpdateColorLevelBar();
        }
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
            yield return new WaitForSeconds(1f);
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

    [Command]
    public void CmdModifyColorLevel(int cl)
    {
        colorLevel = colorLevel + cl;
        RpcUpdateColorLevelBar();
    }
}
