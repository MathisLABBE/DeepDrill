using System.Collections.Generic;
using UnityEngine;

public class TunnelMaskManager : MonoBehaviour
{
    #region [Champs]

    [SerializeField]
    [Min(1)]
    private int _RecentMaskCount = 20;

    [SerializeField]
    [Min(0f)]
    private float _TerrainSearchMargin = 0.25f;

    private readonly Queue<MaskData> _RecentMasks = new();

    private DrillController _Drill;

    #endregion


    #region [Méthodes Unity]

    private void Awake()
    {
        _Drill = FindFirstObjectByType<DrillController>();
    }

    #endregion


    #region [Tunnel]

    public void EnregistrerMasque(Vector2 pPosition, float pRadius)
    {
        _RecentMasks.Enqueue(new MaskData(pPosition, pRadius));

        while (_RecentMasks.Count > _RecentMaskCount)
        {
            _RecentMasks.Dequeue();
        }

        NettoyerTerrainAutour(pPosition, pRadius);
    }

    private void NettoyerTerrainAutour(Vector2 pPosition, float pRadius)
    {
        Collider2D[] lColliders = Physics2D.OverlapCircleAll(
            pPosition,
            pRadius + _TerrainSearchMargin);

        foreach (Collider2D lCollider in lColliders)
        {
            DrillableElement lElement = lCollider.GetComponent<DrillableElement>();

            if (lElement == null)
            {
                continue;
            }

            if (!EstEntierementMasque(lCollider))
            {
                continue;
            }

            lElement.DetruireParForageContinu(_Drill);
        }
    }

    private bool EstEntierementMasque(Collider2D pCollider)
    {
        Bounds lBounds = pCollider.bounds;

        Vector2[] lPoints =
        {
            new Vector2(lBounds.min.x, lBounds.min.y),
            new Vector2(lBounds.max.x, lBounds.min.y),
            new Vector2(lBounds.min.x, lBounds.max.y),
            new Vector2(lBounds.max.x, lBounds.max.y),

            new Vector2(lBounds.center.x, lBounds.min.y),
            new Vector2(lBounds.center.x, lBounds.max.y),
            new Vector2(lBounds.min.x, lBounds.center.y),
            new Vector2(lBounds.max.x, lBounds.center.y),

            lBounds.center
        };

        foreach (Vector2 lPoint in lPoints)
        {
            if (!EstPointDansTunnel(lPoint))
            {
                return false;
            }
        }

        return true;
    }

    private bool EstPointDansTunnel(Vector2 pPoint)
    {
        foreach (MaskData lMask in _RecentMasks)
        {
            float lRadiusSquared = lMask.Radius * lMask.Radius;
            float lDistanceSquared = (pPoint - lMask.Position).sqrMagnitude;

            if (lDistanceSquared <= lRadiusSquared)
            {
                return true;
            }
        }

        return false;
    }

    #endregion


    #region [Données]

    private readonly struct MaskData
    {
        public Vector2 Position { get; }

        public float Radius { get; }

        public MaskData(Vector2 pPosition, float pRadius)
        {
            Position = pPosition;
            Radius = pRadius;
        }
    }

    #endregion
}