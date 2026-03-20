using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    [SerializeField]
    WeaponsObjects[] weapons;

    int current_weapon = 0;
    float currentweapon_accum = 1;

    private void Update()
    {
        float scrollamt = Input.GetAxis("Mouse ScrollWheel");
        
        if (Mathf.Abs(scrollamt) > 0f)
        {
            currentweapon_accum += scrollamt * 10;
            currentweapon_accum = Mathf.Clamp(currentweapon_accum, 0, weapons.Length - 1+0.9f);
        }

        if (current_weapon != (int)currentweapon_accum)
        {
            weapons[current_weapon].objs.ForEach(x => x.SetActive(false));
            current_weapon = (int)currentweapon_accum;
            weapons[current_weapon].objs.ForEach(x => x.SetActive(true));
            currentweapon_accum = current_weapon;
        }
    }
}
[System.Serializable]
public struct WeaponsObjects
{
    public List<GameObject> objs;
}
