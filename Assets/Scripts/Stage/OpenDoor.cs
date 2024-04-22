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

        // gameObjects ����Ʈ ���� null�� �ִ��� Ȯ���ϰ� ����
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
            gameObjects.Add(other.gameObject); // Enemy ������Ʈ�� ����Ʈ�� �߰��մϴ�.
        }
    }
}
