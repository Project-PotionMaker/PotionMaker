using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float _previewYOffset = 0.06f;

    [SerializeField]
    private GameObject _cellIndicator;
    private GameObject _previewObject;
    private Renderer[] _previewObjectRenderers;

    [SerializeField]
    private Material _previewMaterial;
    private MaterialPropertyBlock _previewMaterialPropertyBlock;

    private Renderer _cellIndicatorRenderer;
    private MaterialPropertyBlock _cellIndicatorPropertyBlock;

    private void Awake()
    {
        _previewMaterialPropertyBlock = new MaterialPropertyBlock();
        _cellIndicatorPropertyBlock = new MaterialPropertyBlock();

        _cellIndicator.gameObject.SetActive(false);
        _cellIndicatorRenderer = _cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPlacementPreview(int structureTID, Vector2Int size)
    {
        _previewObject = StructureManager.Instance.CreateStructure(structureTID);
        PreparePreview(_previewObject);
        PrepareCursor(size);
        _cellIndicator.SetActive(true);

        Canvas canvas = _previewObject.GetComponentInChildren<Canvas>();
        if(canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }

        Collider[] colliders = _previewObject.GetComponentsInChildren<Collider>();
        foreach(Collider col in colliders)
        {
            col.enabled = false;
        }
    }

    private void PrepareCursor(Vector2Int size)
    {
        if(size.x > 0 || size.y > 0)
        {
            _cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            _cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        _previewObjectRenderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in _previewObjectRenderers)
        {
            Material[] materials = renderer.materials;
            for(int i = 0; i < materials.Length; i++)
            {
                materials[i] = _previewMaterial;
            }
            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        _cellIndicator.SetActive(false);
        if(ReferenceEquals(_previewObject, null) == false)
        {
            Destroy(_previewObject);
        }
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        if(_previewObject != null)
        {
            MovePreview(position);
            ApplyFeedbackToPreview(validity);
        }
        
        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }

    private void MovePreview(Vector3 position)
    {
        _previewObject.transform.position = new Vector3(position.x, position.y + _previewYOffset, position.z);
    }

    private void MoveCursor(Vector3 position)
    {
        _cellIndicator.transform.position = position;
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;

        foreach (Renderer renderer in _previewObjectRenderers)
        {
            _previewMaterialPropertyBlock.SetColor("_Color", c);
            renderer.SetPropertyBlock(_previewMaterialPropertyBlock);
        }
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        _cellIndicatorPropertyBlock.SetColor("_BaseColor", c);
        _cellIndicatorRenderer.SetPropertyBlock(_cellIndicatorPropertyBlock);
    }

    public void StartShowingRemovePreview()
    {
        _cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }
}
