using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnPanel : MonoBehaviour, IDamageable
{
    [SerializeField] private List<Material> heatMaterial;
    MeshRenderer meshRenderer;

    private int heat = 0;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void TakeDamage(Damage damage)
    {
        if (!damage.property.Contains("FIRE")) return;
        heat = (heat + 1) % 3;
        meshRenderer.material = heatMaterial[heat];
    }
}
