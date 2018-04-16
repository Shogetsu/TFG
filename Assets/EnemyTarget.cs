using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class EnemyTarget : NetworkBehaviour {

    NavMeshAgent agent;
    Transform myTransform;
    Transform targetTransform;
    LayerMask raycastLayer;
    float radius = 10;

	// Use this for initialization
	void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.height = 0.5f;
        agent.baseOffset = 0;
        myTransform = transform;
        raycastLayer = 1 << LayerMask.NameToLayer("Player");
	}
	
	// Para la IA es recomendable utilizar FixedUpdate
	void FixedUpdate ()
    {
        if (!isServer)
            return;

        SearchToTarget();
        MoveToTarget();
	}

    void SearchToTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(myTransform.position, radius, raycastLayer);

        if (targetTransform == null)
        {
            //Array de colliders que colisionen con una esfera creada por el enemigo a su alrededor (myTransform.position) con un radio (radius), afectando unicamente a una determinada capa (raycastLayer, capa "Player")
            
            if (hitColliders.Length > 0)
            {
                int randomInt = Random.Range(0, hitColliders.Length);
                targetTransform = hitColliders[randomInt].transform;
            }
        }
        else
        {
            if (hitColliders.Length == 0) //No esta viendo a nadie
            {
                targetTransform = null;
            }
        }

        

        //Si el enemigo tiene un target pero esta "muerto" (capsuleCollider.enabled==false) el target se resetea a null ... en este videojuego cualquier un jugador muere, se acaba la partida, asi que ESTO NO TIENE UTILIDAD POR EL MOMENTO... pero tendre que hacer que se resetee a null el target cuando se salga del rango de vision del enemigo
     
        /* if (targetTransform != null && targetTransform.GetComponent<CapsuleCollider>().enabled==false)
        {
            targetTransform = null;
        }*/
    }

    void MoveToTarget()
    {
        if (targetTransform != null)
        {
            Debug.Log(targetTransform.gameObject.name);
            SetNavDestination(targetTransform);
        }
    }

    void SetNavDestination(Transform dest)
    {
        agent.SetDestination(dest.position);
    }
}
