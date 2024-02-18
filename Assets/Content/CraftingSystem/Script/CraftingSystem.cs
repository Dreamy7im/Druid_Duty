using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField] private GameObject CraftingUI;
    [SerializeField] private PlayerController _playerController;

    [SerializeField] private CraftingTimer _craftingTimer;

    [SerializeField] private Inventory _inventory;

    [SerializeField] private GameObject[] CraftingButton;

    private UnityAction BuildAction;
    private UnityAction craftingAction;

    [SerializeField] private BuildingSystem _buildingSystem;
    private Button TempButton;

    [field: SerializeField] public bool CraftWay {private get; set; }

    private bool Craft;

    private void Start()
    {
        _playerController.GetComponentInParent<PlayerController>();
        //_buildingSystem.GetComponentInParent<BuildingSystem>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && Craft)
        {
            EndCrafting();
        }
    }


    public void SetUpCrafting(List<CraftingItem> CraftingObjectListToAdd)
    {

        float angleBetweenButtons = 360f / CraftingObjectListToAdd.Count;

        for (int i = 0; i < CraftingObjectListToAdd.Count; i++)
        {
            float rotationAngle = i * angleBetweenButtons;
            CraftingButton[i].transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);

            int index = i; 
            CraftingButton[i].SetActive(true);

            switch (CraftWay)
            {
                case false:
                    {
                        craftingAction = () =>
                        {
                            if (CheckIfHasEnoughResources(CraftingObjectListToAdd[index].NeedToBuild) &&
                                _craftingTimer.StartCrafting(CraftingObjectListToAdd[index].itemToCraft.name, CraftingObjectListToAdd[index].CraftingTime))
                            {
                                _inventory.CheckIfCanAddSomething(CraftingObjectListToAdd[index].itemToCraft, 1);


                                EndCrafting(); 
                            }
                            else
                            {
                                _inventory.ShowErrorText("Not enought resources to create " + CraftingObjectListToAdd[index].itemToCraft.name);
                            }
                        };
                        break;
                    }
                case true:
                    {
                        craftingAction = () =>
                        {
                            _buildingSystem.StartBuild(CraftingObjectListToAdd[index]);
                            EndCrafting(); 
                        };
                        break;
                    }
            }

            BuildAction += craftingAction; 
            TempButton = CraftingButton[i].GetComponentInChildren<Button>();
            TempButton.onClick.AddListener(() => BuildAction()); 
            CraftingButton[i].GetComponent<CraftingButton>().SetUpButton(BuildAction, CraftingObjectListToAdd[i].CraftingItemImage, -angleBetweenButtons * i);
        }

        StartCrafting();
    }


    public bool CheckIfHasEnoughResources(List<NeedIngridient> neededResources)
    {
        Dictionary<string, int> inventoryItems = new Dictionary<string, int>();
        foreach (var item in _inventory.InventoryList)
        {
            if (!inventoryItems.ContainsKey(item.Name))
            {
                inventoryItems[item.Name] = item.Amount;
            }
            else
            {
                inventoryItems[item.Name] += item.Amount;
            }
        }

        Debug.Log("Items in Inventory:");
        foreach (var item in inventoryItems)
        {
            Debug.Log("- " + item.Key + " - Quantity: " + item.Value);
        }

        foreach (NeedIngridient neededResource in neededResources)
        {
            Debug.Log("Needed Item Name: " + neededResource.item.ItemName + " - Needed Amount: " + neededResource.Amount);

            if (!inventoryItems.ContainsKey(neededResource.item.ItemName) || inventoryItems[neededResource.item.ItemName] < neededResource.Amount)
            {
                return false;
            }
        }
        return true;
    }

    private void StartCrafting()
    {
        Craft = true;

        //_playerController.UsingMenuRotate();
        CraftingUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void EndCrafting()
    {
        Craft = false;

        for (int i = 0; i < CraftingButton.Length; i++)
        {
            CraftingButton[i].SetActive(false);
        }

        _playerController.UsingMenuRotate();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CraftingUI.SetActive(false);
    }


}