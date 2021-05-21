using UnityEngine;

public class SlicedFruit : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField][Range(.95f, .99f)] float zSpeed = .98f;
    [SerializeField] Rigidbody2D[] bubblePieces = default;

    Transform moveTarget;
    Rigidbody2D rb;

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        moveTarget = GameManager.Instance.SpiritsMoveTarget.transform;

        for (int i = 0; i < bubblePieces.Length; i++)
        {
            bubblePieces[i].transform.SetParent(null);
            bubblePieces[i].AddForce(new Vector2(Random.Range(-2000f, 2000f), 1f));
            bubblePieces[i].AddTorque(Random.Range(-200f, 200f));
            Destroy(bubblePieces[i].gameObject, 3f);
        }
    }

    private void FixedUpdate()
    {
        Vector3 moveDir = moveTarget.position - this.transform.position;
        rb.velocity = moveDir * speed;
        this.transform.localScale *= zSpeed;
    }
}
