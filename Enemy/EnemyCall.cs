using UnityEngine;

public class EnemyCall : MonoBehaviour {

    private Enemy enemy;

    private void Awake()
    {
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
    }

    public void CallEnemy()
    {
        enemy.CallEnemy(transform.position);
    }
}
