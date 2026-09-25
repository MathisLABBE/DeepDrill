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

    [SerializeField]
    private TunnelMaskManager _TunnelMaskManager;

    private TerrainProbe _TerrainProbe;
    private Vector3 _LastMaskPosition;
    private bool _HasLastMaskPosition;

    #endregion


    #region [Méthodes Unity]

    private void Awake()
    {
        _TerrainProbe = GetComponentInChildren<TerrainProbe>();

        if (_TunnelMaskManager == null)
        {
            _TunnelMaskManager = FindFirstObjectByType<TunnelMaskManager>();
        }
    }

    private void Update()
    {
        if (_TerrainProbe == null || !_TerrainProbe.IsInTerrain)
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

        GameObject lMask = Instantiate(
            _TunnelMaskPrefab,
            transform.position,
            Quaternion.identity,
            _TunnelMasksParent);

        if (_TunnelMaskManager != null)
        {
            SpriteMask lSpriteMask = lMask.GetComponent<SpriteMask>();

            if (lSpriteMask != null && lSpriteMask.sprite != null)
            {
                float lScale = Mathf.Max(
                    Mathf.Abs(lMask.transform.lossyScale.x),
                    Mathf.Abs(lMask.transform.lossyScale.y));

                float lRadius = lSpriteMask.sprite.bounds.extents.x * lScale;

                _TunnelMaskManager.EnregistrerMasque(
                    lMask.transform.position,
                    lRadius);
            }
        }

        _LastMaskPosition = transform.position;
        _HasLastMaskPosition = true;
    }

    #endregion
}