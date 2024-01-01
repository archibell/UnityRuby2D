using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
    public float displayTime = 4.0f; // displayTime will store how long in seconds our dialog box is displayed. 
    public GameObject dialogBox; // dialogBox will store the Canvas GameObject, so you can enable/disable script.
    float timerDisplay; // timerDisplay will store how long to display our dialog.

    // Start is called before the first frame update
    void Start()
    {
        // In the Start function, make sure the dialogueBox is disabeled & initialise the timerDisplay to -1.
        dialogBox.SetActive(false);
        timerDisplay = -1.0f;
    }

    // Update is called once per frame
    // In the Update function, check whether the dialog is currently displayed by testing if timerDisplay is superior or equal to 0. 
    void Update()
    {
        // If it is greater than zero, then the dialog is currently being displayed.
        if (timerDisplay >= 0)
        {
            // In this case, you will decrease Time.deltaTime to count down and then check if your timerDisplay has reached 0.
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                // This means it’s time to hide your dialog box again, so you will need to disable the dialog box:
                dialogBox.SetActive(false);
            }
        }
    }

    // A public function called DisplayDialog that your RubyController will call when Ruby interacts with the NPC frog.
    public void DisplayDialog()
    {
        // This function will show the dialog box and initialize the timeDisplay to the displayTime setup:
        timerDisplay = displayTime;
        dialogBox.SetActive(true);
    }


}
