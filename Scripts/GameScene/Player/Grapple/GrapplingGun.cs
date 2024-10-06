using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    public Animator animator;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    private float maxDistance = 100f;
    private SpringJoint joint;
    public AudioSource grappleSound;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetBool("isGrappling", true);
            
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("isGrappling", false);
            StopGrapple();
        }
    }


    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grappleSound.Play();
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;
        }
    }


    void StopGrapple()
    {
        Destroy(joint);
    }



    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    public Transform GetPlayerTransform()
    {
        return player;
    }

 

}