using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float timeToTarget;
    float speed;
    public float assignedAngle;

    private void Start()
    {
        speed = 2f;
    }
    private void Update()
    {
        Vector3 newPosition = Vector3.MoveTowards(transform.position, Player.noteDespawn, speed * Time.deltaTime);
        transform.position = newPosition;

        if (Vector2.Distance(Player.noteDespawn, transform.position) < 0.001f)
        {
            Destroy(gameObject);
        }
    }
}
