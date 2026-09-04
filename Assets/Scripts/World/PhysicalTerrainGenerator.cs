using UnityEngine;

public class PhysicalTerrainGenerator : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private GameObject _DrillableElementPrefab;

    [SerializeField]
    private int _Columns = 24;

    [SerializeField]
    private int _Rows = 40;

    [SerializeField]
    private float _ElementSize = 0.25f;

    [SerializeField]
    private float _StartY = -2f;

    #endregion


    #region [Méthodes Unity]

    private void Start()
    {
        GenererTerrain();
    }

    #endregion


    #region [Génération]

    private void GenererTerrain()
    {
        if (_DrillableElementPrefab == null)
        {
            return;
        }

        float lTerrainWidth = _Columns * _ElementSize;
        float lStartX = -(lTerrainWidth / 2f) + (_ElementSize / 2f);

        for (int lRow = 0; lRow < _Rows; lRow++)
        {
            for (int lColumn = 0; lColumn < _Columns; lColumn++)
            {
                float lX = lStartX + (lColumn * _ElementSize);
                float lY = _StartY - (lRow * _ElementSize);

                Vector3 lPosition = new Vector3(lX, lY, 0f);

                Instantiate(_DrillableElementPrefab, lPosition, Quaternion.identity, transform);
            }
        }
    }

    #endregion
}