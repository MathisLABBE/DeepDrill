using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TunnelCarver : MonoBehaviour
{
    #region [Champs]

    private DrillController _Drill;
    private Collider2D _CarverCollider;

    #endregion


    #region [Méthodes Unity]

    private void Awake()
    {
        _Drill = GetComponentInParent<DrillController>();
        _CarverCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerStay2D(Collider2D pOther)
    {
        DrillableElement lElement = pOther.GetComponent<DrillableElement>();

        if (lElement == null)
        {
            return;
        }

        if (!EstEntierementDansZone(pOther))
        {
            return;
        }

        lElement.DetruireParForageContinu(_Drill);
    }

    #endregion


    #region [Terrain]

    private bool EstEntierementDansZone(Collider2D pElementCollider)
    {
        Bounds lBounds = pElementCollider.bounds;

        Vector2 lBasGauche = new Vector2(lBounds.min.x, lBounds.min.y);
        Vector2 lBasDroite = new Vector2(lBounds.max.x, lBounds.min.y);
        Vector2 lHautGauche = new Vector2(lBounds.min.x, lBounds.max.y);
        Vector2 lHautDroite = new Vector2(lBounds.max.x, lBounds.max.y);

        return EstPointDansZone(lBasGauche)
            && EstPointDansZone(lBasDroite)
            && EstPointDansZone(lHautGauche)
            && EstPointDansZone(lHautDroite);
    }

    private bool EstPointDansZone(Vector2 pPoint)
    {
        Vector2 lClosestPoint = _CarverCollider.ClosestPoint(pPoint);

        return Vector2.Distance(lClosestPoint, pPoint) < 0.001f;
    }

    #endregion
}