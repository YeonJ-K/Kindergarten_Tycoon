using System.Collections;
using UnityEngine;
using Infos;

public class KidsMoveController : MonoBehaviour
{
    [SerializeField] private GameObject StatusUI;
    [SerializeField] private RectTransform uiRT;
    [SerializeField] private PathManager pathManager;

    [SerializeField] private float moveTime = 0.3f;
    Animator animator;
    
    private IEnumerator GridSmoothMovement(Vector3 end)
    {
        Vector3 start = transform.position;
        Vector3 dir = (end - start);

        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveY", dir.y);

        float	current = 0;
        float	percent = 0;

        while ( percent < 1 )
        {
            current += Time.deltaTime;
            percent = current / moveTime;

            transform.position = Vector3.Lerp(start, end, percent);
            yield return null;
        }
        transform.position = end;
    }
        
}
