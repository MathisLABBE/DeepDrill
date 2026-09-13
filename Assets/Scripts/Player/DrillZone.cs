using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DrillZone : MonoBehaviour
{
    #region [Champs]

    private DrillController _Drill;

    #endregion


    #region [Méthodes Unity]

    private void Awake()
    {
        _Drill = GetComponentInParent<DrillController>();
    }

    private void OnTriggerStay2D(Collider2D pOther)
    {
        if (_Drill == null)
        {
            return;
        }

        DrillableElement lElement = pOther.GetComponent<DrillableElement>();

        if (lElement == null)
        {
            return;
        }

        _Drill.ForerElement(lElement, Time.fixedDeltaTime);
    }

    #endregion
}