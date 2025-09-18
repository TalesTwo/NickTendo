using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDamageController : MonoBehaviour
{
    public int damage = 2;

    // later, for upgrading in the future
    public void UpgradeDamage(int upgrade)
    {
        damage += upgrade;
    }
}
