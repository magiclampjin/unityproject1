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
        Invoke("Bombing",5f); //5초 후 자동삭제
    }

    // Update is called once per frame
    void Update()
    {
        nav.SetDestination(target.position);
    }

    void Bombing() //5초동안 플레이어에게 도달하지 못하면 자동삭제
    {
        Destroy(this.gameObject);
    }
}
