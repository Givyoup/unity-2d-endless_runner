using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundLoop : MonoBehaviour
{
    public Transform otherGround;   // referensi ground lain
    private float length;           // panjang tilemap

    void Start()
    {
        // ambil panjang dari tilemap
        var tilemap = GetComponent<Tilemap>();
        length = tilemap.localBounds.size.x;
    }

    void Update()
    {
        float moveX = GameManager.Instance.gameSpeed * Time.deltaTime;
        transform.position -= new Vector3(moveX, 0, 0);

        // kalau ground sudah kelewat kiri, geser ke kanan setelah ground lain
        if (transform.position.x < otherGround.position.x - length)
        {
            transform.position = new Vector3(
                otherGround.position.x + length, 
                transform.position.y, 
                transform.position.z
            );
        }
    }
}
