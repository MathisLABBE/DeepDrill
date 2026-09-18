using UnityEngine;

public class DrillableElement : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private TerrainElementType _Type = TerrainElementType.Normal;

    [SerializeField]
    private MineralType _MineralType = MineralType.Diamond;

    [SerializeField]
    private float _Resistance = 1f;

    [SerializeField]
    private int _MineralQuantity = 1;

    private float _CurrentDrilling;
    private bool _IsCollected;

    #endregion


    #region [Propriétés]

    public float Resistance => _Resistance;

    #endregion


    #region [Forage]

    public void Forer(DrillController pDrill, float pDrillPower, float pDeltaTime)
    {
        if (_Type == TerrainElementType.Indestructible)
        {
            return;
        }

        _CurrentDrilling += pDrillPower * pDeltaTime;

        if (_CurrentDrilling < _Resistance)
        {
            return;
        }

        if (_Type == TerrainElementType.Ore)
        {
            CollecterMinerai();

            if (pDrill != null)
            {
                pDrill.JouerSonDiamant();
                pDrill.JouerVibrationDiamant();
            }
        }

        if (pDrill != null)
        {
            pDrill.JouerEffetForage();
        }

        Destroy(gameObject);
    }

    public void DetruireParForageContinu(DrillController pDrill)
    {
        if (_Type == TerrainElementType.Indestructible)
        {
            return;
        }

        if (_Type == TerrainElementType.Ore)
        {
            CollecterMinerai();

            if (pDrill != null)
            {
                pDrill.JouerSonDiamant();
                pDrill.JouerVibrationDiamant();
            }
        }

        if (pDrill != null)
        {
            pDrill.JouerEffetForage();
        }

        Destroy(gameObject);
    }

    #endregion


    #region [Minerai]

    private void CollecterMinerai()
    {
        if (_IsCollected || RunManager.Instance == null)
        {
            return;
        }

        _IsCollected = true;

        RunManager.Instance.AjouterMinerai(_MineralType, _MineralQuantity);
    }

    public void CollecterParAimant(DrillController pDrill)
    {
        if (_Type != TerrainElementType.Ore || _IsCollected)
        {
            return;
        }

        CollecterMinerai();

        if (pDrill != null)
        {
            pDrill.JouerSonDiamant();
            pDrill.JouerVibrationDiamant();
        }

        Destroy(gameObject);
    }

    #endregion
}