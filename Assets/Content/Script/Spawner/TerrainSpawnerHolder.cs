using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class TerrainSpawnerHolder : MonoBehaviour
{
    [SerializeField]
    private Texture2D resourceMask;

    [SerializeField] private LayerMask TerrainLayer;


    [SerializeField] private List<GameObject> TerrainNeighbor = new List<GameObject>();
    [SerializeField] private Transform FinderHolder;
    [SerializeField] private Transform Finder;

    public bool AlreadyFindNeighbor;
    private float RotateDegree;

    private void Start()
    {
        FindNeighbor();
    }

    public void FindNeighbor()
    {
        if (!AlreadyFindNeighbor)
        {
            

            // Pobieramy rozmiary terenu
            Vector3 terrainSize = GetComponent<Terrain>().terrainData.size;

            // Pomijamy wysokoœæ terenu
            Vector2 terrainSizeWithoutHeight = new Vector2(terrainSize.x, terrainSize.z);

            FinderHolder.transform.position = new Vector3(terrainSizeWithoutHeight.x, 0, terrainSizeWithoutHeight.y);

            RaycastHit hit;

            for (int i = 0; i < 8; i++)
            {
                FinderHolder.rotation = Quaternion.Euler(0f, RotateDegree, 0f);
                if (Physics.Raycast(Finder.position, Vector3.down, out hit, Mathf.Infinity, TerrainLayer))
                {
                    TerrainNeighbor.Add(hit.collider.gameObject);
                }
                RotateDegree += 45f;
            }

            AlreadyFindNeighbor = true;
            
        }

    }



}

