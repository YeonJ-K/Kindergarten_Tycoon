using UnityEngine;

[CreateAssetMenu(fileName = "Status", menuName = "Scriptable Objects/Status")]
public class Status : ScriptableObject
{
    [SerializeField] private Sprite goodStatus;
    [SerializeField] private Sprite normalStatus;
    [SerializeField] private Sprite badStatus;
}
