using Mirror;
using UnityEngine;

/// <summary>
/// 가구 배치 시 미리보기 오브젝트와 그리드 셀을 시각적으로 표시하는 클래스입니다.
/// 이 클래스는 오직 로컬 플레이어에게만 작동하는 클라이언트 전용 기능입니다.
/// </summary>
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

    public void StartShowingPlacementPreview(int tid, Vector2Int size)
    {
        // 주의: StructureManager.Instance.ServerCreateStructure가 아닌
        // 클라이언트에서 로컬 미리보기 오브젝트를 생성합니다.
        EStructureType type = DataTable.Instance.GetStructureData(tid).StructureType;
        //_previewObject = StructureFactory.Instance.CreateObject(type, transform.position, Quaternion.identity);

        //PreparePreview(_previewObject);
        //PrepareCursor(size);
        //_cellIndicator.SetActive(true);

        //Canvas canvas = _previewObject.GetComponentInChildren<Canvas>();
        //if (canvas != null)
        //{
        //    canvas.gameObject.SetActive(false);
        //}

        //Collider[] colliders = _previewObject.GetComponentsInChildren<Collider>();
        //foreach (Collider col in colliders)
        //{
        //    col.enabled = false;
        //}

        //// 미리보기 오브젝트는 네트워크 오브젝트가 아니므로 NetworkIdentity 제거
        //if (_previewObject.TryGetComponent<NetworkIdentity>(out NetworkIdentity ni))
        //{
        //    Destroy(ni);
        //}
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            _cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            _cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        _previewObjectRenderers = previewObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in _previewObjectRenderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = _previewMaterial;
            }
            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        _cellIndicator.SetActive(false);
        if (ReferenceEquals(_previewObject, null) == false)
        {
            StructureFactory.Instance.ReturnObject(_previewObject);
        }
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        if (_previewObject != null)
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
