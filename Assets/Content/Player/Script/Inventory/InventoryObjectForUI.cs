using TMPro;
using UnityEngine;

public class InventoryObjectForUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ItemName;
    [SerializeField] private TextMeshProUGUI ItemAmount;


    public void SetItem(string ItemNameToAdd, int ItemAmountToAdd)
    {
        ItemName.text = ItemNameToAdd;
        ItemAmount.text = ItemAmountToAdd.ToString();
    }
}
