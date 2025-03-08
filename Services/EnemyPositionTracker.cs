using UnityEngine;

public class EnemyPositionTracker : MonoBehaviour
{
    public Enemy enemy;
    public float mapScale;
    public Transform mapParent;

    void Update()
    {
        if (enemy == null || !enemy.gameObject.activeInHierarchy)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = enemy.transform.position * mapScale + mapParent.position;
        transform.localPosition = new Vector3(transform.localPosition.x, 0f, transform.localPosition.z);
    }
}