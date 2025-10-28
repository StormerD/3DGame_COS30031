using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup), typeof(Animator))]
public class ComponentInventory : MonoBehaviour
{
    [SerializeField] private ItemEventObject _newItemPickedUpStream;
    [SerializeField] private ItemEventObject _someItemUsedStream;
    [SerializeField] private BasicEventObject _failedPickupBecauseFullStream;
    [Tooltip("Blank image with layout group element")]
    [SerializeField] private GameObject _displayComponentPrefab;
    [Tooltip("LayoutElement with a HorizontalLayoutGroup")]
    [SerializeField] private GameObject _rowPrefab;
    [SerializeField] private int _numComponentsPerRow;

    private Animator _animator;
    private int _countChildren;
    private GameObject _currentRow;

    void OnEnable()
    {
        _failedPickupBecauseFullStream.RegisterListener(OnPickupFailure);
        _newItemPickedUpStream.RegisterListener(OnNewItem);
        _someItemUsedStream.RegisterListener(OnUsedItem);
    }

    void OnDisable()
    {
        _failedPickupBecauseFullStream.UnregisterListener(OnPickupFailure);
        _newItemPickedUpStream.UnregisterListener(OnNewItem);
        _someItemUsedStream.UnregisterListener(OnUsedItem);
    }

    void Start()
    {
        _currentRow = Instantiate(_rowPrefab, transform);
    }

    void OnPickupFailure() => _animator.SetTrigger("PlayBagFull");
    void OnNewItem(IItem item)
    {
        Debug.Log("new item received");
        var temp = Instantiate(_displayComponentPrefab, _currentRow.transform);
        temp.name = item.GetId().ToString();
        RawImage i = temp.GetOrAddComponent<RawImage>();
        i.texture = item.GetObject2DRepresentation();
        _countChildren++;
    }

    void OnUsedItem(IItem item)
    {
        _countChildren--;
    }

    #region Debuggin
    public void AddFake()
    {
        _countChildren++;
    }
    
    public void RemoveOne()
    {
        int choice = Random.Range(0, _countChildren);
    }

    #endregion
}
