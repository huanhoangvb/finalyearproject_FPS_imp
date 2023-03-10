using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class companionAI : MonoBehaviour
{
    private NavMeshAgent navmesh;
    private float detectionRadius = 15f;
    private GameObject detectedGameObject;
    private bool isCoroutineExecuting = false;

    // Start is called before the first frame update
    void Start()
    {
        navmesh = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (DetectEnemy())
            StartCoroutine(AttackEnemy());    
    }

    private bool DetectEnemy() 
    {
        Vector3 center = transform.position;
        int maxColliders = 10; 
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(center, detectionRadius, hitColliders);
        if (numColliders > 0)
        {
            for (int i = 0; i < numColliders; i++)
            {
                if (hitColliders[i].CompareTag("Enemy"))
                {
                    detectedGameObject = hitColliders[i].gameObject;
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator AttackEnemy()
    {
        if (isCoroutineExecuting)
            yield break;

        isCoroutineExecuting = true;

        yield return new WaitForSeconds(1);

        if (detectedGameObject != null)
        {
            if (detectedGameObject.TryGetComponent<EnemyHealthSystem>(out EnemyHealthSystem enemyHealth))
            {
                Vector3 enemyPos = detectedGameObject.transform.position;
                navmesh.destination = enemyPos;
                if (Vector3.Distance(enemyPos, transform.position) < 5f)
                {
                    GetComponent<Animator>().SetBool("Attacking", true);
                    enemyHealth.takeDamage(10);
                }
            }
        }
        isCoroutineExecuting = false;
    }
}
