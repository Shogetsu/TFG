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
        StartCoroutine("LosingColorLevel");
    }

    // Update is called once per frame
    void Update () {

    }

    IEnumerator LosingColorLevel()
    {

        for (; ; )
        {
            CmdLosingColorLevel();
            yield return new WaitForSeconds(2f);
        }
    }

    [Command]
    void CmdLosingColorLevel()
    {
        colorLevel = colorLevel - 1;
        RpcUpdateColorLevelBar();
    }

    [ClientRpc]
    void RpcUpdateColorLevelBar()
    {
        Debug.Log("Duele");
        if (colorLevelBar != null)
        {
            colorLevelBar.sizeDelta = new Vector2(colorLevel, colorLevelBar.sizeDelta.y);
        }
    }
}
