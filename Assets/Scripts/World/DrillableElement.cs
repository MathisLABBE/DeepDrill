using UnityEngine;

public class DrillableElement : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private BlockType _Type = BlockType.Normal;

    [SerializeField]
    private float _Resistance = 1f;

    [SerializeField]
    private int _OreValue = 1;

    private float _CurrentDrilling;
    private bool _IsCollected;

    #endregion


    #region [Forage]

    public void Forer(DrillController pDrill, float pDrillPower, float pDeltaTime)
    {
        if (_Type == BlockType.Indestructible)
        {
            return;
        }

        _CurrentDrilling += pDrillPower * pDeltaTime;

        if (_CurrentDrilling < _Resistance)
        {
            return;
        }

        if (_Type == BlockType.Ore)
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

        RunManager.Instance.AjouterMinerai(_OreValue);
    }

    public void CollecterParAimant(DrillController pDrill)
    {
        if (_Type != BlockType.Ore || _IsCollected)
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