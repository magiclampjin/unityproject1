using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed; //�ν����� â���� ������ �� �ֵ��� public���� ����
    public GameObject[] weapons;
    public bool[] hasWeapons;

    float hAixs;
    float vAixs;
   
    bool wDown; //walk (����Ʈ)
    bool jDown; //jump (�����̽���)
    bool iDown; //interaction ��ȣ�ۿ� (eŰ)
    bool sDown1; //swap 1�� ���
    bool sDown2; //swap 2�� ���
    bool sDown3; //swap 3�� ���

    bool isJump; //���� ������ �ϰ� �ִ°�?
    bool isDodge; //ȸ��(������)
    bool isSwap; //�������ΰ�?

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject; //�����̿� �ִ� ������ ����
    GameObject equipWeapon; //���� �������� ���� ��ȣ�� ����

    int equipWeaponIndex = -1; //�������� ������ �ε���, �ʱⰪ�� -1�� �ؾ� �ƹ� ���⵵ �������� ���� ���°� ��.

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
        {
            moveVec = dodgeVec;
        }

        if (isSwap)
            moveVec = Vector3.zero;

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        //���� ���� �ӵ��� ���� ���׿����� �̿�. wDown�� T�̸� �ӵ� 0.3��, F�̸� ���� ���(1��)

        anim.SetBool("isRun", moveVec != Vector3.zero); //�޸��Ⱑ �⺻, shift ������ �ȱ�, ���θ� �ƴϸ� ��.
        anim.SetBool("isWalk", wDown);
    }

    void Turn() //�⺻ ȸ�� ����
    {       
        transform.LookAt(transform.position + moveVec); //������ ���͸� ���ؼ� ȸ�������ִ� �Լ�, ���ư��� �������� �ٶ󺸵���.
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
                equipWeapon.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true); //���� ��ȣ�� �´� ���� Ȱ��ȭ

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

    void OnCollisionEnter(Collision collision) //���� ����
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