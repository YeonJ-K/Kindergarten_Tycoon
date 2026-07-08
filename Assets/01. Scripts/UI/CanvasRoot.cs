using UnityEngine;

public class CanvasRoot : MonoBehaviour
{
    public static CanvasRoot instance = null;

    [Header("UI Canvas Parents")]
    public Transform trScreenParent;

    public Transform trHudParent;
    public Transform trPopupParent;
    
    [Header("UI Loading")]
    public GameObject objLoading;
}
