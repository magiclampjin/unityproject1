using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target; //����ź�� ������ �߽�
    public float orbitSpeed; //������ �ӵ�
    Vector3 offset; //�÷��̾�� ����ź ���� �Ÿ�(������)

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.position;
        //offset�� ���� ����ź ��ġ���� Ÿ�� ��ġ�� �� ��.
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset; //target.position�� ��� ��ȭ��. RotateAround�� ��ǥ�� �����̸� �ϱ׷����� ������ ����.  -> �̸� ����
        //���� update�� ����.

        //����� ������ ȸ���ϴ� �Լ��� RotateAround
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime); // �ι�° �Ű������� ȸ����, ����° �Ű������� ȸ���ϴ� ��ġ
        offset = transform.position - target.position; //�� �̵��� �����״� offset �ٽ� ������Ʈ
    }
}
