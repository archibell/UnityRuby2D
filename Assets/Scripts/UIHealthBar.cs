using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    // Static members are shared by all objects of that type. This allows access to the variable using the class name instead of the reference.
    // B/c below is both public & static, I can write UIHealthBar.instance in any script & it will call this property.
        // The set property is private b/c we don't want people to be able to change it from outside this script.
        // The `static` feature makes sure this particular setup is a `Singleton`, b/c we only want 1 object of the healthBar to exist.
    public static UIHealthBar instance {  get; private set; }

    public Image mask;
    float originalSize;

    // Awake function is called as soon as the object is created, which is when the game starts.
    // The static instance `this` means "the object that currently runs that function".
    void Awake()
    {
        instance = this;
        // When the game starts, your Health bar script get its Awake function call and stores itself in the static member called “instance”.
        // So, if in any other script you call UIHealthBar.instance, then the value it will return to that script is the Health bar in our Scene.
    }

    // Start is called before the first frame update
    void Start()
    {
        originalSize = mask.rectTransform.rect.width;
        // RectTransform >> used to store & manipulate position, size & anchoring of a rectangle & supports 
            // various forms of scaling based on a parent RectTransform.
        // rect.width >> getting the size on screen.
    }

    public void SetValue(float value)
    {
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);
        // SetSizeWithCurrentAnchors(RectTransform.Axis axis, float size);
            // SetSizeWithCurrentAnchors >> makes the RectTransform calculated rect be a given size on the specified axis.
        // axis >> the axis to specify the size along.

    }
}
