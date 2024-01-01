using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;// This component allows the Projectile to use physics. The physics in this case are useful for colliding, etc.

    // Awake is called immediately when the object is created (when Instantiate is called),
    // so Rigidbody2d is properly initialized before calling Launch. 
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Launch will be used to move the Rigidbody: the higher the force, the faster it goes.
    public void Launch(Vector2 direction, float force)
    {
        // AddForce function call on the RigidBody == force = direction multiplied by the force.
        // When the force is added, the Physics Engine will move the Projectile every frame based on that force & direction.
        // By giving a force to the Rigidbody, the Physics System moves the RigidBody for you based on the force,
            // and you don't have to change the position in the Update function manually.
        rigidbody2d.AddForce(direction * force);
    }

    private void Update()
    {
        // Check if the cog's distance from the center of the world is far away enough from Ruby that she'll never reach it (say 1000 for our game),
            // then destroy the cog.
        if(transform.position.magnitude > 1000.0f) 
            // position is the vector from the center of our world.
            // magnitude is the length of that vector.
            // So the magnitude of the position is the distance to the center.
        {
            Destroy(gameObject); // gameObject = the current class's gameObject variable.
        }
    }

    // When the Projectile (Cog) hits the Enemy, the enemy fixes itself.
    private void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController e = other.collider.GetComponent<EnemyController>(); 
        if(e != null)
        {
            e.Fix();
        }
        // Destroy the projectile cog whenever it hits anything.
        Destroy(gameObject); // gameObject = the current class's gameObject variable.
    }

}
