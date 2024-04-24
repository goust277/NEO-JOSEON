using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileQueue : MonoBehaviour
{
    private List<Projectile> projectiles = new List<Projectile>();
    [SerializeField] private int initialCount = 10; // 최초 초기화 시 생성할 발사체 수
    [SerializeField] private GameObject projectilePrefab = null;

    private void Awake()
    {
        if (projectilePrefab == null) return;
        for (int i = 0; i < initialCount; i++)
            CreateNewProjectile();
    }

    /// <summary>
    /// 사용 가능한 발사체를 반환합니다.
    /// </summary>
    /// <returns></returns>
    public Projectile GetProjectile()
    {
        foreach (Projectile ele in projectiles)
        {
            if (!ele.IsAvailable()) continue; // 이미 사용중이면 건너뜀
            ele.SetCallbackHit(null);
            return ele;
        }
        return CreateNewProjectile(); // 모두 사용 중이면 새로 하나 만듦
    }

    // 발사체를 새로 만들어 반환합니다.
    private Projectile CreateNewProjectile()
    {
        GameObject inst = Instantiate(projectilePrefab, transform);
        inst.SetActive(false);
        Projectile p = inst.GetComponent<Projectile>();
        projectiles.Add(p);
        return p;
    }
}
