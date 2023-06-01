using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    //Rock은 Enemy의 Bullet 이용

    Vector3 lookVec; //보스의 지능이 가장 높음. 플레이어가 가는 방향을 미리 예측하기 위한 vector
    Vector3 tauntVec; //내려찍는 공격에 대해 어디로 taunt해야할 지에 대한 vector
    public bool isLook; //jump를 할 때는 플레이어를 쳐다보지 않고 그 방향을 유지할 수 있도록 플래그 변수

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
            Debug.Log("보스죽음");
            StopAllCoroutines(); //작동중인 모든 코루틴 정지
            return; //아래로직 더이상 실행 못하도록 막음.
        }

        if (isLook) //플레이어 바라보도록, 가는 방향 예측
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
        yield return new WaitForSeconds(0.1f); //난이도 조절 시 딜레이 시간 변경. 시간이 길수록 난이도 하향
        int ranAction = Random.Range(0, 5); // 0,1,2,3,4 중 랜덤으로 저장
        switch (ranAction)
        {
            case 0:
            case 1: //미사일 발사 패턴
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3: //돌 굴러가는 패턴
                StartCoroutine(RockShot());
                break;
            case 4: //점프 공격 패턴
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation); 
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();  //유도미사일이므로 타겟이 필요해짐. (미사일 스크립트까지 접근해 목표물 설정해주기)
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2f);
        StartCoroutine(Think()); //공격 후 다시 생각 (다음 공격 구상)
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
        boxCollider.enabled = false;//점프하는 도중에 플레이어를 밀지 않도록 BoxCollider 잠시 false
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
