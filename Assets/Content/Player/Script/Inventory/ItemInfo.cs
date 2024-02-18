using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Inventory", menuName = "Inventory/Item")]
public class ItemInfo : ScriptableObject
{
    public enum ItemType
    { 
        Food,
        Herbs,
        Potion,
        Crafting,
        Weapon,
    }

    public string ItemName;
    [TextArea(3, 10)]
    public string ItemDescription;

    public int MaxStack;

    public float Weight;
    public float Value;

    public Sprite ItemImage;
}
