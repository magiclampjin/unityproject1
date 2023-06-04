using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class BossMissile : Bullet //Bullet.cs �״�� ����ϸ鼭 ����ؼ� ���
{
    public Transform target;
    NavMeshAgent nav;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        Invoke("Bobming", 5); //5�� �� �̻����� �ڵ����� �����.
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
