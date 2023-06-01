using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D }; //���� Ÿ��
    public Type enemyType;
    public int maxHealth; //�ִ�ü��
    public int curHealth; //����ü��
    public Transform target; // ���� AI�� ����ٴ� ��ǥ�� (�÷��̾�)
    public BoxCollider meleeArea;//������ ���ݹ���
    public GameObject bullet; //���Ÿ����� ���� C�� �Ѿ�
    public bool isChase; //���� �߰ݿ���
    public bool isAttack; //���� ���ݿ��� - �÷��̾���� �Ÿ��� ��������� ���� ���� -> Ÿ���� �ʿ�
    public bool isDead; //������ ��������

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if(enemyType != Type.D)
            Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {
        /* if (isChase)//��ǥ�� �Ұ� �̵��� ��ӵ�. -> ���� �ʿ�
             nav.SetDestination(target.position); 
        */

        if (nav.enabled && enemyType != Type.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase; //�Ϻ��ϰ� ����.
        }
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero; //����ȸ���ӵ��� 0���� ��������. (������ ���� ���� ����)
        }
    }

    void Targeting()
    {
        if (enemyType != Type.D && !isDead)
        {
            //ray�� ����ϵ� ray�� ����Ƚ���� ���� ���� ��. -> ���� �� ��.
            //��ü����� ĳ���� - Physics.SphereCast
            float targetRadius = 0; //�β�(��Ȯ��)
            float targetRange = 0; //����(����)

            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;

                case Type.B:
                    targetRadius = 1f; //������ A���� ��Ȯ�ϰ�
                    targetRange = 12f; //���ݹ��� A�� 4��
                    break;

                case Type.C:
                    targetRadius = 0.5f; //���� ���� ��Ȯ�ϰ�
                    targetRange = 25f; //���ݹ��� ���� ���
                    break;
            }

            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
                                                        targetRadius,
                                                        Vector3.forward,
                                                        targetRange,
                                                        LayerMask.GetMask("Player"));

            //Debug.DrawRay(transform.position, transform.forward * targetRange, Color.green);
            //Debug.Log(rayHits.Length);
            if (rayHits.Length > 0 && !isAttack) //rayHit ������ �����Ͱ� ���� ����(�÷��̾ ���������)���� ���� ���� �ƴ϶�� ���� �ڷ�ƾ ����
            {
                StartCoroutine(Attack());
            }
        }
    }
    
    IEnumerator Attack()
    {
        //�����ϰ� �����Ѵ��� ���� �����ϴ� ����

        isChase = false; //���̻� �Ѿư��� �ʵ���
        isAttack = true; //����
        anim.SetBool("isAttack", true); //���� �ִϸ��̼� (�ִϸ��̼� ��ǿ� ��¦ �����̰� ���� �̸� �����ֱ� ���� yield ���)

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;  //���ݹ��� Ȱ��ȭ

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false; //���ݳ������� ���ݹ��� ��Ȱ��ȭ

                yield return new WaitForSeconds(1f);
                break;

            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse); //����, ���ݼ��� 20
                meleeArea.enabled = true;  //���ݹ��� Ȱ��ȭ

                yield return new WaitForSeconds(0.5f); //���� �� ������ (������ �����ϹǷ� ������ ���� ��������.)
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false; //���ݳ������� ���ݹ��� ��Ȱ��ȭ

                yield return new WaitForSeconds(2f);
                break;

            case Type.C:
                
                //�̻����� ���� ��.
                yield return new WaitForSeconds(0.5f); //��� �ִϸ��̼� �غ�ð�
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                
                //�̻����� ���� ��ġ�� ���� -> ���� �ڱ��ڽŰ� ���� ���� �浹 ->enemy�� enemybullet�� �浹���� �ʵ���.
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
        }
       
        isChase = true; //���ݳ������� �ٽ� �Ѿư����� ����
        isAttack = false;
        anim.SetBool("isAttack", false);

    }

    void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    //�÷��̾ �ֵθ��� ��ġ�� �Ѿ˿� �´� �� ����
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")//��ġ
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 rectVec = transform.position - other.transform.position; // ��ġ�� ���� ��ġ
            //Debug.Log(curHealth);

            StartCoroutine(OnDamage(rectVec, false));
        }
        else if (other.tag == "Bullet")//�Ѿ�
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 rectVec = transform.position - other.transform.position; // �Ѿ˷� ���� ��ġ
            Destroy(other.gameObject); //�Ѿ��� ������ ���������

            StartCoroutine(OnDamage(rectVec, false));
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 rectVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(rectVec, true));
    }

    IEnumerator OnDamage(Vector3 rectVec, bool isGrenade) //�ǰ� ����
    {
        //#1. �ǰ� ���ϸ� Enemy�� ���������� ���ϵ��� ����(���Ͱ� �¾Ҵٴ� �� �÷��̾ ���� �� �ֵ���.)
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;
     
        yield return new WaitForSeconds(0.1f); //0.1�� ������

        //�������� ���� curHealth�� ��ȭ�ϴµ�, �̰� 0���� ū�� ������ �Ǵ��ؾ���.
        if (curHealth > 0) //���� ���� �� ����
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white; //���� Color�� ����

        else  //���Ͱ� ����
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 14; //���� ���� ���·� ���� -> ���ݺҰ��� ���·� ����
            isDead = true;
            isChase = false; //�׾����� ���Ͱ� �÷��̾� ������ ���ߵ���
            
            //agent�� ���������� mesh�� ���� �̵��ϱ� ������ y������ �ö��� ����.
            //����ź �¾��� ���� ���� �� �������� �̵��ϴ� ȿ���� �־���. �̸� �츮�� ���� NavAgent ��Ȱ��ȭ
            nav.enabled = false;

            anim.SetTrigger("doDie"); //���� �ִϸ��̼�
           

            if (isGrenade) // ����ź�� �¾� ������ ����ȸ���ϵ���
            {
                rectVec = rectVec.normalized;
                rectVec += Vector3.up * 3;

                rigid.freezeRotation = false; //ȸ���� �ؾ��ϴµ� enemy�� rigidbody�� x,z���� �����Ǿ��־ �̰� Ǫ�� ����
                rigid.AddForce(rectVec * 5, ForceMode.Impulse);
                rigid.AddTorque(rectVec * 15, ForceMode.Impulse);
            }
            else { // ����ź �̿��� ���ݿ� �¾� ������ �˹�
                rectVec = rectVec.normalized; // ���� 1�� �����ϰ� ���⸸ ����ϰ�
                rectVec += Vector3.up; //��¦ ���� ��������� up ������.
                rigid.AddForce(rectVec * 5, ForceMode.Impulse); //AddForce()�Լ��� �˹�(�˹鷮:5) �����ϱ� 
                // ���⼭ �˹��̶�? ���� ���� �� �ڷ� �ణ �з����� ȿ���� ����.
            }

            if(enemyType != Type.D) //Boss�� ������ Stage�� �����Ƿ� ���� Destory �� �ʿ䰡 ����
                Destroy(gameObject, 4); //ȸ������ ���ϰ� 4�� �� �����
        }
    }
}
