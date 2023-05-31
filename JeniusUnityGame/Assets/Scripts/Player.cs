using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float hAixs;
    float vAixs;
    public float speed; //인스펙터 창에서 설정할 수 있도록 public으로 설정
    bool wDown; //걷기 쉬프트
    bool jDown; //점프 스페이스바

    bool isJump; //지금 점프를 하고 있는가?
    bool isDodge; //회피(구분자)

    Vector3 moveVec;
    Vector3 dodgeVec;
    Animator anim;
    Rigidbody rigid;

    void Awake() //초기화
    {
        anim = GetComponentInChildren<Animator>(); //자식이라서 그냥 getComponent가 아닌 칠드런으로
        rigid = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
    }

    void GetInput()
    {
        hAixs = Input.GetAxisRaw("Horizontal");
        vAixs = Input.GetAxisRaw("Vertical"); //입력저장 
        wDown = Input.GetButton("Walk");//버튼이므로 Button
        jDown = Input.GetButtonDown("Jump");
        //input Manager에서 관리됨.
    }
    
    void Move()
    {
        moveVec = new Vector3(hAixs, 0, vAixs).normalized;
        /*위쪽과 오른쪽을 같이 누르면? 대각선으로 1만 이동을 해버림-> 루트2만큼 이동해야함.
         normalized 이용해 해결*/

        if (isDodge) //회피중이면 moveVec는 회피하는 방향으로 변경
        {
            moveVec = dodgeVec;
        }

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        //걸을 때는 속도를 낮춤 삼항연산자 이용. wDown이 T이면 속도 0.3배, F이면 원래 배속(1배)

        anim.SetBool("isRun", moveVec != Vector3.zero); //달리기가 기본, shift 누르면 걷기, 제로만 아니면 됨.
        anim.SetBool("isWalk", wDown);
    }

    void Turn() //기본 회전 구현
    {       
        transform.LookAt(transform.position + moveVec); //지정된 벡터를 향해서 회전시켜주는 함수, 나아가는 방향으로 바라보도록.
    }

    void Jump() //점프 ->제자리에서 스페이스 누르면 점프
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge) //moveVec == Vector3.zero 움직이지 않고 있을 때, !isDodge 회피중이 아닐 때만 점프가능
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); //물리적인 힘 가하기. 위쪽으로,(숫자 클수록 높이 점프) Impulse는 즉각적인 힘을 주기 가능.                                                         
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true; //무한점프를 막기위한 제약조건
        }
    }
    
    void Dodge() //회피 -> 방향키를 누른 채로 스페이스 누르면 회피
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge) //moveVec == Vector3.zero 움직이고 있을 때
        {
            dodgeVec = moveVec;
            speed *= 2; //회피하는 순간에만 속도 2배로 변경
            anim.SetTrigger("doDodge");
            isDodge = true;

            // 회피는 점프와 다르게 물리충돌에 의해 내려가는 게 아님 -> 시간차를 둬서 isDodge를 false로 만드는 함수 필요
            Invoke("DodgeOut",0.5f); //시간차함수, 유니티에서 지원함. 함수이름을 문자열로 넣고, 시간을 두번째 파라미터로 둠.
        }
    }

    void DodgeOut() // 회피는 점프와 다르게 물리충돌에 의해 내려가는 게 아님 -> 시간차를 둬서 isDodge를 false로 만드는 함수 필요, 속도도 다시 낮춤.
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void OnCollisionEnter(Collision collision) //착지 구현
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }
}
