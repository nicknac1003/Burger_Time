using UnityEngine;
using UnityEditor;
public class RainbowQTE : MonoBehaviour
{

    [Header("Rainbow Parameters")]
    public Transform pivotPoint; // Bottom center of the semi-circle to orient the arrow.
    public float radius = 1.0f; // Radius of the semi-circle

    [Header("Arrow Parameters")]
    public Transform arrowPos;
    public float arrowSpeed = 1.0f; // # secs to complete a full rotation
    public float arrowSlowDown = 0.7f; // Slow down the arrow when it reaches the good zone

    [Header("Good-Zone Parameters")]
    public Transform goodZone;
    public float widthScale = 1.0f; // Make the good zone wider or narrower
    public float zRotation = 0.0f; // Rotate the good zone around the rainbow

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        goodZone.localScale = new Vector3(widthScale, goodZone.localScale.y, goodZone.localScale.z);
        goodZone.localRotation = Quaternion.Euler(0.0f, 0.0f, zRotation);

        arrowPos.position = pivotPoint.position + new Vector3(0.0f, radius, 0.0f);
    }

    void Update()
    {
        MoveArrow();
    }


    private void MoveArrow()
    {
        //arrowPos = pivot + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        arrowPos.position = pivotPoint.position + new Vector3(Mathf.Cos(Time.time * arrowSpeed) * radius, Mathf.Sin(Time.time * arrowSpeed) * radius, 0);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        // Draws a wire arc around the pivot (this GameObject's position)
        // Parameters: center, normal, from (starting direction), angle in degrees, and radius
        Handles.DrawWireArc(pivotPoint.position, Vector3.forward, Vector3.right, 180, radius);
    }
#endif
}
