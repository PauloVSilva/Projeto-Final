using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemID
{
    public ItemScriptableObject itemSO;
    public string itemIdentifier;
}

public class ItemsDatabank : Singleton<ItemsDatabank>
{
    public List<ItemID> dataBank = new List<ItemID>();

    public ItemScriptableObject GetItem(string _itemIdentifier)
    {
        for (int i = 0; i < dataBank.Count; i++)
        {
            if(dataBank[i].itemIdentifier == _itemIdentifier)
            {
                return dataBank[i].itemSO;
            }
        }
        return null;
    }

}
