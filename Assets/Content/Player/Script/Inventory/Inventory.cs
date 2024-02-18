using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public List<InventoryObject> InventoryList = new List<InventoryObject>();
    [SerializeField] private float MaxWeight;
    [SerializeField] private float CurrentWeight;

    [SerializeField] private GameObject ErrorTextPanel;
    [SerializeField] private TextMeshProUGUI ErrorText;

    [SerializeField] private CraftingSystem _CraftingSystem;

    private PlayerController _PlayerController;


    private void Start()
    {
        _PlayerController = GetComponent<PlayerController>();
    }

    public bool CheckIfCanAddSomething(ItemInfo itemInfo, int amount = 1)
    {
        if (CurrentWeight + (itemInfo.Weight * amount) > MaxWeight)
        {
            StartCoroutine(ShowErrorText("To heavy inventory!"));
            return false;
        }
        else
        {
            AddItem(itemInfo, amount);
            return true;
        }
    }

   



    public IEnumerator ShowErrorText(string ErrorTextToAdd)
    {

        ErrorText.text = ErrorTextToAdd;
        ErrorTextPanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        ErrorTextPanel.SetActive(false);

    }


    private void AddItem(ItemInfo itemInfo, int amount = 1)
    {
        int remainingAmount = amount;

        while (remainingAmount > 0)
        {
            InventoryObject existingItem = InventoryList.Find(item => item.Name == itemInfo.ItemName && item.Amount < itemInfo.MaxStack && CurrentWeight + itemInfo.Weight <= MaxWeight);

            if (existingItem != null)
            {
                int spaceAvailable = itemInfo.MaxStack - existingItem.Amount;
                int amountToAdd = Mathf.Min(remainingAmount, spaceAvailable);
                existingItem.Amount += amountToAdd;
                remainingAmount -= amountToAdd;
                CurrentWeight += itemInfo.Weight;
                existingItem.StackValue += itemInfo.Value;
            }
            else
            {
                InventoryObject newItem = new InventoryObject
                {
                    Name = itemInfo.ItemName,
                    Description = itemInfo.ItemDescription,
                    Amount = Mathf.Min(remainingAmount, itemInfo.MaxStack),
                    ItemImage = itemInfo.ItemImage,
                    StackValue = itemInfo.Value
                };

                InventoryList.Add(newItem);
                remainingAmount -= newItem.Amount;
                CurrentWeight += itemInfo.Weight;
            }
        }

        CheckUI();
    }

    public void RemoveItem(string itemName, int amount = 1)
    {
        int remainingAmount = amount;
        float removedWeight = 0f;

        for (int i = InventoryList.Count - 1; i >= 0 && remainingAmount > 0; i--)
        {
            InventoryObject item = InventoryList[i];

            if (item.Name == itemName)
            {
                int amountToRemove = Mathf.Min(item.Amount, remainingAmount);
                item.Amount -= amountToRemove;
                remainingAmount -= amountToRemove;
                removedWeight += item.StackValue * amountToRemove; 
                CurrentWeight -= item.StackValue * amountToRemove; 

                if (item.Amount == 0)
                {
                    InventoryList.RemoveAt(i);
                }
            }
        }

        CheckUI();
    }


    private void CheckUI()
    {
        if (CurrentWeight >= MaxWeight - 5f)
        {
            _PlayerController.SetWeightSpeed(true);
        }
        else if (CurrentWeight > MaxWeight) 
        {
            _PlayerController.SetWeightOver();
        }
        else
        {
            _PlayerController.SetWeightSpeed(false);
        }
    }

}

[Serializable]
public class InventoryObject
{
    public string Name;
    public string Description;
    public int Amount;
    public Sprite ItemImage;
    public float StackValue;
}