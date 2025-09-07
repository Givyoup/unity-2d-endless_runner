using UnityEngine;

public class ParallaxLoop2D : MonoBehaviour
{
    public float parallaxMultiplier = 0.5f;

    private float length;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        // panjang sprite (lebar dalam world unit)
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        // gerakkan background
        float moveX = GameManager.Instance.gameSpeed * parallaxMultiplier * Time.deltaTime;
        transform.position -= new Vector3(moveX, 0, 0);

        // kalau sprite sudah jauh di kiri → geser ke kanan lagi
        if (cam.transform.position.x - transform.position.x >= length)
        {
            transform.position += new Vector3(length * 2f , 0, 0);

            // pembulatan biar gak ada floating point error
            float newX = Mathf.Round(transform.position.x * 100f) / 100f;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }
}
