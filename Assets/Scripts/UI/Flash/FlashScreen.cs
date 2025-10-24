

using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Animator))]
public class FlashScreen : MonoBehaviour
{
    [SerializeField] private EventObject flashStarts;
    [SerializeField] private EventObject flashEnds;
    [SerializeField] private RebuildableObjectBase rebuildable;
    private Animator _screenAnimator;

    void Start()
    {
        _screenAnimator = GetComponent<Animator>();
        rebuildable.OnCompletedRebuild += ExecuteFlash;
    }

    protected void ExecuteFlash() => _screenAnimator.SetTrigger("PlayFlash");
    protected void RaiseFlashStarted() => flashStarts.RaiseEvent();
    protected void RaiseFlashEnded() => flashEnds.RaiseEvent();
}