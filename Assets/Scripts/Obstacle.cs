using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float minSize = 0.5f;
    public float maxSize = 2.0f;

    public float minSpeed = 100f;
    public float maxSpeed = 200f;

    public float maxSpinSpeed = 10f;

    public GameObject bounceEffect;

    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1);

        rb = GetComponent<Rigidbody2D>();

        float randomSpeed = Random.Range(minSpeed, maxSpeed) / randomSize;
        Vector2 randomDirection = Random.insideUnitCircle;
        rb.AddForce(randomDirection * randomSpeed);

        float randomTorque = Random.Range(-maxSpinSpeed, maxSpinSpeed);
        rb.AddTorque(randomTorque);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // This will print: "I am [ObjectName] and I just hit [OtherObjectName]"
        Debug.Log("I am " + gameObject.name + " and I just hit " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Obstacle"))
        {
            Vector2 contactPoint = collision.GetContact(0).point;
            GameObject bounceEffectPrefab = Instantiate(bounceEffect, contactPoint, Quaternion.identity);

            // Destroy the effect after 1 second
            Destroy(bounceEffectPrefab, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
