using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Rigidbody2D rigidbody2d; // This component allows the Enemy to use physics. The physics in this case are useful for colliding, etc.
    public float speed = 3.0f;
    // vertical = if vert is true, then move enemy along y-axis, otherwise move along x-axis.
    public bool vertical;
    // changeTime = time before you reverse the enemy's direction.
    public float changeTime = 3.0f;
    // time = keep the current value of the timer.
    float timer;
    // direction = the enemy's current direction: either 1 or -1.
    int direction = 1;

    Animator animator; // Interface to control the Mecanim animation system.

    bool broken = true; // Initialize to true, so the Robot starts broken.

    public ParticleSystem smokeEffect; // For use to stop the smokeEffect when the Robot is fixed.

    public AudioClip RubyCollideRobotClip; // Public so you can attach an AudioClip in the Unity Inspector.
    public AudioClip fixRobotClip; // Public so you can attach an AudioClip in the Unity Inspector.

    AudioSource audioSource; // To store the Audio Source.

    // Start is called before the first frame update
    // Initialize the timer to the time before you reverse the enemy's direction.
    void Start()
    {
        // Initializing the created variables to something so it's not null.
        rigidbody2d = GetComponent<Rigidbody2D>();
        timer = changeTime; // Set the initial value of timer to the configured changeTime.
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Retrieve the audioSource in the Start function w/ GetComponent.
    }

    // Decrement the timer, test to see if it's less than 0, and if it is, change the direction & reset the timer.
    // Used for non-physics.
    void Update()
    {   
        // If broken is false, !broken will be true, and return will be executed.        
        if (!broken)
        {
            return;
        }
        // If broken is true, !broken will be false, and return won’t be executed.
            // A broken Robot will then be running around.
        timer -= Time.deltaTime; // Decrement the time that has passed since the last frame.
        // If you run out of time, then reverse direction & reset the timer.
        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    // Physics update always happens 50x per second. 
    void FixedUpdate()
    {
        // If broken is false, !broken will be true, and return will be executed.
        if (!broken)
        {
            return;
        }
        // If broken is true, !broken will be false, and return won’t be executed.
            // A broken Robot will then be running around.
        Vector2 position = rigidbody2d.position;
        // Mupltiply speed by the direction.
        if (vertical)
        {
            position.y = position.y + speed * direction * Time.deltaTime;
            // When the Robot moves vertically, 0 is sent to the horizontal parameter
            // And the direction will define whether the Robot moves up or down.
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + speed * direction * Time.deltaTime;
            // When the Robot is moving horizontally, send the inverse.
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }
        rigidbody2d.MovePosition(position);
    }

    // OnCollisionEnter2D is called on the frame when a collision happens.
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Check whether the other gameObject involved in the collision is Ruby.
        RubyController player = other.gameObject.GetComponent<RubyController>();
        // If it is Ruby, than damage her.
        if (player != null)
        {
            player.ChangeHealth(-1);
            // When Ruby runs into a Robot, then play sound clip:
            player.PlaySound(RubyCollideRobotClip); // Call on RubyController's PlaySound() function & pass in the collectedClip to play sound.
        }
    }

    // Fix() method is called to fix the robot.
    //Public because we want to call it from elsewhere like the projectile script.
    public void Fix()
    {
        broken = false;
        rigidbody2d.simulated = false;
        // This removes the Rigidbody from the Physics System simulation, so it won't be taken into account
            // by the system for collision, & the fixed robot won't stop the Projectile anymore
            // or be able to hurt the main character.
        // I.e., once the Robot is fixed and no longer broken, it's no longer running around.
        animator.SetTrigger("Fixed");
        smokeEffect.Stop();
        audioSource.PlayOneShot(fixRobotClip);
    }

}
