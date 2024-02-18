using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ItemName;
    [SerializeField] private GameObject ItemNameParent;
    [SerializeField] private Camera _Camera;

    [SerializeField] private float SearchRange;
    [SerializeField] private LayerMask InteractionMask;
    [SerializeField] private LayerMask ItemLayer;

    [SerializeField] private Inventory InventoryScript;

    [SerializeField] private KeyCode InteractionKey;

    [SerializeField] private PlayerController _playerController;

    private PlayerStats _playerStats;

    private CraftingSystem _CraftingSystem;

    private bool wasHit = false; 
    private RaycastHit lastHit; 

    private void Start()
    {
        ItemNameParent.SetActive(false);
        _CraftingSystem = GetComponent<CraftingSystem>();
        _playerController.GetComponentInParent<PlayerController>();
        _playerStats = GetComponentInParent<PlayerStats>();
    }

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(_Camera.transform.position, _Camera.transform.forward, out hit, SearchRange, ItemLayer))
        {
            OnRaycastHitSomethingOnItemLayer(hit);

            return;
        }
        else
        {
            wasHit = false;
            ItemNameParent.SetActive(false);
        }

        if (Physics.Raycast(_Camera.transform.position, _Camera.transform.forward, out hit, SearchRange, InteractionMask))
        {
            ShowCraftingUI(hit);
        }

        

    }

    private void ShowCraftingUI(RaycastHit hit)
    {
        CraftingHolder RefCrafting = hit.collider.gameObject.GetComponent<CraftingHolder>();

        if (RefCrafting != null && Input.GetKeyDown(InteractionKey))
        {
            Debug.Log("StartCrafting");
            _playerController.UsingMenuRotate();


            _CraftingSystem.CraftWay = (RefCrafting.CraftOrBuild);
            _CraftingSystem.SetUpCrafting(RefCrafting.CraftingObjectList);
        }
    }

    private void OnRaycastHitSomethingOnItemLayer(RaycastHit hit)
    {
        if (hit.collider.gameObject.GetComponent<ItemInfoHolder>() && Input.GetKeyDown(InteractionKey))
        {
            Debug.Log("TryHarvest");
            if (InventoryScript.CheckIfCanAddSomething(hit.collider.gameObject.GetComponent<ItemInfoHolder>().ItemInfo))
            {
                hit.collider.gameObject.SetActive(false);
            }
        }

        if (!wasHit || hit.collider.gameObject != lastHit.collider.gameObject)
        {
            wasHit = true;
            lastHit = hit;

            ItemNameParent.SetActive(true);
            ItemName.text = hit.collider.gameObject.GetComponent<ItemInfoHolder>().ItemInfo.ItemName;
        }

    }
}