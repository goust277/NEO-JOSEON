using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileQueue : MonoBehaviour
{
    private List<Projectile> projectiles = new List<Projectile>();
    [SerializeField] private int initialCount = 10; // ���� �ʱ�ȭ �� ������ �߻�ü ��
    [SerializeField] private GameObject projectilePrefab = null;

    private void Awake()
    {
        if (projectilePrefab == null) return;
        for (int i = 0; i < initialCount; i++)
            CreateNewProjectile();
    }

    /// <summary>
    /// ��� ������ �߻�ü�� ��ȯ�մϴ�.
    /// </summary>
    /// <returns></returns>
    public Projectile GetProjectile()
    {
        foreach (Projectile ele in projectiles)
        {
            if (!ele.IsAvailable()) continue; // �̹� ������̸� �ǳʶ�
            ele.SetCallbackHit(null);
            return ele;
        }
        return CreateNewProjectile(); // ��� ��� ���̸� ���� �ϳ� ����
    }

    // �߻�ü�� ���� ����� ��ȯ�մϴ�.
    private Projectile CreateNewProjectile()
    {
        GameObject inst = Instantiate(projectilePrefab, transform);
        inst.SetActive(false);
        Projectile p = inst.GetComponent<Projectile>();
        projectiles.Add(p);
        return p;
    }
}
