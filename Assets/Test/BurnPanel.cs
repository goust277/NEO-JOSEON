using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnPanel : MonoBehaviour, IDamageable
{
    [SerializeField] private List<Material> heatMaterial;
    MeshRenderer meshRenderer;
    private GameObject fireWall;

    private int heat = 0;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        fireWall = transform.GetChild(0).gameObject;
    }

    public void TakeDamage(Damage damage)
    {
        if (heat == 3) return;
        heat = (heat + 1) % 4;
        meshRenderer.material = heatMaterial[heat];
        if (heat == 3)
        {
            Invoke("Explode", 1);
            return;
        }
    }

    public void Explode()
    {
        fireWall.SetActive(true);
        Invoke("Explode2", 0.5f);
    }
    public void Explode2()
    {
        fireWall.SetActive(false);
        heat = 0;
        meshRenderer.material = heatMaterial[heat];
    }
}
