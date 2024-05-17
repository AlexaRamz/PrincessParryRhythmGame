using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] private GameObject destroyParticlesPrefab;

    private void Update()
    {
        Vector3 newPosition = Vector3.MoveTowards(transform.position, Player.noteDespawn, Player.Instance.noteSpeed * Time.deltaTime);
        transform.position = newPosition;

        if (Vector2.Distance(Player.noteDespawn, transform.position) < 0.001f)
        {
            Destroy(gameObject);
        }
    }
    public void Kill()
    {
        Instantiate(destroyParticlesPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
