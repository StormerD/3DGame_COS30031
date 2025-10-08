using UnityEngine;

public class ShowChildrenPosition : MonoBehaviour
{
    void OnDrawGizmosSelected()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) Debug.DrawLine(transform.position, transform.GetChild(i).position, Color.red);
    }
}
