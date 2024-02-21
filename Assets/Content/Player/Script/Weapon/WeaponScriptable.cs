using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Weapon")]
public class WeaponScriptable : ScriptableObject
{

    public string Name;
    public int MaxAmmoAmount;

    public enum FireType
    {
        Single,
        SemiAuto,
        Auto
    }

    public FireType type;

    public float FireRate;

    public GameObject WeaponPrefab;
}
