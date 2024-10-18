using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float moveSpeed = 100f;
    public float rotationSpeed = 100f;

    void Start()
    {
        StartCoroutine(Wander());
    }

    IEnumerator Wander()
    {
        while (true)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            yield return new WaitForSeconds(Random.Range(1f, 3f));
            float randomRotation = Random.Range(-rotationSpeed, rotationSpeed);
            transform.Rotate(Vector3.up, randomRotation);
        }
    }
}
    