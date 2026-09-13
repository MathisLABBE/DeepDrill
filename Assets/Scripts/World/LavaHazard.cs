using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LavaHazard : MonoBehaviour
{
    #region [Méthodes Unity]

    private void OnTriggerEnter2D(Collider2D pOther)
    {
        if (pOther.isTrigger)
        {
            return;
        }

        DrillController lDrill = pOther.GetComponentInParent<DrillController>();

        if (lDrill == null || GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.TerminerPartie();
    }

    #endregion
}