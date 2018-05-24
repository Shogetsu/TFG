using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ActivePointLight : NetworkBehaviour {

    [SyncVar (hook="UpdateLight")]
    bool active;

    DayNightCycle whatTime;
    float endTime;
    int totalTime;

	// Use this for initialization
	void Start () {
        if (GameObject.Find("GameManager")!=null)
            whatTime = GameObject.Find("GameManager").GetComponent<DayNightCycle>();

        if (isServer)
            active = false;

        SetupLightEmission();

    }

    void SetupLightEmission()
    {
        this.gameObject.transform.Find("PointLight").GetComponent<Light>().color = GetComponent<SetupLocalPlayer>().playerColor;
        //Color newcolor = new Color(GetComponent<SetupLocalPlayer>().playerColor.r + 0.5f, GetComponent<SetupLocalPlayer>().playerColor.g, GetComponent<SetupLocalPlayer>().playerColor.b);
        this.gameObject.transform.Find("pjRigSkin2facesPrefab").transform.Find("Object001").GetComponent<Renderer>().material.SetColor("_EmissionColor", GetComponent<SetupLocalPlayer>().playerColor);
        this.gameObject.transform.Find("pjRigSkin2facesPrefab").transform.Find("Object002").GetComponent<Renderer>().material.SetColor("_EmissionColor", GetComponent<SetupLocalPlayer>().playerColor);
    }

    // Update is called once per frame
    void Update () {

        if(whatTime==null && GameObject.Find("GameManager") != null)
            whatTime = GameObject.Find("GameManager").GetComponent<DayNightCycle>();

        if (active)
            TimeLight();

	}

    [Command]
    public void CmdActiveLight(bool a)
    {
        active = a;
        //RpcUpdateLight(active);
    }

    //[ClientRpc]
    void UpdateLight(bool a)
    {
        if (whatTime == null) return;

        this.gameObject.transform.Find("PointLight").GetComponent<Light>().enabled = a;

        if (a)
        {
            this.gameObject.transform.Find("pjRigSkin2facesPrefab").transform.Find("Object001").GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            this.gameObject.transform.Find("pjRigSkin2facesPrefab").transform.Find("Object002").GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

            endTime = GetTotalTime() + 3600; // 3600s = 1h dura la iluminacion | El tiempo total transcurrido es importante para evitar bug cuando termina el dia (a las 00:00 la variable time se resetea a 0)
        }
        else
        {
            this.gameObject.transform.Find("pjRigSkin2facesPrefab").transform.Find("Object001").GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
            this.gameObject.transform.Find("pjRigSkin2facesPrefab").transform.Find("Object002").GetComponent<Renderer>().material.DisableKeyword("_EMISSION");

            endTime = 0;
        }
    }

    void TimeLight()
    {
        if (GetTotalTime() > endTime)
        {
            CmdActiveLight(false);
           
        }
    }

    float GetTotalTime()
    {
        //Obtener el tiempo total transcurrido
        return (float)whatTime.time + (whatTime.HourToSeconds(24) * whatTime.days);
    }

    public bool CheckActive()
    {
        return active;
    }
}
