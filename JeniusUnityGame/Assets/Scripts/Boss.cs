using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    //Rock�� Enemy�� Bullet �̿�

    Vector3 lookVec; //������ ������ ���� ����. �÷��̾ ���� ������ �̸� �����ϱ� ���� vector
    Vector3 tauntVec; //������� ���ݿ� ���� ���� taunt�ؾ��� ���� ���� vector
    public bool isLook; //jump�� �� ���� �÷��̾ �Ĵٺ��� �ʰ� �� ������ ������ �� �ֵ��� �÷��� ����

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true;
        StartCoroutine(Think());
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            Debug.Log("��������");
            StopAllCoroutines(); //�۵����� ��� �ڷ�ƾ ����
            return; //�Ʒ����� ���̻� ���� ���ϵ��� ����.
        }

        if (isLook) //�÷��̾� �ٶ󺸵���, ���� ���� ����
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
        else
        {
            nav.SetDestination(tauntVec);
        }
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f); //���̵� ���� �� ������ �ð� ����. �ð��� ����� ���̵� ����
        int ranAction = Random.Range(0, 5); // 0,1,2,3,4 �� �������� ����
        switch (ranAction)
        {
            case 0:
            case 1: //�̻��� �߻� ����
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3: //�� �������� ����
                StartCoroutine(RockShot());
                break;
            case 4: //���� ���� ����
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation); 
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();  //�����̻����̹Ƿ� Ÿ���� �ʿ�����. (�̻��� ��ũ��Ʈ���� ������ ��ǥ�� �������ֱ�)
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2f);
        StartCoroutine(Think()); //���� �� �ٽ� ���� (���� ���� ����)
    }
    IEnumerator RockShot()
    {
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);

        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;//�����ϴ� ���߿� �÷��̾ ���� �ʵ��� BoxCollider ��� false
        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true;
        StartCoroutine(Think());
    }
}
