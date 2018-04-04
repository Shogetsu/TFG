using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider : MonoBehaviour
{

    public GameObject player;

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
        if (player.GetComponent<PjControl>().GetAnimator() != null)
        {
            if (player.GetComponent<PjControl>().GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("Hitting"))
                GetComponent<Collider>().isTrigger = true;
            else
                GetComponent<Collider>().isTrigger = false;
        }
    }

}
