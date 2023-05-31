using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float hAixs;
    float vAixs;
    public float speed; //�ν����� â���� ������ �� �ֵ��� public���� ����
    bool wDown; //�ȱ� ����Ʈ
    bool jDown; //���� �����̽���

    bool isJump; //���� ������ �ϰ� �ִ°�?
    bool isDodge; //ȸ��(������)

    Vector3 moveVec;
    Vector3 dodgeVec;
    Animator anim;
    Rigidbody rigid;

    void Awake() //�ʱ�ȭ
    {
        anim = GetComponentInChildren<Animator>(); //�ڽ��̶� �׳� getComponent�� �ƴ� ĥ�己����
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
        vAixs = Input.GetAxisRaw("Vertical"); //�Է����� 
        wDown = Input.GetButton("Walk");//��ư�̹Ƿ� Button
        jDown = Input.GetButtonDown("Jump");
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
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge) //moveVec == Vector3.zero �������� �ʰ� ���� ��, !isDodge ȸ������ �ƴ� ���� ��������
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); //�������� �� ���ϱ�. ��������,(���� Ŭ���� ���� ����) Impulse�� �ﰢ���� ���� �ֱ� ����.                                                         
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true; //���������� �������� ��������
        }
    }
    
    void Dodge() //ȸ�� -> ����Ű�� ���� ä�� �����̽� ������ ȸ��
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge) //moveVec == Vector3.zero �����̰� ���� ��
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

    void OnCollisionEnter(Collision collision) //���� ����
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }
}
