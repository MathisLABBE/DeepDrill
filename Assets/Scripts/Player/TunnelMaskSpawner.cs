using UnityEngine;

public class TunnelMaskSpawner : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private GameObject _TunnelMaskPrefab;

    [SerializeField]
    private Transform _TunnelMasksParent;

    [SerializeField]
    [Min(0.01f)]
    private float _MaskSpacing = 0.1f;

    private DrillZone _DrillZone;
    private Vector3 _LastMaskPosition;
    private bool _HasLastMaskPosition;

    #endregion


    #region [Méthodes Unity]

    private void Awake()
    {
        _DrillZone = GetComponentInParent<DrillController>()?.GetComponentInChildren<DrillZone>();
    }

    private void Update()
    {
        if (_DrillZone == null || !_DrillZone.IsInTerrain)
        {
            _HasLastMaskPosition = false;
            return;
        }

        if (!_HasLastMaskPosition)
        {
            CreerMasque();
            return;
        }

        float lDistance = Vector2.Distance(transform.position, _LastMaskPosition);

        if (lDistance < _MaskSpacing)
        {
            return;
        }

        CreerMasque();
    }

    #endregion


    #region [Tunnel]

    private void CreerMasque()
    {
        if (_TunnelMaskPrefab == null)
        {
            return;
        }

        Instantiate(
            _TunnelMaskPrefab,
            transform.position,
            Quaternion.identity,
            _TunnelMasksParent);

        _LastMaskPosition = transform.position;
        _HasLastMaskPosition = true;
    }

    #endregion
}