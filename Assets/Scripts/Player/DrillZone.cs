using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DrillZone : MonoBehaviour
{
    #region [Champs]

    private DrillController _Drill;
    private int _TerrainContactCount;

    #endregion


    #region [Propriétés]

    public bool IsInTerrain => _TerrainContactCount > 0;

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

        _TerrainContactCount++;
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

    private void OnTriggerExit2D(Collider2D pOther)
    {
        DrillableElement lElement = pOther.GetComponent<DrillableElement>();

        if (lElement == null)
        {
            return;
        }

        _TerrainContactCount = Mathf.Max(0, _TerrainContactCount - 1);
    }

    #endregion
}