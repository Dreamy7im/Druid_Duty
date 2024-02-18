using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingHolder : MonoBehaviour
{
    [field: SerializeField] public List<CraftingItem> CraftingObjectList {  get; private set; } = new List<CraftingItem>();


    [Tooltip("If true this is Build")]
    [field: SerializeField] public bool CraftOrBuild { get; private set; }

}


[Serializable]
public class CraftingItem
{
    public string name;
    [TextArea(2, 10)]
    public string description;
    public int LevelRequire;
    public List<NeedIngridient> NeedToBuild = new List<NeedIngridient>();
    public GameObject BuildingPrefab;
    public ItemInfo itemToCraft;
    public Sprite CraftingItemImage;
    public float CraftingTime;
}

[Serializable]
public class IngridientNeedToCraft
{
    public ItemInfo item;
    public int Amount;
}
