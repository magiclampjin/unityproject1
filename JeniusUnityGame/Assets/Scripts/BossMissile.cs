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
        Invoke("Bombing",5f); //5�� �� �ڵ�����
    }

    // Update is called once per frame
    void Update()
    {
        nav.SetDestination(target.position);
    }

    void Bombing() //5�ʵ��� �÷��̾�� �������� ���ϸ� �ڵ�����
    {
        Destroy(this.gameObject);
    }
}
