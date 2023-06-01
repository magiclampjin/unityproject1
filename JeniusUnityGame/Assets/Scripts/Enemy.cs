using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D }; //몬스터 타입
    public Type enemyType;
    public int maxHealth; //최대체력
    public int curHealth; //현재체력
    public Transform target; // 몬스터 AI가 따라다닐 목표물 (플레이어)
    public BoxCollider meleeArea;//몬스터의 공격범위
    public GameObject bullet; //원거리공격 몬스터 C의 총알
    public bool isChase; //몬스터 추격여부
    public bool isAttack; //몬스터 공격여부 - 플레이어와의 거리가 가까워지면 공격 수행 -> 타겟팅 필요
    public bool isDead; //몬스터의 생존여부

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
        /* if (isChase)//목표만 잃고 이동은 계속됨. -> 보정 필요
             nav.SetDestination(target.position); 
        */

        if (nav.enabled && enemyType != Type.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase; //완벽하게 멈춤.
        }
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero; //물리회전속도를 0으로 만들어버림. (스스로 도는 현상 제거)
        }
    }

    void Targeting()
    {
        if (enemyType != Type.D && !isDead)
        {
            //ray를 사용하되 ray는 공격횟수가 많이 작을 것. -> 굵게 줄 것.
            //구체모양의 캐스팅 - Physics.SphereCast
            float targetRadius = 0; //두께(정확도)
            float targetRange = 0; //길이(범위)

            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;

                case Type.B:
                    targetRadius = 1f; //돌격이 A보다 정확하게
                    targetRange = 12f; //공격범위 A의 4배
                    break;

                case Type.C:
                    targetRadius = 0.5f; //돌격 가장 정확하게
                    targetRange = 25f; //공격범위 가장 길게
                    break;
            }

            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
                                                        targetRadius,
                                                        Vector3.forward,
                                                        targetRange,
                                                        LayerMask.GetMask("Player"));

            //Debug.DrawRay(transform.position, transform.forward * targetRange, Color.green);
            //Debug.Log(rayHits.Length);
            if (rayHits.Length > 0 && !isAttack) //rayHit 변수에 데이터가 들어온 상태(플레이어가 가까워지면)에서 공격 중이 아니라면 공격 코루틴 실행
            {
                StartCoroutine(Attack());
            }
        }
    }
    
    IEnumerator Attack()
    {
        //정지하고 공격한다음 추적 개시하는 로직

        isChase = false; //더이상 쫓아가지 않도록
        isAttack = true; //공격
        anim.SetBool("isAttack", true); //공격 애니메이션 (애니메이션 모션에 살짝 딜레이가 있음 이를 맞춰주기 위해 yield 사용)

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;  //공격범위 활성화

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false; //공격끝났으니 공격범위 비활성화

                yield return new WaitForSeconds(1f);
                break;

            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse); //돌격, 공격세기 20
                meleeArea.enabled = true;  //공격범위 활성화

                yield return new WaitForSeconds(0.5f); //돌격 후 멈춰짐 (빠르게 돌격하므로 빠르게 멈춰 세워야함.)
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false; //공격끝났으니 공격범위 비활성화

                yield return new WaitForSeconds(2f);
                break;

            case Type.C:
                
                //미사일을 만들어서 쏨.
                yield return new WaitForSeconds(0.5f); //쏘는 애니메이션 준비시간
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                
                //미사일을 몬스터 위치에 만듦 -> 몬스터 자기자신과 가장 먼저 충돌 ->enemy와 enemybullet은 충돌하지 않도록.
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
        }
       
        isChase = true; //공격끝났으니 다시 쫓아가도록 성정
        isAttack = false;
        anim.SetBool("isAttack", false);

    }

    void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    //플레이어가 휘두르는 망치나 총알에 맞는 것 구현
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")//망치
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 rectVec = transform.position - other.transform.position; // 망치로 때린 위치
            //Debug.Log(curHealth);

            StartCoroutine(OnDamage(rectVec, false));
        }
        else if (other.tag == "Bullet")//총알
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 rectVec = transform.position - other.transform.position; // 총알로 때린 위치
            Destroy(other.gameObject); //총알이 맞으면 사라지도록

            StartCoroutine(OnDamage(rectVec, false));
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 rectVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(rectVec, true));
    }

    IEnumerator OnDamage(Vector3 rectVec, bool isGrenade) //피격 로직
    {
        //#1. 피격 당하면 Enemy가 빨간색으로 변하도록 설정(몬스터가 맞았다는 걸 플레이어가 느낄 수 있도록.)
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;
     
        yield return new WaitForSeconds(0.1f); //0.1초 딜레이

        //데미지에 따라 curHealth가 변화하는데, 이게 0보다 큰지 작은지 판단해야함.
        if (curHealth > 0) //몬스터 아직 안 죽음
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white; //원래 Color로 변경

        else  //몬스터가 죽음
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 14; //죽은 몬스터 상태로 변경 -> 공격불가능 상태로 변경
            isDead = true;
            isChase = false; //죽었으니 몬스터가 플레이어 추적을 멈추도록
            
            //agent가 켜져있으면 mesh를 따라서 이동하기 때문에 y축으로 올라가지 않음.
            //수류탄 맞았을 때나 죽을 때 위쪽으로 이동하는 효과를 주었음. 이를 살리기 위해 NavAgent 비활성화
            nav.enabled = false;

            anim.SetTrigger("doDie"); //죽은 애니메이션
           

            if (isGrenade) // 수류탄에 맞아 죽으면 공중회전하도록
            {
                rectVec = rectVec.normalized;
                rectVec += Vector3.up * 3;

                rigid.freezeRotation = false; //회전을 해야하는데 enemy의 rigidbody는 x,z축이 고정되어있어서 이걸 푸는 역할
                rigid.AddForce(rectVec * 5, ForceMode.Impulse);
                rigid.AddTorque(rectVec * 15, ForceMode.Impulse);
            }
            else { // 수류탄 이외의 공격에 맞아 죽으면 넉백
                rectVec = rectVec.normalized; // 값은 1로 통일하고 방향만 기억하게
                rectVec += Vector3.up; //살짝 위에 띄워지도록 up 더해줌.
                rigid.AddForce(rectVec * 5, ForceMode.Impulse); //AddForce()함수로 넉백(넉백량:5) 구현하기 
                // 여기서 넉백이란? 몬스터 공격 시 뒤로 약간 밀려나는 효과를 말함.
            }

            if(enemyType != Type.D) //Boss는 죽으면 Stage가 끝나므로 굳이 Destory 할 필요가 없음
                Destroy(gameObject, 4); //회색으로 변하고 4초 뒤 사라짐
        }
    }
}
