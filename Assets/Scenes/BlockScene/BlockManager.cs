using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField, Min(0)] private float width = 10f;

    // Start is called before the first frame update
    void Start()
    {
        this.camera = Camera.main;
    } 

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    private void OnDrawGizmos() {
        var pos = this.camera.transform.position;
        Vector2 centerPosition = new Vector2(pos.x, pos.y);
        Vector2 initialPosition = centerPosition + Vector2.left * this.width / 2;
        Vector2 finalPosition = centerPosition + Vector2.right * this.width / 2;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(initialPosition, finalPosition);
    }
}
