using UnityEngine;

public class GrapplingPhysics : MonoBehaviour
{
    public float maxSwingAngle = 45f; // Maximum angle the grappling hook can swing from the initial position.
    public float swingSpeed = 5f; // Speed of the swing animation.
    public Transform hingePoint; // Reference to the hinge point GameObject.

    private HingeJoint2D hingeJoint;
    private LineRenderer lineRenderer;

    void Start()
    {
        hingeJoint = GetComponent<HingeJoint2D>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // Calculate the swing angle.
        float angle = Mathf.Sin(Time.time * swingSpeed) * maxSwingAngle;

        // Set the target angle of the hinge joint.
        hingeJoint.limits = new JointAngleLimits2D { min = -angle, max = angle };

        // Update the Line Renderer's positions to match the swing.
        Vector3[] linePositions = { transform.position, hingePoint.position };
        lineRenderer.SetPositions(linePositions);
    }
}
