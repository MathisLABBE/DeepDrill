using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LavaHazard : MonoBehaviour
{
    #region [Méthodes Unity]

    private void OnTriggerEnter2D(Collider2D pOther)
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        DrillHead lDrillHead = pOther.GetComponent<DrillHead>();

        if (lDrillHead != null)
        {
            GameManager.Instance.TerminerPartie();
            return;
        }

        if (pOther.isTrigger)
        {
            return;
        }

        DrillController lDrill = pOther.GetComponentInParent<DrillController>();

        if (lDrill == null)
        {
            return;
        }

        GameManager.Instance.TerminerPartie();
    }

    #endregion
}