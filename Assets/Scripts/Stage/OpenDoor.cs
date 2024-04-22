using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public List<GameObject> gameObjects;
    public GameObject Door;
    private List<GameObject> previousGameObjects = new List<GameObject>();

    [SerializeField] private int targetKill;
    [SerializeField] private int killCount = 0;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        previousGameObjects = new List<GameObject>(gameObjects);

        // gameObjects 리스트 내에 null이 있는지 확인하고 제거
        gameObjects = gameObjects.Where(item => item != null).ToList();

        bool listChanged = !Enumerable.SequenceEqual(gameObjects, previousGameObjects);

        foreach (var obj in new List<GameObject>(gameObjects))
        {
            RemoveAndCheckEmpty(obj);
        }

        if (gameObjects.Count == 0)
        {
            Destroy(Door);
        }
    }
    private void RemoveAndCheckEmpty(GameObject obj)
    {

       // killCount++;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            gameObjects.Add(other.gameObject); // Enemy 오브젝트를 리스트에 추가합니다.
        }
    }
}
