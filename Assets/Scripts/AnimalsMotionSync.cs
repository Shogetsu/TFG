﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AnimalsMotionSync : NetworkBehaviour {

    [SyncVar]
    Vector3 syncPos;
    [SyncVar]
    float syncYRot;

    Vector3 lastPos;
    Quaternion lastRot;
    Transform myTransform;
    float lerpRate = 10;
    float posThreshold = 0.5f;
    float rotThreshold = 5;

	// Use this for initialization
	void Start ()
    {
        myTransform = transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        TransmitMotion();
        LerpMotion();
	}

    void TransmitMotion()
    {
        if (!isServer)
            return;

        if(Vector3.Distance(myTransform.position,lastPos)>posThreshold || Quaternion.Angle(myTransform.rotation, lastRot) > rotThreshold)
        {
            lastPos = myTransform.position;
            lastRot = myTransform.rotation;

            syncPos = myTransform.position;
            syncYRot = myTransform.localEulerAngles.y;
        }
    }

    void LerpMotion()
    {
        if (!isServer)
            return;

        myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);

        Vector3 newRot = new Vector3(0, syncYRot, 0);
        myTransform.rotation = Quaternion.Lerp(myTransform.rotation, Quaternion.Euler(newRot), Time.deltaTime * lerpRate);
    }
}