using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    // Betrag, der dem Spieler im Todesbereich zugef�gt wird
    private const int damageAmount = 100; // Setze auf den maximalen Gesundheit

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �berpr�fen, ob der Spieler den Collider betritt
        S_PlayerHealth playerHealth = other.GetComponent<S_PlayerHealth>();
        if (playerHealth != null)
        {
            // F�ge dem Spieler Schaden zu, setze seine Gesundheit auf 0
            playerHealth.TakeDamage(damageAmount);
            Debug.Log("Der Spieler ist in die Todeszone geraten und nimmt Schaden!");
        }
    }
}


