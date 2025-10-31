using UnityEngine;

public class TriggerWrapper : MonoBehaviour
{
    [SerializeField] private BasicEventObject triggersEventStream;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) triggersEventStream.RaiseEvent();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player")) triggersEventStream.RaiseEvent();
    }
}
