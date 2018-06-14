using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerUtilities : MonoBehaviour {

    [SerializeField]
    private Camera cam;

    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;
    private Vector3 rotation = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Vector3 thrusterForce = Vector3.zero;

    //Player should not be able to look more than 85 degrees up or down
    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    //Gets a movement vector
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    //Gets a rotational vector
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    //Gets a rotational vector for the camera
    public void RotateCamera(float _cameraRotationX)
    {
        cameraRotationX = _cameraRotationX;
    }

    //Gets a thrust force vector
    public void ApplyThruster(Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce;
    }

    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    //Perform movement based on velocity
    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if(thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    //Perform rotation
    void PerformRotation()
    {
        //Take our vector 3 and turn it into a Quaternion to perform rotation
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if(cam != null)
        {
            //Set rotation and clamp it
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit); //-85 to 85 degrees

            //Apply rotation to transform of camera
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }






}
