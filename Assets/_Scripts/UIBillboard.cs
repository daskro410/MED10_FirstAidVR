using UnityEngine;

public class UIBillboard : MonoBehaviour
{
    public Transform target; // The player's camera (Main Camera)

    void Start()
    {
        if (target == null)
        {
            target = Camera.main?.transform; // Fallback to Main Camera if not assigned
        }
    }

    void Update()
    {
        if (target == null) return;

        // Get the direction from the canvas to the player
        Vector3 direction = transform.position - target.position;

        // Ensure the canvas is always facing the player
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Correct the rotation so the UI is upright and facing the camera
        transform.rotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y, 0f);
    }
}