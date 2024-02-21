using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class SpawnerItem : MonoBehaviour
{
    [SerializeField] private List<BiomColor> colorList = new List<BiomColor>();
    [SerializeField] private Terrain terrain; // Referencja do obiektu terenu
    [SerializeField] private Texture2D textureToCompare; // Tekstura do porównania

    void Update()
    {
        if (terrain == null || textureToCompare == null)
        {
            Debug.LogWarning("Terrain or Texture not set!");
            return;
        }

        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 terrainPosition = terrain.transform.position;
        Vector3 playerPosition = transform.position;

        // Mapowanie pozycji gracza na pozycjê na terenie (w zakresie od 0 do 1)
        float normalizedX = Mathf.InverseLerp(terrainPosition.x, terrainPosition.x + terrainSize.x, playerPosition.x);
        float normalizedZ = Mathf.InverseLerp(terrainPosition.z, terrainPosition.z + terrainSize.z, playerPosition.z);

        // Mapowanie pozycji na terenie na pozycjê w teksturze (w zakresie od 0 do szerokoœci/rozmiaru tekstury)
        int textureX = Mathf.FloorToInt(normalizedX * textureToCompare.width);
        int textureY = Mathf.FloorToInt(normalizedZ * textureToCompare.height);

        // Sprawdzenie koloru piksela na teksturze
        Color pixelColor = textureToCompare.GetPixel(textureX, textureY);
        Debug.Log("Current pixel color: " + pixelColor);

        foreach (var biomColor in colorList)
        {
            if (biomColor.ColorForBiom == pixelColor)
            {
                Debug.Log("Player entered marked area on the texture: " + biomColor.BiomName);
                break; // Mo¿emy przerwaæ pêtlê, gdy znaleziono dopasowanie koloru
            }
        }
    }
}

[Serializable]
public class BiomColor
{
    public string BiomName;
    public Color ColorForBiom;
    public List<ItemInfo> ItemForBiom = new List<ItemInfo>();
}