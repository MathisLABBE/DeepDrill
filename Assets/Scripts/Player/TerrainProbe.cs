using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TerrainProbe : MonoBehaviour
{
    #region [Champs]

    private readonly HashSet<DrillableElement> _TerrainContacts = new();

    #endregion


    #region [Propriétés]

    public bool IsInTerrain => TerrainResistance > 0f;

    public float TerrainResistance => ObtenirResistanceTerrain();

    #endregion


    #region [Méthodes Unity]

    private void OnTriggerEnter2D(Collider2D pOther)
    {
        DrillableElement lElement = pOther.GetComponent<DrillableElement>();

        if (lElement == null)
        {
            return;
        }

        _TerrainContacts.Add(lElement);
    }

    private void OnTriggerExit2D(Collider2D pOther)
    {
        DrillableElement lElement = pOther.GetComponent<DrillableElement>();

        if (lElement == null)
        {
            return;
        }

        _TerrainContacts.Remove(lElement);
    }

    #endregion


    #region [Terrain]

    private float ObtenirResistanceTerrain()
    {
        float lResistance = 0f;

        foreach (DrillableElement lElement in _TerrainContacts)
        {
            if (lElement == null)
            {
                continue;
            }

            lResistance = Mathf.Max(lResistance, lElement.Resistance);
        }

        return lResistance;
    }

    #endregion
}