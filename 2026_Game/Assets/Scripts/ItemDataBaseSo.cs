using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDataBaseSo", menuName = "Invenory/DataBase")]
public class ItemDataBaseSo : ScriptableObject
{
    public List<ItemSo> items = new List<ItemSo>();                         //itemso를 리스트로 관리

    //캐싱을 위하ㄴ Dictrionary
    private Dictionary<int, ItemSo> itemsByld;                  //id로 아이템 찾기 위한 캐싱
    Dictionary<string, ItemSo> itemByName;                      //이름으로 아이템 찾기
    
    public void Initialze()
    {
        itemsByld = new Dictionary<int, ItemSo>();      //위에 선언만 했기 때문에 Dictionary 할당 
        itemByName = new Dictionary<string, ItemSo>();

        foreach(var item in items)
        {
            itemsByld[item.id] = item;
            itemByName[item.Itemname] = item;
        }


    }

    public ItemSo GetItemById(int id)
    {
        if((itemsByld == null)){                     //캐싱이 되어있는지 확인 하고 아니면 초기화 한
            Initialze();
        }

        if(itemsByld.TryGetValue(id, out ItemSo item))      //id값을 찾아서 ItemSo를 리턴한다.
            return item;

        return null;                                    //없을 경우 NULL
    }

    public ItemSo GetItemByName(string name)
    {
        if (itemByName == null)
        {
            Initialze();                            //캐싱이 되어있는지 확인하고 아니면 초기화 한다.
        }

        if(itemByName.TryGetValue(name, out ItemSo item))       //NULL값을 찾아서 ItemSo를 리턴한다.
            return item;

        return null;
    }


    //타입으로 아이템 필터링
    public List<ItemSo> GetItemByType(ItemType type)
    {
        return items.FindAll(item => item.itemType == type);
    }

}
