using UnityEngine;

public class HandData : MonoBehaviour
{
    public enum HandType
    {
        Left,
        Right
    }

    public HandType handType;
    public Transform root;
    public Animator animator;
    public Transform[] fingerBones;
}
