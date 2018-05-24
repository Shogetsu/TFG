using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AnimalAttack : NetworkBehaviour {

    private AudioManager audioManager;

    [SerializeField] float attackRate = 3;
    float nextAttack;
    [SerializeField] int dmg = 5;
    [SerializeField] float minDistance = 2;
    float currentDistance;
    Transform myTransform;
    AnimalIA targetScript;

    
	// Use this for initialization
	void Start ()
    {
        /*myTransform = transform;
        targetScript = GetComponent<AnimalIA>();

        if (!isServer) return;

        StartCoroutine(Attack());

        Debug.Log("AL ATAQUERL!");*/

        //audioManager
        audioManager = AudioManager.instance;
    }

    private void OnEnable() //Importante usar OnEnable, este trozo de codigo se debe de ejecutar siempre que el script se active
    {
        myTransform = transform;
        targetScript = GetComponent<AnimalIA>();

        if (!isServer) return;

        StartCoroutine(Attack());

       // Debug.Log("AL ATAQUERL!");
    }

    void CheckIfTargetInRange()
    {
        if(targetScript.targetTransform != null)
        {
            currentDistance = Vector3.Distance(targetScript.targetTransform.position, myTransform.position);

            if(currentDistance<minDistance && Time.time > nextAttack)
            {
                nextAttack = Time.time + attackRate;
                targetScript.targetTransform.GetComponent<Health>().TakeDamage(dmg);
                RpcHitting(); //La animacion de golpeo de los enemigos se transmite a todos los clientes
            }
        }
    }

    [ClientRpc]
    void RpcHitting()
    {
        transform.GetChild(0).GetComponent<Animator>().SetTrigger("isHitting");
        audioManager.PlaySound("Hit01");
    }

    IEnumerator Attack()
    {
        for(; ; )
        {
            yield return new WaitForSeconds(0.2f);
            CheckIfTargetInRange();
        }
    }
}
