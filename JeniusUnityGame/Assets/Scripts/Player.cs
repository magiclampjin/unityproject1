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
    public GameObject grenadeObj;
    public Camera followCamera;
    public GameManager manager;

    public AudioSource jumpSound;
    public AudioSource attackSound;
    public AudioSource dodgeSound;
    public AudioSource deadSound;
    public AudioSource healSound;
    public AudioSource coinSound;
    public AudioSource swapSound;

    //������ ������ ����
    public int ammo;
    public int coin;
    public int health;

    //�÷��̾��� �����ۺ� �ִ� �κ��丮 ��
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    //�÷��̾��� ����
    public int score;

    float hAixs;
    float vAixs;
   
    bool wDown; //walk (����Ʈ)
    bool jDown; //jump (�����̽���)
    bool fDown; //fire (����)
    bool gDown; //grenade (����ź ������) ���콺 ������
    bool rDown; //reload (����)
    bool iDown; //interaction ��ȣ�ۿ� (eŰ)
    bool sDown1; //swap 1�� ��� -����1
    bool sDown2; //swap 2�� ��� -����2
    bool sDown3; //swap 3�� ��� -����3

    bool isJump; //���� ������ �ϰ� �ִ°�?
    bool isDodge; //ȸ��(������)
    bool isSwap; //�������ΰ�?
    bool isReload; //�������ΰ�?
    bool isFireReady = true; //���� ������ ������ �Ǿ��°�?
    //true�� �ʱ�ȭ�ϴ� ����: �̵��ϸ� �ֵθ��� �� ������ �̵��� �� �ֵ��� �������. (�ʱⰪ�� false�̸� ó���� �̵��Ұ�)
    bool isBorder; //�� �浹 �÷��� (��輱�� ��Ҵ��� �Ǵ�)
    bool isDamage; //�°� �ִ�? (���޾� ���� ���Ϳ��� ���ݹ޴� �� �����ϱ� ���� ���� �� ��� �����ð��� �����ϱ� ���� bool ����)
    bool isShop; //�������̴�. (���� ���� �ÿ� ���콺 Ŭ������ ���ؼ� ���� ������ ���� ����)
    bool isDead; //�÷��̾��� ��������

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] meshs; //�Ӹ��� �ְ� �ȵ� �ְ� �ϹǷ� �迭�� ������ (���Ϳ��� ������ �� ���� ���ϰ�)

    GameObject nearObject; //�����̿� �ִ� ������ ����
    public Weapon equipWeapon; //���� �������� ���� ��ȣ�� ����

    int equipWeaponIndex = -1; //�������� ������ �ε���, �ʱⰪ�� -1�� �ؾ� �ƹ� ���⵵ �������� ���� ���°� ��.
    float fireDelay; //���� ������ -> ������ ���� �� �����غ� �Ǿ��ٴ� ��.

    void Awake() //�ʱ�ȭ
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>(); //�ڽ��̶� �׳� getComponent�� �ƴ� InChildren����
        meshs = GetComponentsInChildren<MeshRenderer>(); //GetComponents �ϸ� ��� �ڽ�������Ʈ�� �� ������.
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
        Jump(); //����
        Grenade(); //����ź ������
        Attack(); //����
        Reload(); //����
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
        gDown = Input.GetButtonDown("Fire2"); //���콺 ������ Ŭ��
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

        if (isSwap || isReload || !isFireReady && !isDead) //���⸦ �����ϴ� ���̳� ���⸦ �ֵθ��� ���̰ų� ź�� �����ϴ� �߿��� �̵��Ұ�.
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
        if (fDown && !isDead) // ���콺�� �������ʾƵ� �÷��̾ ���콺 Ŀ���� �ٶ󺸰� �Ǵ� ���� �����ϱ� ���ؼ�
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
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap && !isDead) //moveVec == Vector3.zero �������� �ʰ� ���� ��, !isDodge ȸ������ �ƴ� ���� ��������
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); //�������� �� ���ϱ�. ��������,(���� Ŭ���� ���� ����) Impulse�� �ﰢ���� ���� �ֱ� ����.                                                         
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true; //���������� �������� ��������

            jumpSound.Play();
        }
    }

    void Grenade() //����ź ������
    {
        if (hasGrenades == 0)
            return; //����ź ������ �� ����

        if (gDown && !isReload && !isSwap && !isShop && !isDead) //�������̳� ���⺯���߿��� ����ź�� ���� �� ����.
        {
            // ����ź�� ���콺�� ������ �� ȸ���� �������� �׳� ���콺 Ŭ���� �� �ڸ��� ������
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit; 
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position; 
                nextVec.y = 15; //����ź�̴� �ణ ���� ���������� y�� �ٲ���. 

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                //Instantiate()�Լ��� ����ź ���� - �������� �ν��Ͻ�ȭ�ؼ� �ϳ� �ø���
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>(); //������ ���� �����ϱ� ���� Rigidbody �̿� - ���� ��
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse); //���־� ���̰� ȸ�� ���� �ֱ�

                //����ź �������� ����ź ���� �ϳ� ����.
                hasGrenades--;
                grenades[hasGrenades].SetActive(false); //�÷��̾� ��ó�� �����ϴ� ����ź�� �ϳ� ����
                //�����ϴ� ����Ʈ�� ��Ȱ��ȭ�صΰ� ���� ���� Ȱ��ȭ�ǵ�����. (������ ����ź �����鿡��)
                //�ٸ� ��ũ��Ʈ���� ���� -> Grenade.cs
            }
            
        }
    }

    void Attack() //����
    {
        if (equipWeapon == null)
            return; //���⸦ ������� ������ ��

        fireDelay += Time.deltaTime; //�������� �Һ��� �ð� �����ֱ�
        isFireReady = equipWeapon.rate < fireDelay; //���� ����ִ� ������ ���ݼӵ����� fireDelay�� ũ�� ������ �غ� �Ǿ��ٴ� ���̴� True�� ����

        if(fDown && isFireReady && !isDodge && !isSwap && !isShop && !isDead) //���ݹ�ư ��������, ������ �غ� �Ǿ���, ȸ�����̰ų� ���⸦ �ٲٴ� ���� �ƴϸ�, ���� �̿����� �ƴ� �� ���� ����
        {
            equipWeapon.Use(); //�÷��̾�� ������ ������ �����Ǿ������� �Ǵ��ϰ�, ������ ������ ���⿡ �����ϴ� ���.
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot"); //�����ϴ� �ִϸ��̼�
            fireDelay = 0;

            attackSound.Play();
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

        if (rDown && !isJump && !isDodge && !isSwap && isFireReady && !isShop && !isDead) //���� ��� �߿��� ���� �Ұ�
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
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap && !isShop && !isDead) //moveVec == Vector3.zero �����̰� ���� ��
        {
            dodgeVec = moveVec;
            speed *= 2; //ȸ���ϴ� �������� �ӵ� 2��� ����
            dodgeSound.Play();
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

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isShop && !isDead)
        {
            if (equipWeapon != null) //����� �ƴҰ�쿡�� ����ִ� ���� ����
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true); //���� ��ȣ�� �´� ���� Ȱ��ȭ

            swapSound.Play();
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
        if (iDown && nearObject != null & !isJump && !isDodge && !isShop && !isDead) //eŰ�� ������, player ��ó�� ��ȣ�ۿ�Ǵ� ��ü�� �ִٸ�
        {
            if(nearObject.tag == "Weapon")
            {
                //���Ⱑ ��ó�� �ִٸ�?
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value; //���⺰�� �ٸ� value �� �������� �����Ǿ�����.(�ظ�: 0, ����: 1, �ӽŰ�: 2)
                hasWeapons[weaponIndex] = true;
               
                Destroy(nearObject); //�Ծ����� ������ �������� ��������� ����
            }

            else if (nearObject.tag == "Shop")
            {
                //������ ��ó�� �ִٸ�?
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this); //this: Player ��ũ��Ʈ���̹Ƿ� �ڱ��ڽ�.
                isShop = true;
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

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")  //������ �Ա�
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
                    coinSound.Play();
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    healSound.Play();
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

        else if (other.tag == "EnemyBullet")
        {
            if (!isDamage)//�°� ���� ���� ���¿����� ���ݹ���.(���� ���Ϳ��� ���ÿ� ���ݹ��� �ʵ���)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(OnDamage(isBossAtk));
            }

            if (other.GetComponent<Rigidbody>() != null) //���������� rigidbody�� ������� �����Ƿ� �̻��ϰ���(���Ÿ�����)�� �ش�.
            {
                Destroy(other.gameObject);
            }
        }
    }

    IEnumerator OnDamage(bool isBossAtk)
    {
        isDamage = true;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.magenta;
        }

        if (isBossAtk)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);

        if (health <= 0 && !isDead)//�÷��̾ ������
            OnDie();

        yield return new WaitForSeconds(1f); //1�ʵ��� �������� -> �÷��̾� ����Ÿ�� ����

        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

        if (isBossAtk)
            rigid.velocity = Vector3.zero;
    }

    void OnDie()
    {
        deadSound.Play();
        anim.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Shop") 
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;

        else if(other.tag == "Shop")
        {
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }
}