using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootHolder : MonoBehaviour
{
    private List<ItemInLoot> LootItems = new List<ItemInLoot>();
    [SerializeField] private int HowManyMaxItem;
    [SerializeField] private int MaxItemInStack;

    private int ItemInside;



    private void OnEnable()
    {
        LootItems.Clear();

        ItemInside = Random.Range(0, HowManyMaxItem);

        for (int i = 0; i < ItemInside; i++)
        {
            ItemInLoot newItem = new ItemInLoot(); // Tworzenie nowej instancji obiektu ItemInLoot
            newItem.Item = ObjectPooling.Instance.ReturnItemFromDatabase();
            newItem.Amount = Random.Range(0, MaxItemInStack);
            LootItems.Add(newItem);
        }
    }


    public List<ItemInLoot> AddLootToInventory()
    {
        return LootItems;
    }

}


[Serializable]
public class ItemInLoot
{
    public ItemInfo Item;
    public int Amount;
}