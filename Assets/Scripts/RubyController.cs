using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MonoBehavior is a base class that offers life cycle functions that make it easier to develop w/ Unity.
public class RubyController : MonoBehaviour
    // RubyController is a component.
    // Ruby is a gameObject.
{
    Rigidbody2D rigidbody2d; // This component allows Ruby to use physics. The physics in this case are useful for colliding, etc.
    public int maxHealth = 5; // A C# Public Instance Variable (i.e. Field)  that can be accessed via the Unity UI.
    public int health { get { return currentHealth; } } // A C# Property, a field that comes w/ getters and/or setters.
    int currentHealth; // Private fields, the ones I keep to implement my code.

    public float speed = 3.0f;
    float horizontal;
    float vertical;

    public float timeInvincible = 2.0f; // Time you remain Invincible.
    bool isInvincible;
    float invincibleTimer; // Track how much time left for invincibility.

    // Interface to control the Mecanim animation system.
    Animator animator; 
    // Why lookDirection? >> Unlike the Robot, Ruby can stand still. When Ruby stands still, Move X and Y are both 0, so the State Machine 
    // doesn't know which direction to use unless we tell it.
    // lookDirection stores the direction that Ruby is looking so you can always provide a direction to the State Machine.
    Vector2 lookDirection = new Vector2(1, 0); // Vector2 is a 2D direction with a (X, Y) component.

    // GameObject is an Entity >> anything in a game that can have different behaviors or logic.
    public GameObject projectilePrefab; // A prototype projectile - i.e. something you base the projectiles off of.
        // Prefab is meant to be cloned, not used directly.

    public ParticleSystem hitEffect; // For use to start the hitEffect for a limited duration when Ruby is hit.
    public ParticleSystem healthEffect; // For use to start the healthEffect for a limited duration when the HealthCollectible is claimed by Ruby.

    public AudioClip throwCogClip; // Public so you can attach an AudioClip in the Unity Inspector.

    AudioSource audioSource; // To store the Audio Source.

    // Start is called before the first frame update
    void Start()
    {
        // Initializing the created variables to something so it's not null.
            // The main components you access a lot during the game, you grab during start &
            // store them in a private field for use later.
        // GetComponent >> 	Gets a reference to a component of type T on the same GameObject as the component specified.
            // myResults = GetComponent<ComponentType>()
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Retrieve the audioSource in the Start function w/ GetComponent.
    }
     
    // Update is called once per frame.
    // Used for non-physics.
    void Update()
    {
        // The direction in which the keyboard is telling Ruby to move in this frame.
        horizontal = Input.GetAxis("Horizontal"); // "Horizontal" is tied to the Project Settings >> Input Manager settings.
        vertical = Input.GetAxis("Vertical");

        // Using horizontal & vertical, you get the directional vector that Ruby moves in this frame.
        Vector2 move = new Vector2(horizontal, vertical);

        // Check to see if either move.x or move.y are not equal to 0.
            // Use Mathf.Approximately instead of ==, b/c the way computers store float numbers
            // means there is a tiny loss in precision.
            // Approximately takes imprecision into account & will return true if the number can be considered equal minus the precision.
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            // If either x or y isn't equal to 0, then Ruby is moving.
            // Set your look direction to the move vector and Ruby should look in the direction that she is moving.
            // If Ruby stops moving (Move x and y are 0) then look will remain as the value it was just before she stopped moving.
            lookDirection.Set(move.x, move.y);
            // We Normalize to make the vector's length equal to 1, b/c we only care about the direction.
            lookDirection.Normalize();
        }
        // The next 3 following lines sends the data to the Animator, which is the direction you look in and the speed (length of the move vector).
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime; // Subtract the time that has passed since the last frame.
            if(invincibleTimer < 0) // If invincibleTimer ran out, than Ruby is no longer Invincible.
            {
                isInvincible = false;
            }
        }
        // Detect when the player presses a key for a Cog and call Launch when they do.
        // Input = interface into the input system. KeyCode maps to the physical keys.
        if (Input.GetKeyDown(KeyCode.C)) // GetKeyDown: Returns true during the frame the user starts pressing down the key identified by name.
        {
            Launch();
        }

        // Raycasting is the action of casting a ray in the Scene & checkign to see if that ray intersects with a Collider.
            // A ray has a starting point, a direction & length.
            // The term to “cast” a ray is used because the test is made from the starting point all along the ray until its end.
        if (Input.GetKeyDown(KeyCode.X)) // Check if the X key is pressed. This will be my "talk" button.
        {
            // If X is pressed, then enter the if block & start the Raycast.
            // RaycastHit2D >> Returns information about an object detected by a raycast in 2D physics.
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            // Physics2D.Raycast declaration:
                // public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance = Mathf.Infinity,
                // int layerMask = DefaultRaycastLayers, float minDepth = -Mathf.Infinity, float maxDepth = Mathf.Infinity);

            // Finally, test to see if your Raycast has hit a Collider. If the Raycast didn’t intersect anything, this will be null so do nothing.
            // Otherwise, RaycastHit2D will contain the Collider the Raycast intersected,
            // so you will go inside your final if block to log the object you have just found with the Raycast.
            // Check if we have a hit:
            if (hit.collider != null)
            {
                Debug.Log("Raycast has hit the object " + hit.collider.gameObject);
                // Try to find a NonPlayerCharacter script on the object the Raycast hit.
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    // If that script exists on that object, then display the dialog.
                    character.DisplayDialog();
                }
            }
        }
    }

    void FixedUpdate() // Physics update always happens 50x per second. 
    {
        // Define local Vector2 variable that represents the position of Ruby's physics component (x, y) used for colliding w/ other things.
        Vector2 position = rigidbody2d.position; 
        // Calculate the new X & Y positions:
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;
        // Reset the rigidbody2d's position.
        rigidbody2d.MovePosition(position);
    }

    // ChangeHealth is called from HealthCollectible.cs, EnemyController.cs, & DamageZone.cs.
    public void ChangeHealth(int amount)
    {
        // If I am being damaged:
        if (amount < 0)
        {
            // Trigger the hit animation.
            animator.SetTrigger("Hit");
            // If I am Invincible, then do nothing, do not change the health anymore.
            if (isInvincible)
            {
                return;
            }
            // If I am not Invincible, then make me Invincible for the next 2 seconds.
            isInvincible = true;
            invincibleTimer = timeInvincible;
            hitEffect.Play(); // If reach here, then it means Ruby was damaged, therefore play the hitEffect.
        }
        // Clamp means to limit the new currentHealth value between min and max numbers.
        // Clamp(float value, float min, float max)
        int newHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth); // Alina deviated from Ruby tutorial.
        if (newHealth > currentHealth)
        {
            healthEffect.Play(); // If reach here, then it means a HealthCollectible was claimed by Ruby, therefore play the healthEffect.
        }
        currentHealth = newHealth; // Alina deviated from Ruby tutorial.
        // Below updates the health bar dynamically during gameplay.
        Debug.Log("currentHealth " + currentHealth + "/ maxHealth " + maxHealth);
        // Adjust the healthBar.
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
            // Making the denominator a float so we get a float result rather than an integer.
    }

    // Launch function is called when I want to launch a Projectile cog (such as when a keyboard key is pressed).
    void Launch()
    {
        // Create a new projectileObject clone based off the projectilePrefab at the specified position & rotation.
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up*0.5f, Quaternion.identity);
            // Instantiate: Clones the object original and returns the clone.
                // Instantiate(Object original, Vector3 position, Quaternion rotation);
            // Quaternion are mathematical operators that express rotation.
                // Quaternion.identity == no rotation.
        // Grab the custom Projectile component from the new projectileObject.
        Projectile projectile = projectileObject.GetComponent<Projectile>(); // Pass in custom Projectile class as the argument.
        // Call the method Launch, which is in the Projectile.cs, w/ the direction being where the charactor is looking & the force value field set at 300.
        // Launch(Vector2 direction, float force)
        projectile.Launch(lookDirection, 300); // Direction in which you launch it, is the direction in which you are looking.
        // Trigger has been set for the Animator. This will make the Animator play the launching animation.
        animator.SetTrigger("Launch");
        // When Ruby launches a cog, then play sound clip:
        audioSource.PlayOneShot(throwCogClip);
    }

    public void PlaySound(AudioClip clip)
    {   
        // PlayOneShot takes the audio clip as its first parameter & plays that audio clip once, 
            // with all the settings of the Audio Source, at the position of the Audio Source.
        audioSource.PlayOneShot(clip);
    }

}
