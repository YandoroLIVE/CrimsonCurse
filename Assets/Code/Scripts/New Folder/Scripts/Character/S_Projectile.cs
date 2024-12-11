using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Projectile : MonoBehaviour
{
    public int damage = 20; // Schaden des Projektils
    public float lifetime = 5f; // Lebenszeit des Projektils
    public GameObject explsionVFX;
    private void Start()
    {
        Destroy(gameObject, lifetime); // Zerstört das Projektil nach der angegebenen Lebenszeit
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Überprüfen, ob das Projektil mit einem Gegner kollidiert
        S_GroundEnemy enemy = collision.gameObject.GetComponent<S_GroundEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage); // Schaden an den Gegner anwenden
            Debug.Log("Projektil verursacht " + damage + " Schaden.");
            Instantiate(explsionVFX, transform.position, transform.rotation).transform.localScale = new Vector3(.2f, .2f, .2f);
            Destroy(gameObject); // Zerstört das Projektil nach der Kollision
        }
    }
}