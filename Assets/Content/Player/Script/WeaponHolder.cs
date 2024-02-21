using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private List<WeaponScriptable> WeaponInEqu = new List<WeaponScriptable>();
    [SerializeField] private List <AmmoInEqu> AmmoInEquList = new List<AmmoInEqu>();

    [SerializeField] private Transform WeaponParent;
    private GameObject NewWeapon;

    public void AddWeapon(WeaponScriptable WeaponToAdd)
    {
        // Sprawd�, czy bro� ju� jest w ekwipunku
        if (!WeaponInEqu.Contains(WeaponToAdd))
        {
            WeaponInEqu.Add(WeaponToAdd);

            // Tworzenie instancji prefabrykatu broni
            if (WeaponToAdd.WeaponPrefab != null && WeaponParent != null)
            {
                NewWeapon = Instantiate(WeaponToAdd.WeaponPrefab, WeaponParent.position, WeaponParent.rotation);
            }
        }
        else
        {
            Debug.LogWarning("Bro� ju� znajduje si� w ekwipunku.");
        }
    }

    public void AddAmmo(string AmmoTag, int AmmoAmount)
    {
        // Sprawd�, czy istnieje ju� amunicja o podanym tagu
        AmmoInEqu existingAmmo = AmmoInEquList.Find(ammo => ammo.Name == AmmoTag);

        if (existingAmmo != null)
        {
            // Sprawd�, czy dodanie amunicji nie przekroczy limitu
            if (existingAmmo.CurrentAmmo + AmmoAmount <= existingAmmo.MaxAmmo)
            {
                existingAmmo.CurrentAmmo += AmmoAmount;
            }
            else
            {
                Debug.LogError("Nie mo�na doda� amunicji. Osi�gni�to maksymaln� ilo�� amunicji.");
            }
        }
        else
        {
            AmmoInEqu newAmmo = new AmmoInEqu()
            {
                Name = AmmoTag,
                CurrentAmmo = Mathf.Min(AmmoAmount, 100), 
                MaxAmmo = 100 
            };
            AmmoInEquList.Add(newAmmo);
        }
    }


    public void SelectWeapon(int WeaponIndex)
    {

    }

}

[Serializable]
public class AmmoInEqu
{
    public string Name;
    public int CurrentAmmo;
    public int MaxAmmo;
    
}