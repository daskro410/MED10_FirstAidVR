using System.Collections;
using System.Linq;
using UnityEngine;

public class JawHandler : MonoBehaviour
{
    public CustomInteractionManager customInteractionManager;
    public GameObject HMDCamera;

    public Animator jawAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        customInteractionManager = FindFirstObjectByType<CustomInteractionManager>(); // Find the CustomInteractionManager in the scene
    }

    public void OpenJaw(){
        jawAnimator.SetTrigger("TriggerJawOpen");
    }

    public void CloseJaw(){
        jawAnimator.SetTrigger("TriggerJawClose");
    }

    // Check the distance between the main camera and the object to determine if the player is close enough
    public bool IsHMDCloseWithinDistance(){
        float distance = Vector3.Distance(HMDCamera.transform.position, transform.position);
        //Debug.Log("Distance to HMD: " + distance);
        return distance < customInteractionManager.minHMDDistance; // Adjust the distance threshold as needed
    }

    private void OnDrawGizmos()
    {
        if (customInteractionManager == null) return;

        Gizmos.color = new Color(0f, 1f, 1f, 0.3f); // Light cyan with transparency
        Gizmos.DrawSphere(transform.position, customInteractionManager.minHMDDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, customInteractionManager.minHMDDistance);
    }
}
