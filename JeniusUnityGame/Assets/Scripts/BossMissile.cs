using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class BossMissile : Bullet //Bullet.cs 그대로 사용하면서 상속해서 사용
{
    public Transform target;
    NavMeshAgent nav;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        Invoke("Bobming", 5); //5초 후 미사일이 자동으로 사라짐.
    }

    // Update is called once per frame
    void Update()
    {
        nav.SetDestination(target.position);
    }

    void Bobming()
    {
        Destroy(this.gameObject);
    }
}
