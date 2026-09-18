using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DrillHead : MonoBehaviour
{
    #region [Champs]

    private DrillController _Drill;

    #endregion


    #region [Méthodes Unity]

    private void Awake()
    {
        _Drill = GetComponentInParent<DrillController>();
    }

    private void OnTriggerEnter2D(Collider2D pOther)
    {
        DrillableElement lElement = pOther.GetComponent<DrillableElement>();

        if (lElement == null)
        {
            return;
        }

        lElement.DetruireParForageContinu(_Drill);
    }

    #endregion
}