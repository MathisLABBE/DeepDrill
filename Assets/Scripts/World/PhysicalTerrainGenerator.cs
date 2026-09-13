using UnityEngine;

public class PhysicalTerrainGenerator : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private GameObject _NormalElementPrefab;

    [SerializeField]
    private GameObject _OreElementPrefab;

    [SerializeField]
    private GameObject _IndestructibleElementPrefab;

    [SerializeField]
    private GameObject _LavaPrefab;

    [SerializeField]
    [Range(0f, 1f)]
    private float _OreChance = 0.15f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _IndestructibleChance = 0.05f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _LavaChance = 0.02f;

    [SerializeField]
    private Transform _Drill;

    [SerializeField]
    private int _Columns = 24;

    [SerializeField]
    private int _RowsAhead = 40;

    [SerializeField]
    private float _ElementSize = 0.25f;

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
        _NextRowY = _StartY;

        for (int lRow = 0; lRow < _RowsAhead; lRow++)
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
        float lGenerationLimit = _Drill.position.y - (_RowsAhead * _ElementSize);

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

        float lTerrainWidth = _Columns * _ElementSize;
        float lStartX = -(lTerrainWidth / 2f) + (_ElementSize / 2f);

        for (int lColumn = 0; lColumn < _Columns; lColumn++)
        {
            float lX = lStartX + (lColumn * _ElementSize);
            Vector3 lPosition = new Vector3(lX, _NextRowY, 0f);

            GameObject lPrefab = ChoisirElement();

            Instantiate(lPrefab, lPosition, Quaternion.identity, transform);
        }

        _NextRowY -= _ElementSize;
    }

    private GameObject ChoisirElement()
    {
        float lRandom = Random.value;

        if (_LavaPrefab != null &&
            lRandom < _LavaChance)
        {
            return _LavaPrefab;
        }

        if (_IndestructibleElementPrefab != null &&
            lRandom < _LavaChance + _IndestructibleChance)
        {
            return _IndestructibleElementPrefab;
        }

        if (_OreElementPrefab != null &&
            lRandom < _LavaChance + _IndestructibleChance + _OreChance)
        {
            return _OreElementPrefab;
        }

        return _NormalElementPrefab;
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