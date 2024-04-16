using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MuzzleFlash : MonoBehaviour
{
    // Play all VisualEffect components in children
    private void PlayAllVFX(Transform parentTransform)
    {
        foreach (Transform child in parentTransform)
        {
            VisualEffect vfx = child.GetComponent<VisualEffect>();
            if (vfx != null)
            {
                vfx.Play();
            }
            //PlayAllVFX(child);
        }
    }

    public void PlayVFX()
    {
        PlayAllVFX(transform);
    }
}
