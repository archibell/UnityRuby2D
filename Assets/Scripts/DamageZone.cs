using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public AudioClip damageClip; // Public so you can attach an AudioClip in the Unity Inspector.

    // OnTriggerStay2D = Sent each frame where another object is within a trigger collider attached to this object (2D physics only)
    // Every frame, this code will be involked.
    void OnTriggerStay2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();
        // If Ruby is the one ontop of this damage zone, then damage her.
        if (controller != null)
        {
            controller.ChangeHealth(-1); // ChangeHealth is a method of RubyController.cs

            // When Ruby runs into Damageable hazard cones, then play sound clip:
            controller.PlaySound(damageClip); // Call on RubyController's PlaySound() function & pass in the collectedClip to play sound.
        }
    }

}
