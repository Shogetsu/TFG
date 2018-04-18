using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider : MonoBehaviour
{

    public GameObject character;

    void Start()
    {
        GetComponent<Collider>().isTrigger = false;
    }

    void Update()
    {
        ActiveTrigger();
    }

    void ActiveTrigger()
    {
        if (character.GetComponent<Animator>() != null)
        {
            if (character.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Hitting"))
                GetComponent<Collider>().isTrigger = true;
            else
                GetComponent<Collider>().isTrigger = false;

        }else if(character.transform.GetChild(0).GetComponent<Animator>() != null)
        {
            if (character.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Hitting"))
                GetComponent<Collider>().isTrigger = true;
            else
                GetComponent<Collider>().isTrigger = false;
        }
    }

}
