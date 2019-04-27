using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public int health;
    public abstract void takeDamage(int damageTaken);
}
