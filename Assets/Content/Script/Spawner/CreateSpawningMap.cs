using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSpawningMap : MonoBehaviour
{
    public Terrain[] terrains; // Tablica obiekt�w terenu
    public int textureSize = 512; // Rozmiar tekstury (zmniejszony dla lepszej wydajno�ci)

    private int TerrainCount;

    void Start()
    {
        foreach (Terrain terrain in terrains)
        {
            GenerateTexture(terrain);
            TerrainCount++;
        }
    }

    void GenerateTexture(Terrain terrain)
    {

        terrain.gameObject.name = "Terrain_" + TerrainCount;
        // Pobieranie danych alphamap
        float[,,] alphamapData = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);

        // P�tle iteruj�ce po pikselach tekstury
        Texture2D texture = new Texture2D(textureSize, textureSize);
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                // Pobieranie wsp�rz�dnych dla alfamapy
                int alphamapX = Mathf.FloorToInt((float)x / textureSize * terrain.terrainData.alphamapWidth);
                int alphamapY = Mathf.FloorToInt((float)y / textureSize * terrain.terrainData.alphamapHeight);

                // Inicjalizacja koloru
                Color color = Color.black; // Domy�lny kolor, aby unikn�� b��du

                // Sprawdzenie czy istniej� dane alfamapy dla danego punktu
                if (alphamapX < terrain.terrainData.alphamapWidth && alphamapY < terrain.terrainData.alphamapHeight)
                {
                    // Konstrukcja koloru na podstawie warstw tekstur
                    for (int layer = 0; layer < alphamapData.GetLength(2); layer++)
                    {
                        // Pobranie koloru z danej warstwy
                        Color layerColor = terrain.terrainData.terrainLayers[layer].diffuseTexture.GetPixel(alphamapX, alphamapY);
                        // Warto�� alfamapy dla danego punktu i warstwy
                        float alphaValue = alphamapData[alphamapY, alphamapX, layer];
                        // Sumowanie kolor�w warstw, wa�onych przez warto�� alfamapy
                        color += layerColor * alphaValue;
                    }
                }

                // Ustawienie koloru piksela w teksturze
                texture.SetPixel(x, y, color);
            }
        }

        // Zastosowanie zmian do tekstury
        texture.Apply();

        terrain.GetComponent<TerrainSpawnerHolder>().ResourceMask = texture;

        // Zapisanie tekstury do pliku PNG
        byte[] textureBytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Content/Script/Spawner/BiomColor/TerrainMap/BiomTexture_" + terrain.name + ".png", textureBytes);

        Debug.Log("Texture generated and saved for terrain: " + terrain.name);
    }
}