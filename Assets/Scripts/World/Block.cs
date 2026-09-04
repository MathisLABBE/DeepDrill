using UnityEngine;

public class Block : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    private BlockType _Type;

    [SerializeField]
    private int _OreValue = 1;

    private bool _IsCollected;

    #endregion


    #region [Méthodes Unity]

    private void OnTriggerEnter2D(Collider2D pCollision)
    {
        DrillController lDrill = pCollision.GetComponent<DrillController>();

        if (lDrill == null)
        {
            return;
        }

        if (_Type == BlockType.Indestructible)
        {
            GameManager.Instance.TerminerPartie();
            return;
        }

        if (_Type == BlockType.Ore)
        {
            CollecterMinerai();
            lDrill.JouerSonDiamant();
            lDrill.JouerVibrationDiamant();
        }

        lDrill.JouerEffetForage();

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