using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    public Transform target; // The target the camera follows (usually the pllayer, but it can be changed [in-game cinematic potential])

    // Zoom variables
    public float distance = 20.0f; // Default distance from target
    public float minDistance = 14.0f;
    public float maxDistance = 30.0f;
    public float zoomSpeed = 6.0f;

    // Rotation variables
    public float rotationSpeed = 5.0f; // Cam speed rotation
    public float currentX = 0f;
    public float currentY = 0f;
    private const float minAngleY = 0f;
    private const float maxAngleY = 90f;

    public float collisionRadius;
    public LayerMask collisionMask;

    private Quaternion currentRotation; // Store the current rotation as a Quaternion

    private void Start()
    {
        // Initialize current rotation based on the camera's rotation
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
        collisionRadius = minDistance;
    }

    private void Update()
    {
        // Camera rotation
        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentY = Mathf.Clamp(currentY, minAngleY, maxAngleY); // Clamp vertical angle
            if (currentY == maxAngleY) currentY -= 0.01f;
        }

        /// Zoom with mouse wheel or keyboard
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            distance -= scroll * zoomSpeed;
        }
        // Keyboard equivalent (in case the wheel isn't good or whatever)
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Z)) // Zoom out
        {
            distance += zoomSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Z)) // Zoom in
        {
            distance -= zoomSpeed * Time.deltaTime;
        }


        distance = Mathf.Clamp(distance, minDistance, maxDistance); // Clamp zoom distance
    }

    private void LateUpdate()
    {
        // Calculate new rotation for the parent GO
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        currentRotation = rotation;

        // Calculate the new position for the camera (relative to target)
        Vector3 dir = new Vector3(0, 0, -distance);
        Vector3 pos = target.position + currentRotation * dir;

        // Check for collision and adjust if necessary
        RaycastHit hit;
        if (Physics.SphereCast(target.position, collisionRadius, dir.normalized, out hit, distance, collisionMask))
        {
            // Move the camera to just before the collision point
            float adjustedDistance = Vector3.Distance(target.position, hit.point) - collisionRadius;
            pos = target.position + currentRotation * Vector3.forward * Mathf.Clamp(adjustedDistance, minDistance, maxDistance);
        }

        // Apply the calculated position and rotation to the parent GO
        transform.position = pos;
        transform.LookAt(target.position);

        // Ensuring the camera's rotation is adjusted if obstructed
        //HandleObstruction();
    }

    /*void HandleObstruction()
    {
        RaycastHit hit;
        Vector3 direction = transform.position - target.position;
        if (Physics.Raycast(target.position, direction.normalized, out hit, distance, collisionMask))
        {
            //Check if the camera is blocked and adjust rotation to find a clear view
            Vector3 adjustedDirection = Vector3.Reflect(direction, -hit.normal);
            transform.rotation = Quaternion.LookRotation(adjustedDirection);
        }
    }*/

}
