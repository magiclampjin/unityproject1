using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed; //인스펙터 창에서 설정할 수 있도록 public으로 설정
    public GameObject[] weapons;
    public bool[] hasWeapons;

    float hAixs;
    float vAixs;
   
    bool wDown; //walk (쉬프트)
    bool jDown; //jump (스페이스바)
    bool iDown; //interaction 상호작용 (e키)
    bool sDown1; //swap 1번 장비
    bool sDown2; //swap 2번 장비
    bool sDown3; //swap 3번 장비

    bool isJump; //지금 점프를 하고 있는가?
    bool isDodge; //회피(구분자)
    bool isSwap; //스왑중인가?

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject; //가까이에 있는 아이템 저장
    GameObject equipWeapon; //현재 장착중인 무기 번호를 저장

    int equipWeaponIndex = -1; //장착중인 무기의 인덱스, 초기값은 -1로 해야 아무 무기도 장착하지 않은 상태가 됨.

    void Awake() //초기화
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>(); //자식이라서 그냥 getComponent가 아닌 칠드런으로
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput(); //입력한 키 불러오기
        Move(); //움직임
        Turn(); //회전
        Jump(); //점프
        Dodge(); //회피
        Swap(); //무기 바꾸기
        Interaction(); //아이템 먹기 (상호작용)
    }

    void GetInput()
    {
        hAixs = Input.GetAxisRaw("Horizontal");
        vAixs = Input.GetAxisRaw("Vertical"); //입력저장 
        wDown = Input.GetButton("Walk");//버튼이므로 Button
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction"); //상호작용
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
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

        if (isSwap)
            moveVec = Vector3.zero;

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
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap) //moveVec == Vector3.zero 움직이지 않고 있을 때, !isDodge 회피중이 아닐 때만 점프가능
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); //물리적인 힘 가하기. 위쪽으로,(숫자 클수록 높이 점프) Impulse는 즉각적인 힘을 주기 가능.                                                         
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true; //무한점프를 막기위한 제약조건
        }
    }
    
    void Dodge() //회피 -> 방향키를 누른 채로 스페이스 누르면 회피
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap) //moveVec == Vector3.zero 움직이고 있을 때
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

    void Swap()
    {
        //같은 무기 들 때는 변화가 없게, 무기 먹지도 않았는데 사용하는 것 방지
        //->코드에 제약사항 추가

        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            if (equipWeapon != null) //빈손이 아닐경우에만 들고있는 무기 삭제
                equipWeapon.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true); //누른 번호에 맞는 무기 활성화

            anim.SetTrigger("doSwap");
            //애니메이션 길이만큼 (스왑하는 동안) 아무액션도 취하지 않고, 움직이지도 못하도록 설정
            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interaction() 
    {
        if (iDown && nearObject != null & !isJump && !isDodge) //e키를 눌렀고, player 근처에 상호작용되는 물체가 있다면
        {
            if(nearObject.tag == "Weapon")
            {
                //무기가 근처에 있다면?
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value; //무기별로 다른 value 값 가지도록 설정되어있음.(해머: 0, 권총: 1, 머신건: 2)
                hasWeapons[weaponIndex] = true;
               
                Destroy(nearObject); //먹었으니 스폰된 아이템은 사라지도록 설정

            }
        }
    }

    void OnCollisionEnter(Collision collision) //착지 구현
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
        //Debug.Log(nearObject.name);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}