using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSo", menuName = "Inventory/ItemSo")]
public class ItemSo : ScriptableObject
{
    public int id;
    public string Itemname;
    public string description;
    public string nameEng;

    public ItemType itemType;
    public int price;
    public int power;
    public int level;
    public bool isStackable;
    public Sprite icon;                     //실제 사용할 스프라이트 선언


    public override string ToString()
    {
        return $"[{id}] {Itemname} ({itemType}) - 가격 : {price} 골드, 속성 : {power} ";
    }

    public string DisplayNmae
    {
        get {return string.IsNullOrEmpty(nameEng) ? Itemname : nameEng; }
    }

}
