using UnityEngine;

public class TriggerWrapper : MonoBehaviour
{
    [SerializeField] private BasicEventObject triggersEventStream;
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entered trigger!");
        if (collision.gameObject.CompareTag("Player")) triggersEventStream.RaiseEvent();
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Entered trigger!");
        if (col.gameObject.CompareTag("Player")) triggersEventStream.RaiseEvent();
    }
}
