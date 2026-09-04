using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private GameObject _NormalBlockPrefab;

    [SerializeField]
    private GameObject _OreBlockPrefab;

    [SerializeField]
    private GameObject _IndestructibleBlockPrefab;

    [SerializeField]
    private Transform _Drill;

    [SerializeField]
    private int _Columns = 6;

    [SerializeField]
    private int _RowsAhead = 25;

    [SerializeField]
    private float _BlockSize = 1f;

    [SerializeField]
    private float _StartY = -3f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _OreChance = 0.15f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _IndestructibleChance = 0.10f;

    [SerializeField]
    private float _CleanupDistance = 12f;

    [SerializeField]
    private float _CleanupInterval = 1f;

    [SerializeField]
    private float _IndestructibleChancePer100Meters = 0.15f;

    [SerializeField]
    private float _MaxIndestructibleChance = 0.35f;

    private int _SafeColumn;
    private int _TargetSafeColumn;
    private int _TransitionRowsRemaining;

    private float _NextRowY;
    private float _CleanupTimer;

    #endregion


    #region [Méthodes Unity]

    private void Start()
    {
        _NextRowY = _StartY;

        _SafeColumn = Random.Range(0, _Columns);
        _TargetSafeColumn = _SafeColumn;

        for (int lRow = 0; lRow < _RowsAhead; lRow++)
        {
            GenererLigne();
        }
    }

    private void Update()
    {
        GenererMine();
        NettoyerBlocs();
    }

    #endregion


    #region [Génération]

    private void GenererMine()
    {
        float lLimiteGeneration = _Drill.position.y - (_RowsAhead * _BlockSize);

        while (_NextRowY > lLimiteGeneration)
        {
            GenererLigne();
        }
    }

    private void GenererLigne()
    {
        float lStartX = -((_Columns - 1) * _BlockSize) / 2f;

        if (_TransitionRowsRemaining <= 0)
        {
            int lDirection = Random.Range(-1, 2);

            _TargetSafeColumn = _SafeColumn + lDirection;
            _TargetSafeColumn = Mathf.Clamp(_TargetSafeColumn, 0, _Columns - 1);

            if (_TargetSafeColumn != _SafeColumn)
            {
                _TransitionRowsRemaining = 3;
            }
        }

        for (int lColumn = 0; lColumn < _Columns; lColumn++)
        {
            float lPositionX = lStartX + (lColumn * _BlockSize);
            Vector3 lPosition = new Vector3(lPositionX, _NextRowY, 0f);

            GameObject lPrefab;

            bool lIsSafeColumn = lColumn == _SafeColumn;

            if (_TransitionRowsRemaining > 0)
            {
                lIsSafeColumn |= lColumn == _TargetSafeColumn;
            }

            if (lIsSafeColumn)
            {
                lPrefab = ChoisirPrefabSecurise();
            }
            else
            {
                lPrefab = ChoisirPrefab();
            }

            Instantiate(lPrefab, lPosition, Quaternion.identity, transform);
        }

        if (_TransitionRowsRemaining > 0)
        {
            _TransitionRowsRemaining--;

            if (_TransitionRowsRemaining == 0)
            {
                _SafeColumn = _TargetSafeColumn;
            }
        }

        _NextRowY -= _BlockSize;
    }

    private GameObject ChoisirPrefabSecurise()
    {
        if (Random.value < _OreChance)
        {
            return _OreBlockPrefab;
        }

        return _NormalBlockPrefab;
    }

    private GameObject ChoisirPrefab()
    {
        float lIndestructibleChance = CalculerChanceIndestructible();
        float lValeurAleatoire = Random.value;

        if (lValeurAleatoire < lIndestructibleChance)
        {
            return _IndestructibleBlockPrefab;
        }

        if (lValeurAleatoire < lIndestructibleChance + _OreChance)
        {
            return _OreBlockPrefab;
        }

        return _NormalBlockPrefab;
    }

    private float CalculerChanceIndestructible()
    {
        float lProfondeur = Mathf.Max(0f, _StartY - _NextRowY);
        float lBonus = (lProfondeur / 100f) * _IndestructibleChancePer100Meters;

        return Mathf.Min(_IndestructibleChance + lBonus, _MaxIndestructibleChance);
    }

    #endregion


    #region [Nettoyage]

    private void NettoyerBlocs()
    {
        _CleanupTimer += Time.deltaTime;

        if (_CleanupTimer < _CleanupInterval)
        {
            return;
        }

        _CleanupTimer = 0f;

        for (int lIndex = transform.childCount - 1; lIndex >= 0; lIndex--)
        {
            Transform lBloc = transform.GetChild(lIndex);

            if (lBloc.position.y > _Drill.position.y + _CleanupDistance)
            {
                Destroy(lBloc.gameObject);
            }
        }
    }

    #endregion
}