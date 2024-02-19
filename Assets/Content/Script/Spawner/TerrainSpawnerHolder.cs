using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpawnerHolder : MonoBehaviour
{
    [SerializeField]
    private Texture2D resourceMask;

    public Texture2D ResourceMask
    {
        get { return resourceMask; }
        set { resourceMask = value; }
    }


}
