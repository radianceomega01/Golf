using UnityEngine;
using UnityEngine.Events;

public class EventInvoker : MonoBehaviour
{
    public UnityEvent OnInvoke;

    public void Invoke() => OnInvoke?.Invoke();
}
