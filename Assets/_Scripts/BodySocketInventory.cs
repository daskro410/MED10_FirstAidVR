using UnityEngine;

[System.Serializable]
public class BodySocket
{
    public GameObject gameObject;
    [Range (0.01f, 1)] public float heightRatio;
}

public class BodySocketInventory : MonoBehaviour
{
    public PhoneInteractionBehavior phoneInteractionBehavior;

    public GameObject HMD;
    public BodySocket[] bodySockets;

    private Vector3 currentHMDPosition;
    private Quaternion currentHMDRotation;

    void Update()
    {
        currentHMDPosition = HMD.transform.position;
        currentHMDRotation = HMD.transform.rotation;

        foreach (var bodySocket in bodySockets)
        {
            UpdateBodySocketHeight(bodySocket);
        }

        UpdateBodySocketInventory();
    }

    public void UpdateBodySocketHeight(BodySocket bodySocket){
        bodySocket.gameObject.transform.position = new Vector3(bodySocket.gameObject.transform.position.x, currentHMDPosition.y * bodySocket.heightRatio, bodySocket.gameObject.transform.position.z);
    }

    public void UpdateBodySocketInventory(){
        transform.position = new Vector3(currentHMDPosition.x, 0, currentHMDPosition.z);
        transform.rotation = Quaternion.Euler(transform.rotation.x, currentHMDRotation.eulerAngles.y, transform.rotation.z);
    }

    public void OnPhoneSocketEnter(){
        phoneInteractionBehavior.SetPhoneInteractionState(PhoneInteractionState.InPocket);
    }

    public void OnPhoneSocketExit(){
        phoneInteractionBehavior.SetPhoneInteractionState(PhoneInteractionState.InHand);
    }
}
