using UnityEngine;
using UnityEditor;
public class RainbowQTE : QuickTimeEvent
{

    [Header("Rainbow Parameters")]
    public Transform pivotPoint; // Bottom center of the semi-circle to orient the arrow.
    public float radius = 1.0f; // Radius of the semi-circle
    public bool QTESuccess = false; // True if the arrow is in the good zone

    [Header("Arrow Parameters")]
    public Transform arrowPos;
    public float maxArrowSpeed = 1.0f; // # secs to complete a full rotation
    public float arrowSlowDown = 0.7f; // Slow down the arrow when it reaches the good zone
    public float angleBuffer = 10.0f; // Min angle of arrow along rainbow = angleBuffer, Max angle = 180 - angleBuffer

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
        // Get position of the arrow along the semi-circle & look at center of the semi-circle
        float angle = angleBuffer + Mathf.PingPong((Time.time * maxArrowSpeed * Mathf.Rad2Deg), (180f - angleBuffer * 2));
        Quaternion rotation = Quaternion.Euler(0, 0, angle - 90f);

        // Move along the semi-circle
        angle = angle * Mathf.Deg2Rad;
        arrowPos.position = pivotPoint.position + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        arrowPos.rotation = rotation;
    }

    private void OnTriggerEnter()
    {
        QTESuccess = true;
    }

    private void OnTriggerExit()
    {
        QTESuccess = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Handles.color = Color.blue;
        // Draws a wire arc around the pivot (this GameObject's position)
        // Parameters: center, normal, from (starting direction), angle in degrees, and radius
        //Handles.DrawWireArc(pivotPoint.position, Vector3.forward, Vector3.right, 180, radius);
    }

    public override float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput)
    {
        throw new System.NotImplementedException();
    }

    public override bool InProgress()
    {
        throw new System.NotImplementedException();
    }
#endif
}
