using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_PlayerLongRangeAttack : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab für das Projektil
    public Transform firePoint; // Der Punkt, an dem das Projektil abgefeuert wird
    public float projectileSpeed = 10f; // Geschwindigkeit des Projektils
    public float attackCooldown = 0.5f; // Abkühlzeit zwischen den Angriffen
    private float lastAttackTime = 0f; // Zeitpunkt des letzten Angriffs

    void Update()
    {
        // Überprüfen, ob der Spieler die Long-Range-Angriffs-Taste drückt (hier auf "R" festgelegt)
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                LongRangeAttack(); // Spieler führt den Long-Range-Angriff aus
                lastAttackTime = Time.time; // Zeitstempel für den letzten Angriff
            }
        }
    }

    // Methode für den Long-Range-Angriff
    void LongRangeAttack()
    {
        Debug.Log("Long-Range-Angriff ausgelöst!");
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            if(transform.localScale.x > 0)
            {
                rb.linearVelocity = firePoint.right * projectileSpeed;
            }
            else
            {
                rb.linearVelocity = firePoint.right * -projectileSpeed;
            }
             // Setzt die Geschwindigkeit des Projektils
        }
    }
}
