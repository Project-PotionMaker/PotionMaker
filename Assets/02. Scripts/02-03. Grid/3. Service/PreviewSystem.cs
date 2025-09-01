using Mirror;
using NUnit.Framework;
using System.Collections.Generic;
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
    private Transform _modelTransform;
    [SerializeField]
    private List<GameObject> _previewModelList;
    private Renderer[] _previewObjectRenderers;

    private GameObject _previewGameObject;

    [SerializeField]
    private Material _previewMaterial;
    private MaterialPropertyBlock _previewMaterialPropertyBlock;

    private void Awake()
    {
        _previewMaterialPropertyBlock = new MaterialPropertyBlock();
    }

    public void StartShowingPlacementPreview(int tid)
    {
        StructureData data = DataTable.Instance.GetStructureData(tid);

        for(int i = 0; i < _previewModelList.Count; i++)
        {
            _previewModelList[i].SetActive(false);
        }

        PreparePreview(_previewModelList[data.TID - 10000]);
    }

    private void PreparePreview(GameObject previewObject)
    {
        _previewGameObject = previewObject;
        _previewGameObject.SetActive(true);

        _previewObjectRenderers = _previewGameObject.GetComponentsInChildren<MeshRenderer>();
    }

    public void StopShowingPreview()
    {
        if (_previewGameObject != null)
        {
            _previewGameObject.SetActive(false);
            _previewGameObject = null;
        }
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        if (_previewGameObject != null)
        {
            MovePreview(position);
            ApplyFeedbackToPreview(validity);
        }
    }

    private void MovePreview(Vector3 position)
    {
        _modelTransform.transform.position = new Vector3(position.x, position.y + _previewYOffset, position.z);
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
}
