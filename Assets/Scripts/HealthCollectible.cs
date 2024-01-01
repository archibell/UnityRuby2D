using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public AudioClip collectHealthClip; // Public so you can attach an AudioClip in the Unity Inspector.

    // OnTriggerEnter2D = Sent when another object enters a trigger collider attached to this object (2D physics only).
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();
        // If it's Ruby and she is not at full health then heal her & destroy the collectible.
        if (controller != null)
        {
            if (controller.health < controller.maxHealth)
            {
                controller.ChangeHealth(1);
                Destroy(gameObject);
                // When Ruby runs into & collects a strawberry, then play sound clip:
                controller.PlaySound(collectHealthClip); // Call on RubyController's PlaySound() function & pass in the collectedClip to play sound.
            }
        }
       
    }
}
