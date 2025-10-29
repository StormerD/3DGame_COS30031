using System.Collections;
using TMPro;
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
    [SerializeField] private GameObject bagFullTextWrapper;
    [SerializeField] private int minimumDisplayHeight = 64;

    private Animator _animator;
    private int _countChildren;
    private GameObject _currentRow;

    void Awake() => _animator = GetComponent<Animator>();

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

    void OnPickupFailure() => _animator.SetTrigger("BagFull");
    void OnNewItem(IItem item)
    {
        var temp = Instantiate(_displayComponentPrefab, _currentRow.transform);
        temp.name = item.GetId().ToString();
        RawImage i = temp.GetOrAddComponent<RawImage>();
        Texture2D texture = item.GetObject2DRepresentation();
        RectTransform rect = temp.GetComponent<RectTransform>();

        // assign texture
        i.texture = item.GetObject2DRepresentation();
        // set it to the native size of the object
        i.SetNativeSize();
        // tell the layout group we want to maintain this size (don't stretch)
        LayoutElement e = temp.GetComponent<LayoutElement>();
        float scaleFactor = 1;
        if (texture.height < minimumDisplayHeight) // gonna re-size the image to make it a little more visible
        {
            scaleFactor = minimumDisplayHeight / texture.height;
            Debug.Log("Using scale factor! " + scaleFactor);
        }
        rect.sizeDelta = new(texture.width * scaleFactor, texture.height * scaleFactor);
        e.preferredHeight = texture.height * scaleFactor;
        e.preferredWidth = texture.width * scaleFactor;

        StartCoroutine(FadeImage(i));

        // now that the child is added we need to see if we should make a new row (for large bags)
        // and we also need to update the position of BagFullTextWrapper

        _countChildren++;
        _animator.SetBool("ObjectsHeld", true);
    }

    void OnUsedItem(IItem item)
    {
        string itemId = item.GetId().ToString();
        // iterate through child objects and find the one matching ID; delete it.
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform curChild = transform.GetChild(i);
            bool found = false;
            for (int j = 0; j < curChild.childCount; j++)
            {
                if (curChild.GetChild(j).name == itemId)
                {
                    StartCoroutine(FadeImage(curChild.GetChild(j).GetComponent<RawImage>(), 1, 0, true));
                    break;
                }
            }
            if (found) break;
        }
        _countChildren--;
        if (_countChildren == 0) _animator.SetBool("ObjectsHeld", false);
    }

    private IEnumerator FadeImage(RawImage image, float from = 0, float to = 1, bool destroyAtCompletion = false, float overTime = 0.25f, float stepSize = 0.01f)
    {
        Color imgColor = new(image.color.r, image.color.g, image.color.b);
        image.color = new(imgColor.r, imgColor.g, imgColor.b, from);
        float stepAmount = stepSize / overTime * (to - from);
        float curAlpha = from;
        WaitForSeconds wait = new(stepSize);
        int stepCount = (int)((to - from) / stepAmount);
        for (int i = 0; i < stepCount; i++)
        {
            yield return wait;
            curAlpha += stepAmount;
            image.color = new(imgColor.r, imgColor.g, imgColor.b, curAlpha);
        }
        image.color = new(imgColor.r, imgColor.g, imgColor.b, to);

        if (destroyAtCompletion) Destroy(image.gameObject);
    }
}
