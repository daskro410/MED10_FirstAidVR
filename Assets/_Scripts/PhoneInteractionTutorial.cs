using UnityEngine;
using UnityEngine.UI;

public class PhoneInteractionTutorial : MonoBehaviour
{
    public Sprite[] phoneScreens;
    public Image phoneScreenImage; // Reference to the UI Image component for the phone screen

    public void OnPhoneActivate(){
    Debug.Log("Activation button pressed on phone.");

        int currentScreenIndex = System.Array.IndexOf(phoneScreens, phoneScreenImage.sprite);

        if (currentScreenIndex < phoneScreens.Length - 1)
        {
            int nextScreenIndex = currentScreenIndex + 1;
            phoneScreenImage.sprite = phoneScreens[nextScreenIndex];

            Debug.Log("Phone screen changed to: " + phoneScreens[nextScreenIndex].name);
        }
    }
}
