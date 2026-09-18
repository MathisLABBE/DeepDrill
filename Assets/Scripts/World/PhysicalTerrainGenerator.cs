using UnityEngine;

public class PhysicalTerrainGenerator : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private GameObject _NormalElementPrefab;

    [SerializeField]
    private GameObject _DiamondElementPrefab;

    [SerializeField]
    private GameObject _GoldElementPrefab;

    [SerializeField]
    private GameObject _IndestructibleElementPrefab;

    [SerializeField]
    private GameObject _LavaPrefab;

    [SerializeField]
    private float _DiamondVeinScale = 0.18f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _DiamondVeinThreshold = 0.72f;

    private float _DiamondNoiseOffsetX;
    private float _DiamondNoiseOffsetY;

    [SerializeField]
    private float _GoldVeinScale = 0.16f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _GoldVeinThreshold = 0.78f;

    private float _GoldNoiseOffsetX;
    private float _GoldNoiseOffsetY;

    [SerializeField]
    [Range(0f, 1f)]
    private float _IndestructibleChance = 0.05f;

    [SerializeField]
    private float _LavaPocketScale = 0.12f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _LavaPocketThreshold = 0.78f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _LavaFillRatio = 0.5f;

    [SerializeField]
    [Min(1)]
    private int _LavaPocketVerticalSearch = 20;

    private float _LavaNoiseOffsetX;
    private float _LavaNoiseOffsetY;

    [SerializeField]
    private Transform _Drill;

    [SerializeField]
    private float _TerrainWidth = 6f;

    [SerializeField]
    private float _GenerationDepth = 10f;

    [SerializeField]
    private float _ElementSize = 0.25f;

    [SerializeField]
    private float _ReferenceElementSize = 0.25f;

    [SerializeField]
    private float _StartY = -2f;

    [SerializeField]
    private float _CleanupDistance = 8f;

    [SerializeField]
    private float _CleanupInterval = 0.5f;

    private float _NextRowY;
    private float _CleanupTimer;

    #endregion


    #region [Méthodes Unity]

    private void Start()
    {
        _DiamondNoiseOffsetX = Random.Range(-10000f, 10000f);
        _DiamondNoiseOffsetY = Random.Range(-10000f, 10000f);

        _GoldNoiseOffsetX = Random.Range(-10000f, 10000f);
        _GoldNoiseOffsetY = Random.Range(-10000f, 10000f);

        _LavaNoiseOffsetX = Random.Range(-10000f, 10000f);
        _LavaNoiseOffsetY = Random.Range(-10000f, 10000f);

        _NextRowY = _StartY;

        int lRows = Mathf.CeilToInt(_GenerationDepth / _ElementSize);

        for (int lRow = 0; lRow < lRows; lRow++)
        {
            GenererLigne();
        }
    }

    private void Update()
    {
        if (_Drill == null)
        {
            return;
        }

        GenererTerrain();
        MettreAJourNettoyage();
    }

    #endregion


    #region [Génération]

    private void GenererTerrain()
    {
        float lGenerationLimit = _Drill.position.y - _GenerationDepth;

        while (lGenerationLimit < _NextRowY)
        {
            GenererLigne();
        }
    }

    private void GenererLigne()
    {
        if (_NormalElementPrefab == null)
        {
            return;
        }

        int lColumns = Mathf.CeilToInt(_TerrainWidth / _ElementSize);
        float lTerrainWidth = lColumns * _ElementSize;
        float lStartX = -(lTerrainWidth / 2f) + (_ElementSize / 2f);

        for (int lColumn = 0; lColumn < lColumns; lColumn++)
        {
            float lX = lStartX + (lColumn * _ElementSize);
            Vector3 lPosition = new Vector3(lX, _NextRowY, 0f);

            TerrainElementType lType = ChoisirTypeElement(lPosition);
            GameObject lPrefab = ObtenirPrefab(lType, lPosition);

            CreerElement(lPrefab, lPosition);
        }

        _NextRowY -= _ElementSize;
    }

    private void CreerElement(GameObject pPrefab, Vector3 pPosition)
    {
        if (pPrefab == null || _ReferenceElementSize <= 0f)
        {
            return;
        }

        GameObject lElement = Instantiate(pPrefab, pPosition, Quaternion.identity, transform);

        float lScaleRatio = _ElementSize / _ReferenceElementSize;

        lElement.transform.localScale *= lScaleRatio;
    }

    private TerrainElementType ChoisirTypeElement(Vector3 pPosition)
    {
        float lGridX = pPosition.x / _ElementSize;
        float lGridY = pPosition.y / _ElementSize;

        float lLavaNoise = CalculerBruitPocheLave(pPosition);

        if (_LavaPrefab != null && lLavaNoise >= _LavaPocketThreshold)
        {
            return ChoisirTypePocheLave(pPosition);
        }

        if (_IndestructibleElementPrefab != null && Random.value < _IndestructibleChance)
        {
            return TerrainElementType.Indestructible;
        }

        float lDiamondNoise = Mathf.PerlinNoise(
            (lGridX * _DiamondVeinScale) + _DiamondNoiseOffsetX,
            (lGridY * _DiamondVeinScale) + _DiamondNoiseOffsetY);

        float lGoldNoise = Mathf.PerlinNoise(
            (lGridX * _GoldVeinScale) + _GoldNoiseOffsetX,
            (lGridY * _GoldVeinScale) + _GoldNoiseOffsetY);

        bool lIsDiamond = _DiamondElementPrefab != null && lDiamondNoise >= _DiamondVeinThreshold;
        bool lIsGold = _GoldElementPrefab != null && lGoldNoise >= _GoldVeinThreshold;

        if (lIsDiamond || lIsGold)
        {
            return TerrainElementType.Ore;
        }

        return TerrainElementType.Normal;
    }

    private GameObject ObtenirPrefab(TerrainElementType pType, Vector3 pPosition)
    {
        switch (pType)
        {
            case TerrainElementType.Ore:
                return ObtenirPrefabMinerai(pPosition);

            case TerrainElementType.Indestructible:
                return _IndestructibleElementPrefab;

            case TerrainElementType.Lava:
                return _LavaPrefab;

            case TerrainElementType.Empty:
                return null;

            default:
                return _NormalElementPrefab;
        }
    }

    private GameObject ObtenirPrefabMinerai(Vector3 pPosition)
    {
        float lGridX = pPosition.x / _ElementSize;
        float lGridY = pPosition.y / _ElementSize;

        float lGoldNoise = Mathf.PerlinNoise(
            (lGridX * _GoldVeinScale) + _GoldNoiseOffsetX,
            (lGridY * _GoldVeinScale) + _GoldNoiseOffsetY);

        if (_GoldElementPrefab != null && lGoldNoise >= _GoldVeinThreshold)
        {
            return _GoldElementPrefab;
        }

        return _DiamondElementPrefab;
    }

    private float CalculerBruitPocheLave(Vector3 pPosition)
    {
        float lGridX = pPosition.x / _ElementSize;
        float lGridY = pPosition.y / _ElementSize;

        return Mathf.PerlinNoise(
            (lGridX * _LavaPocketScale) + _LavaNoiseOffsetX,
            (lGridY * _LavaPocketScale) + _LavaNoiseOffsetY);
    }

    private TerrainElementType ChoisirTypePocheLave(Vector3 pPosition)
    {
        int lTopSteps = 0;
        int lBottomSteps = 0;

        for (int lStep = 1; lStep <= _LavaPocketVerticalSearch; lStep++)
        {
            Vector3 lPosition = pPosition + (Vector3.up * (_ElementSize * lStep));

            if (CalculerBruitPocheLave(lPosition) < _LavaPocketThreshold)
            {
                break;
            }

            lTopSteps = lStep;
        }

        for (int lStep = 1; lStep <= _LavaPocketVerticalSearch; lStep++)
        {
            Vector3 lPosition = pPosition + (Vector3.down * (_ElementSize * lStep));

            if (CalculerBruitPocheLave(lPosition) < _LavaPocketThreshold)
            {
                break;
            }

            lBottomSteps = lStep;
        }

        float lTopY = pPosition.y + (lTopSteps * _ElementSize);
        float lBottomY = pPosition.y - (lBottomSteps * _ElementSize);

        float lLavaSurfaceY = Mathf.Lerp(lBottomY, lTopY, Mathf.Clamp01(_LavaFillRatio));

        if (pPosition.y <= lLavaSurfaceY)
        {
            return TerrainElementType.Lava;
        }

        return TerrainElementType.Empty;
    }

    #endregion


    #region [Nettoyage]

    private void MettreAJourNettoyage()
    {
        _CleanupTimer += Time.deltaTime;

        if (_CleanupTimer < _CleanupInterval)
        {
            return;
        }

        _CleanupTimer = 0f;

        NettoyerTerrain();
    }

    private void NettoyerTerrain()
    {
        float lCleanupLimit = _Drill.position.y + _CleanupDistance;

        for (int lIndex = transform.childCount - 1; lIndex >= 0; lIndex--)
        {
            Transform lElement = transform.GetChild(lIndex);

            if (lElement.position.y <= lCleanupLimit)
            {
                continue;
            }

            Destroy(lElement.gameObject);
        }
    }

    #endregion
}