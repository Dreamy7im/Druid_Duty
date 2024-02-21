using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    private CraftingItem ItemToCraft;
    [SerializeField] private Camera _Camera;

    [SerializeField] private float SearchRange;
    [SerializeField] private LayerMask GroundMask;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationDeegree;

    [SerializeField] private bool RotationDegree;

    private Inventory InventoryObject;

    private GameObject TempBuildingGhost;
    private Material BuildingMaterial;

    [SerializeField] private Material GhostMaterial;
    [SerializeField] private LineRenderer _LineRenderer;

    [SerializeField] private Transform LineRendererStartPoint;

    [SerializeField] private LayerMask CollisionLayer;


    private bool CurrentBuilding;
    private bool CanBuilding;
    private bool collidesWithOtherObjects;

    private RaycastHit hit;

    private void Start()
    {
        InventoryObject = GetComponent<Inventory>();
    }


    public void StartBuild(CraftingItem BuildingObject)
    {
        CurrentBuilding = true;

        

        // Pobieramy budynk z listy na podstawie indeksu
        ItemToCraft = BuildingObject;

        TempBuildingGhost = Instantiate(ItemToCraft.BuildingPrefab);

        // Sprawdzamy, czy mamy wystarczaj¹c¹ iloœæ sk³adników
        bool hasEnoughResources = CheckIfHasEnoughResources(ItemToCraft.NeedToBuild);

        BuildingMaterial = TempBuildingGhost.GetComponentInChildren<Renderer>().material;
        TempBuildingGhost.GetComponentInChildren<Renderer>().material = GhostMaterial;


        if (!hasEnoughResources)
        {
            CanBuilding = false;
            GhostMaterial.SetInt("_CanBuild", 0);
        }
        else
        {
            CanBuilding = true;
            GhostMaterial.SetInt("_CanBuild", 1);
        }

        _LineRenderer.enabled = true;
        StartCoroutine(SetPlaceBuilding());
    }

    private bool CheckIfHasEnoughResources(List<NeedIngridient> neededResources)
    {
        // Tworzymy s³ownik dla ³atwiejszego odwo³ywania siê do przedmiotów w inventory
        Dictionary<string, int> inventoryItems = new Dictionary<string, int>();
        foreach (var item in InventoryObject.InventoryList)
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

        // Debug log dla nazw obecnych w inventory
        Debug.Log("Items in Inventory:");
        foreach (var item in inventoryItems)
        {
            Debug.Log("- " + item.Key + " - Quantity: " + item.Value);
        }

        foreach (NeedIngridient neededResource in neededResources)
        {
            // Debug log dla nazwy potrzebnego przedmiotu
            Debug.Log("Needed Item Name: " + neededResource.item.ItemName + " - Needed Amount: " + neededResource.Amount);

            // Sprawdzamy, czy przedmiot jest w inwentarzu i czy mamy wystarczaj¹c¹ iloœæ
            if (!inventoryItems.ContainsKey(neededResource.item.ItemName) || inventoryItems[neededResource.item.ItemName] < neededResource.Amount)
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator SetPlaceBuilding()
    {
        while (CurrentBuilding)
        {
            _LineRenderer.SetPosition(0, LineRendererStartPoint.transform.position);

            if (Physics.Raycast(_Camera.transform.position, _Camera.transform.forward, out hit, SearchRange, GroundMask))
            {
                _LineRenderer.SetPosition(1, hit.point);
                TempBuildingGhost.transform.position = hit.point;

                // Obracanie obiektu wokó³ osi Y na scrollu myszy
                float scrollInput = Input.GetAxis("Mouse ScrollWheel");
                if (scrollInput != 0f)
                {
                    // Obracaj obiekt proporcjonalnie do wartoœci scrolla
                    if (RotationDegree)
                    {
                        TempBuildingGhost.transform.Rotate(Vector3.up, rotationDeegree * Mathf.Sign(scrollInput));
                    }
                    else
                    {
                        TempBuildingGhost.transform.Rotate(Vector3.up, scrollInput * rotationSpeed);
                    }
                }

                if (CanBuilding)
                {
                    // SprawdŸ kolizje z innymi obiektami
                    collidesWithOtherObjects = CheckCollisions();
                }

                if (Input.GetMouseButtonDown(0) && CanBuilding && !collidesWithOtherObjects)
                {
                    // Umieœæ budynek w obecnej pozycji
                    TempBuildingGhost.transform.position = hit.point;
                    TempBuildingGhost.GetComponentInChildren<Renderer>().material = BuildingMaterial;

                    // Usuñ wymagan¹ do postawienia iloœæ obiektów z inventory
                    RemoveRequiredItemsFromInventory(ItemToCraft.NeedToBuild);

                    // Zakoñcz proces budowania
                    CurrentBuilding = false;
                }


                if (Input.GetMouseButtonDown(0) && !CanBuilding || Input.GetMouseButtonDown(1))
                {
                    Destroy(TempBuildingGhost);
                    // Zakoñcz proces budowania
                    CurrentBuilding = false;
                }

            }
            else
            {

            }

            yield return null; // Upewniamy siê, ¿e korutyna zwraca null w ka¿dej iteracji
        }

        _LineRenderer.enabled = false;

    }


    private void RemoveRequiredItemsFromInventory(List<NeedIngridient> neededResources)
    {
        foreach (NeedIngridient neededResource in neededResources)
        {
            InventoryObject.RemoveItem(neededResource.item.ItemName, neededResource.Amount);
        }
    }


    private bool CheckCollisions()
    {
        Collider[] colliders = Physics.OverlapBox(TempBuildingGhost.transform.position, TempBuildingGhost.GetComponentInChildren<BoxCollider>().bounds.extents);

        foreach (Collider collider in colliders)
        {
            // Jeœli collider nie jest na layerze CollisionLayer, oznacza to kolizjê z innym obiektem
            if (collider.gameObject.layer == CollisionLayer)
            {
                GhostMaterial.SetInt("_CanBuild", 0);

                // Wypisz collider, z którym nastêpuje kolizja
                Debug.Log("Collision with: " + collider.name);

                return true;
            }
        }

        GhostMaterial.SetInt("_CanBuild", 1);
        return false;
    }


}

[Serializable]
public class Building
{
    public string name;
    [TextArea(2,10)]
    public string description;
    public int LevelRequire;
    public List<NeedIngridient> NeedToBuild = new List<NeedIngridient>();
    public GameObject BuildingPrefab;
}

[Serializable]
public class NeedIngridient
{
    public ItemInfo item;
    public int Amount;
}