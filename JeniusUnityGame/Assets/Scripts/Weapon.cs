using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //�������� - ��ġ
    //���Ÿ����� - ��
    public enum Type { Melee, Range}; //�������� type
    public Type type; // ���� ����ִ� ������ Ÿ��
    public int damage; //������ ���ݷ�
    public float rate; //������ ���ݼӵ�
    public BoxCollider meleeArea; //������ ���ݹ���
    public TrailRenderer trailEffect; //���� �ֵθ��� ȿ��

    public void Use() //������
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing"); //���� �ڷ�ƾ�� ���� �����ϱ� ���ؼ� ���� �������� �ڷ�ƾ�� ������ ������ ������ �ʵ��� �ϴ� ����
            StartCoroutine("Swing"); //��������� �ֵθ���.
        }
    } 

    IEnumerator Swing() //box collider�� trail renderer�� �Ѱ�, �����ð��� ������ �ٽ� ����.
    {
        //yield //����� �����ϴ� Ű����

        //1������
        //yield return null; //1������ ���
        //2������
        //yield return null; //yield Ű���� ���� �� ����� �ð��� ���� �ۼ� ���� 
        //3������

        //�̷� ������ ������ ���̿� ��⸦ ��.
        //�����Ӹ��� ��밡��! 0.1�� ������?
        //yield return new WaitForSeconds(0.1f); //�־��� ��ġ(�ð�)��ŭ ��ٸ��� �Լ�
        //�߰��� �׸��ΰ� ������ yield break; ��� (�ڷ�ƾ Ż��) -> �Ʒ� ������ �� ������ �Ʒ� �������� ��Ȱ��ȭ��. ������ ����� ��.

        yield return new WaitForSeconds(0.1f); //0.1�� ���
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;

    }

    //box collider�� trail renderer�� �Ѱ�, �����ð��� ������ �ٽ� ����.
    //invoke ������ ��� �ڷ�ƾ(Co-Routine) ���.
    // Use()�Լ��� ���η�ƾ�̶�� ��.
    // ���η�ƾ���� Swing()�Լ� ȣ��
    // Swing()�Լ��� �����ƾ�̶�� ��.
    // �����ƾ�� ������ �ٽ� ���η�ƾ�� Use()�� ���ư� �Ʒ��ִ� ������ ���������� ������.

    // �׷���, Swing()�Լ��� �ڸ�ƾ�̶�� ���� �ٸ���.
    // �ڷ�ƾ �Լ�: ���η�ƾ + �ڷ�ƾ (Swing()�� ȣ��Ǵ� �������� ���� ����)
    // Use() ���η�ƾ + Swing() �ڷ�ƾ - �ڴ� ������ �� �����ϴ� ���ӿ��� (Co-op)�̶�� �θ�(�Բ���� ��)
    // �������? void ����� IEnumerator��� ������ �Լ� Ŭ���� ���
    // �ڷ�ƾ�� yield�� �� �ϳ� �̻� �ʿ�.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
