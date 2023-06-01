using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed; //인스펙터 창에서 설정할 수 있도록 public으로 설정
    public GameObject[] weapons;
    public bool[] hasWeapons;

    public GameObject[] grenades;
    public int hasGrenades;
    public Camera followCamera;

    //소지한 아이템 개수
    public int ammo;
    public int coin;
    public int health;

    //플레이어의 아이템별 최대 인벤토리 수
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    float hAixs;
    float vAixs;
   
    bool wDown; //walk (쉬프트)
    bool jDown; //jump (스페이스바)
    bool fDown; //fire (공격)
    bool rDown; //reload (장전)
    bool iDown; //interaction 상호작용 (e키)
    bool sDown1; //swap 1번 장비
    bool sDown2; //swap 2번 장비
    bool sDown3; //swap 3번 장비

    bool isJump; //지금 점프를 하고 있는가?
    bool isDodge; //회피(구분자)
    bool isSwap; //스왑중인가?
    bool isReload; //장전중인가?
    bool isFireReady = true; //공격 딜레이 만족이 되었는가?
    //true로 초기화하는 이유: 이동하며 휘두르는 게 금지라서 이동할 수 있도록 만드려고. (초기값이 false이면 처음에 이동불가)
    bool isBorder; //벽 충돌 플래그 (경계선에 닿았는지 판단)

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject; //가까이에 있는 아이템 저장
    Weapon equipWeapon; //현재 장착중인 무기 번호를 저장

    int equipWeaponIndex = -1; //장착중인 무기의 인덱스, 초기값은 -1로 해야 아무 무기도 장착하지 않은 상태가 됨.
    float fireDelay; //공격 딜레이 -> 딜레이 만족 시 공격준비가 되었다는 뜻.

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
        Attack(); //공격
        Reload(); //장전
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
        fDown = Input.GetButton("Fire1"); //Fire1은 따로 설정 안해도 기본적으로 설정되어있음. 마우스 왼쪽 클릭. (Down이 아니기 때문에 꾹 누르고 있으면 계속 공격이 나가게 됨.)
        rDown = Input.GetButtonDown("Reload");
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
            moveVec = dodgeVec;

        if (isSwap || isReload || !isFireReady) //무기를 변경하는 중이나 무기를 휘두르는 중이거나 탄을 장전하는 중에는 이동불가.
            moveVec = Vector3.zero; //회전까지 불가능

        if(!isBorder) //벽에 충돌 시 회전은 가능하지만 이동은 못하도록 (충돌하지 않았을 때만 이동)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
            //걸을 때는 속도를 낮춤 삼항연산자 이용. wDown이 T이면 속도 0.3배, F이면 원래 배속(1배)

        anim.SetBool("isRun", moveVec != Vector3.zero); //달리기가 기본, shift 누르면 걷기, 제로만 아니면 됨.
        anim.SetBool("isWalk", wDown);
    }

    void Turn() //기본 회전 구현
    {   
        //#1. 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec); //지정된 벡터를 향해서 회전시켜주는 함수, 나아가는 방향으로 바라보도록.

        //#2. 마우스에 의한 회전
        if (fDown) // 마우스를 누르지않아도 플레이어가 마우스 커서를 바라보게 되는 것을 방지하기 위해서
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            // ScreenPointToRay(): 스크린에서 주어진 위치에서 월드로 Ray를 쏘는 함수
            RaycastHit rayHit; //RayCastHit의 마우스 클릭 위치를 활용해 회전 구현
            if (Physics.Raycast(ray, out rayHit, 100)) //세번째 매개변수는 ray의 길이(무조건 닿기 때문에 세게 주어야함.)
            {
                //out? return처럼 반환값을 주어진 변수에 저장하는 키워드 (ray가 어느 오브젝트에 닿았다. 라는 것을 rayHit에 저장)
                Vector3 nextVec = rayHit.point - transform.position; //ray가 닿았던 지점에서 플레이어의 위치를 뺌.
                nextVec.y = 0; //부피가 있는 Collider를 hit 하게 되면 플레이어는 축이 회전을 하게 되는 것을 방지
                transform.LookAt(transform.position + nextVec); //플레이어가 바라보는 방향을 위에서 구한 상대위치로 변경.
            }
        }
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

    void Attack() //공격
    {
        if (equipWeapon == null)
            return; //무기를 들고있지 않으면 땡

        fireDelay += Time.deltaTime; //매프레임 소비한 시간 더해주기
        isFireReady = equipWeapon.rate < fireDelay; //현재 들고있는 무기의 공격속도보다 fireDelay가 크면 공격할 준비가 되었다는 뜻이니 True로 변경

        if(fDown && isFireReady && !isDodge & !isSwap) //공격버튼 눌렀으며, 공격할 준비가 되었고, 회피중이거나 무기를 바꾸는 중이 아니면 공격 가능
        {
            equipWeapon.Use(); //플레이어는 공격할 조건이 만족되었는지만 판단하고, 공격할 로직은 무기에 위임하는 방식.
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot"); //공격하는 애니메이션
            fireDelay = 0;
        }
    }

    void Reload() //장전
    {
        if (equipWeapon == null) //무기가 없으면 장전 못함
            return;

        if (equipWeapon.type == Weapon.Type.Melee) //근접무기면
            return;

        if (ammo == 0) //총알이 없으면
            return;

        if (rDown && !isJump && !isDodge && !isSwap && isFireReady) //총을 쏘는 중에는 장전 불가
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2f); // 장전속도 2초로 지정
        }
    }

    void ReloadOut()
    {
        int emAmmo = equipWeapon.maxAmmo - equipWeapon.curAmmo; //현재 총에 넣을 수 있는 총알 수 계산 ex.max가 30인데 17개 들어있으면 13개.
        int reAmmo = ammo > emAmmo ? emAmmo : ammo;
        // 플레이어의 탄창이 최대탄창수보다 적으면 장전하면서 탄을 다 넣으세요. 아니면 최대탄창수만큼만 넣으세요.
        equipWeapon.curAmmo += reAmmo;
        ammo -= reAmmo;
        // 무기에 탄을 넣은만큼 플레이어의 소지하고 있는 탄의 수를 뺌.

        isReload = false;
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
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true); //누른 번호에 맞는 무기 활성화

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

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero; //물리회전속도를 0으로 만들어버림. (스스로 도는 현상 제거)
    }

    void StopToWall()
    {
        //Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        //DrawRay(): Scene 내에서 Ray를 보여주는 함수
        //어느 위치에서, 어느 방향으로 * ray의 길이, 색상
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
        //Raycast(): Ray를 쏘아 닿는 오브젝트를 감지하는 함수
        //어느 위치에서, 어느 방향으로, ray의 길이, layermask
        //wall이라는 layermask를 가진 물체랑 충돌하면 bool 값이 true로 변경됨.
        //이걸 Move()에서 제한사항으로 걸어 벽 관통 방지
    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void OnCollisionEnter(Collision collision) //착지 구현
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other) //아이템 먹기
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    if (hasGrenades == maxHasGrenades)
                        return;
                    grenades[hasGrenades].SetActive(true); //소지중인 수류탄은 플레이어 허리춤에 보이도록 함.
                    hasGrenades += item.value;
                    break;

            }
            Destroy(other.gameObject); //먹은 아이템 화면에서 삭제
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}