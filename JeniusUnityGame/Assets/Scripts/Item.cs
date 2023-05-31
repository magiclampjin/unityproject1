using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type {Ammo,Coin,Grenade, Heart, Weapon}; //열거형 타입 enum (그냥 int형, float형 같은 타입임)
    public Type type; //아이템 저장 변수
    public int value; //아이템 개수

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 30 * Time.deltaTime); //아이템 제자리 회전(예쁘게)
    }
}
