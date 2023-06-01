using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed; //�ν����� â���� ������ �� �ֵ��� public���� ����
    public GameObject[] weapons;
    public bool[] hasWeapons;

    public GameObject[] grenades;
    public int hasGrenades;
    public Camera followCamera;

    //������ ������ ����
    public int ammo;
    public int coin;
    public int health;

    //�÷��̾��� �����ۺ� �ִ� �κ��丮 ��
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    float hAixs;
    float vAixs;
   
    bool wDown; //walk (����Ʈ)
    bool jDown; //jump (�����̽���)
    bool fDown; //fire (����)
    bool rDown; //reload (����)
    bool iDown; //interaction ��ȣ�ۿ� (eŰ)
    bool sDown1; //swap 1�� ���
    bool sDown2; //swap 2�� ���
    bool sDown3; //swap 3�� ���

    bool isJump; //���� ������ �ϰ� �ִ°�?
    bool isDodge; //ȸ��(������)
    bool isSwap; //�������ΰ�?
    bool isReload; //�������ΰ�?
    bool isFireReady = true; //���� ������ ������ �Ǿ��°�?
    //true�� �ʱ�ȭ�ϴ� ����: �̵��ϸ� �ֵθ��� �� ������ �̵��� �� �ֵ��� �������. (�ʱⰪ�� false�̸� ó���� �̵��Ұ�)
    bool isBorder; //�� �浹 �÷��� (��輱�� ��Ҵ��� �Ǵ�)

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject; //�����̿� �ִ� ������ ����
    Weapon equipWeapon; //���� �������� ���� ��ȣ�� ����

    int equipWeaponIndex = -1; //�������� ������ �ε���, �ʱⰪ�� -1�� �ؾ� �ƹ� ���⵵ �������� ���� ���°� ��.
    float fireDelay; //���� ������ -> ������ ���� �� �����غ� �Ǿ��ٴ� ��.

    void Awake() //�ʱ�ȭ
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>(); //�ڽ��̶� �׳� getComponent�� �ƴ� ĥ�己����
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput(); //�Է��� Ű �ҷ�����
        Move(); //������
        Turn(); //ȸ��
        Attack(); //����
        Reload(); //����
        Jump(); //����
        Dodge(); //ȸ��
        Swap(); //���� �ٲٱ�
        Interaction(); //������ �Ա� (��ȣ�ۿ�)
    }

    void GetInput()
    {
        hAixs = Input.GetAxisRaw("Horizontal");
        vAixs = Input.GetAxisRaw("Vertical"); //�Է����� 
        wDown = Input.GetButton("Walk");//��ư�̹Ƿ� Button
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1"); //Fire1�� ���� ���� ���ص� �⺻������ �����Ǿ�����. ���콺 ���� Ŭ��. (Down�� �ƴϱ� ������ �� ������ ������ ��� ������ ������ ��.)
        rDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interaction"); //��ȣ�ۿ�
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        //input Manager���� ������.
    }
    
    void Move()
    {
        moveVec = new Vector3(hAixs, 0, vAixs).normalized;
        /*���ʰ� �������� ���� ������? �밢������ 1�� �̵��� �ع���-> ��Ʈ2��ŭ �̵��ؾ���.
         normalized �̿��� �ذ�*/

        if (isDodge) //ȸ�����̸� moveVec�� ȸ���ϴ� �������� ����
            moveVec = dodgeVec;

        if (isSwap || isReload || !isFireReady) //���⸦ �����ϴ� ���̳� ���⸦ �ֵθ��� ���̰ų� ź�� �����ϴ� �߿��� �̵��Ұ�.
            moveVec = Vector3.zero; //ȸ������ �Ұ���

        if(!isBorder) //���� �浹 �� ȸ���� ���������� �̵��� ���ϵ��� (�浹���� �ʾ��� ���� �̵�)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
            //���� ���� �ӵ��� ���� ���׿����� �̿�. wDown�� T�̸� �ӵ� 0.3��, F�̸� ���� ���(1��)

        anim.SetBool("isRun", moveVec != Vector3.zero); //�޸��Ⱑ �⺻, shift ������ �ȱ�, ���θ� �ƴϸ� ��.
        anim.SetBool("isWalk", wDown);
    }

    void Turn() //�⺻ ȸ�� ����
    {   
        //#1. Ű���忡 ���� ȸ��
        transform.LookAt(transform.position + moveVec); //������ ���͸� ���ؼ� ȸ�������ִ� �Լ�, ���ư��� �������� �ٶ󺸵���.

        //#2. ���콺�� ���� ȸ��
        if (fDown) // ���콺�� �������ʾƵ� �÷��̾ ���콺 Ŀ���� �ٶ󺸰� �Ǵ� ���� �����ϱ� ���ؼ�
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            // ScreenPointToRay(): ��ũ������ �־��� ��ġ���� ����� Ray�� ��� �Լ�
            RaycastHit rayHit; //RayCastHit�� ���콺 Ŭ�� ��ġ�� Ȱ���� ȸ�� ����
            if (Physics.Raycast(ray, out rayHit, 100)) //����° �Ű������� ray�� ����(������ ��� ������ ���� �־����.)
            {
                //out? returnó�� ��ȯ���� �־��� ������ �����ϴ� Ű���� (ray�� ��� ������Ʈ�� ��Ҵ�. ��� ���� rayHit�� ����)
                Vector3 nextVec = rayHit.point - transform.position; //ray�� ��Ҵ� �������� �÷��̾��� ��ġ�� ��.
                nextVec.y = 0; //���ǰ� �ִ� Collider�� hit �ϰ� �Ǹ� �÷��̾�� ���� ȸ���� �ϰ� �Ǵ� ���� ����
                transform.LookAt(transform.position + nextVec); //�÷��̾ �ٶ󺸴� ������ ������ ���� �����ġ�� ����.
            }
        }
    }

    void Jump() //���� ->���ڸ����� �����̽� ������ ����
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap) //moveVec == Vector3.zero �������� �ʰ� ���� ��, !isDodge ȸ������ �ƴ� ���� ��������
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); //�������� �� ���ϱ�. ��������,(���� Ŭ���� ���� ����) Impulse�� �ﰢ���� ���� �ֱ� ����.                                                         
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true; //���������� �������� ��������
        }
    }

    void Attack() //����
    {
        if (equipWeapon == null)
            return; //���⸦ ������� ������ ��

        fireDelay += Time.deltaTime; //�������� �Һ��� �ð� �����ֱ�
        isFireReady = equipWeapon.rate < fireDelay; //���� ����ִ� ������ ���ݼӵ����� fireDelay�� ũ�� ������ �غ� �Ǿ��ٴ� ���̴� True�� ����

        if(fDown && isFireReady && !isDodge & !isSwap) //���ݹ�ư ��������, ������ �غ� �Ǿ���, ȸ�����̰ų� ���⸦ �ٲٴ� ���� �ƴϸ� ���� ����
        {
            equipWeapon.Use(); //�÷��̾�� ������ ������ �����Ǿ������� �Ǵ��ϰ�, ������ ������ ���⿡ �����ϴ� ���.
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot"); //�����ϴ� �ִϸ��̼�
            fireDelay = 0;
        }
    }

    void Reload() //����
    {
        if (equipWeapon == null) //���Ⱑ ������ ���� ����
            return;

        if (equipWeapon.type == Weapon.Type.Melee) //���������
            return;

        if (ammo == 0) //�Ѿ��� ������
            return;

        if (rDown && !isJump && !isDodge && !isSwap && isFireReady) //���� ��� �߿��� ���� �Ұ�
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2f); // �����ӵ� 2�ʷ� ����
        }
    }

    void ReloadOut()
    {
        int emAmmo = equipWeapon.maxAmmo - equipWeapon.curAmmo; //���� �ѿ� ���� �� �ִ� �Ѿ� �� ��� ex.max�� 30�ε� 17�� ��������� 13��.
        int reAmmo = ammo > emAmmo ? emAmmo : ammo;
        // �÷��̾��� źâ�� �ִ�źâ������ ������ �����ϸ鼭 ź�� �� ��������. �ƴϸ� �ִ�źâ����ŭ�� ��������.
        equipWeapon.curAmmo += reAmmo;
        ammo -= reAmmo;
        // ���⿡ ź�� ������ŭ �÷��̾��� �����ϰ� �ִ� ź�� ���� ��.

        isReload = false;
    }

    void Dodge() //ȸ�� -> ����Ű�� ���� ä�� �����̽� ������ ȸ��
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap) //moveVec == Vector3.zero �����̰� ���� ��
        {
            dodgeVec = moveVec;
            speed *= 2; //ȸ���ϴ� �������� �ӵ� 2��� ����
            anim.SetTrigger("doDodge");
            isDodge = true;

            // ȸ�Ǵ� ������ �ٸ��� �����浹�� ���� �������� �� �ƴ� -> �ð����� �ּ� isDodge�� false�� ����� �Լ� �ʿ�
            Invoke("DodgeOut",0.5f); //�ð����Լ�, ����Ƽ���� ������. �Լ��̸��� ���ڿ��� �ְ�, �ð��� �ι�° �Ķ���ͷ� ��.
        }
    }

    void DodgeOut() // ȸ�Ǵ� ������ �ٸ��� �����浹�� ���� �������� �� �ƴ� -> �ð����� �ּ� isDodge�� false�� ����� �Լ� �ʿ�, �ӵ��� �ٽ� ����.
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        //���� ���� �� ���� ��ȭ�� ����, ���� ������ �ʾҴµ� ����ϴ� �� ����
        //->�ڵ忡 ������� �߰�

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
            if (equipWeapon != null) //����� �ƴҰ�쿡�� ����ִ� ���� ����
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true); //���� ��ȣ�� �´� ���� Ȱ��ȭ

            anim.SetTrigger("doSwap");
            //�ִϸ��̼� ���̸�ŭ (�����ϴ� ����) �ƹ��׼ǵ� ������ �ʰ�, ���������� ���ϵ��� ����
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
        if (iDown && nearObject != null & !isJump && !isDodge) //eŰ�� ������, player ��ó�� ��ȣ�ۿ�Ǵ� ��ü�� �ִٸ�
        {
            if(nearObject.tag == "Weapon")
            {
                //���Ⱑ ��ó�� �ִٸ�?
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value; //���⺰�� �ٸ� value �� �������� �����Ǿ�����.(�ظ�: 0, ����: 1, �ӽŰ�: 2)
                hasWeapons[weaponIndex] = true;
               
                Destroy(nearObject); //�Ծ����� ������ �������� ��������� ����

            }
        }
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero; //����ȸ���ӵ��� 0���� ��������. (������ ���� ���� ����)
    }

    void StopToWall()
    {
        //Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        //DrawRay(): Scene ������ Ray�� �����ִ� �Լ�
        //��� ��ġ����, ��� �������� * ray�� ����, ����
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
        //Raycast(): Ray�� ��� ��� ������Ʈ�� �����ϴ� �Լ�
        //��� ��ġ����, ��� ��������, ray�� ����, layermask
        //wall�̶�� layermask�� ���� ��ü�� �浹�ϸ� bool ���� true�� �����.
        //�̰� Move()���� ���ѻ������� �ɾ� �� ���� ����
    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void OnCollisionEnter(Collision collision) //���� ����
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other) //������ �Ա�
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
                    grenades[hasGrenades].SetActive(true); //�������� ����ź�� �÷��̾� �㸮�㿡 ���̵��� ��.
                    hasGrenades += item.value;
                    break;

            }
            Destroy(other.gameObject); //���� ������ ȭ�鿡�� ����
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