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
    float radius = 100;

	// Use this for initialization
	void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
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
        if (targetTransform == null)
        {
            //Array de colliders que colisionen con una esfera creada por el enemigo a su alrededor (myTransform.position) con un radio (radius), afectando unicamente a una determinada capa (raycastLayer, capa "Player")
            Collider[] hitColliders = Physics.OverlapSphere(myTransform.position, radius, raycastLayer);

            if (hitColliders.Length > 0)
            {
                int randomInt = Random.Range(0, hitColliders.Length);
                targetTransform = hitColliders[randomInt].transform;
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
            SetNavDestination(targetTransform);
        }
    }

    void SetNavDestination(Transform dest)
    {
        agent.SetDestination(dest.position);
    }
}
