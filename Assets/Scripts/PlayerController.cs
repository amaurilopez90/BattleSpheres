using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerUtilities))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

    //Creating movement
    [SerializeField] //Make it show in inspector even though it is private
    private float speed = 5f;

    [SerializeField]
    private float sensitivity = 3f;

    [SerializeField]
    private float thrusterForce = 1000f;

    [SerializeField]
    private float fuelDecreaseSpeed = 1f;

    [SerializeField]
    private float fuelRegenSpeed = 0.3f;
    private float fuelAmount = 1f;

    [SerializeField]
    private LayerMask environmentMask;

    [Header("Spring Settings:")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    //Component caching
    private Animator animator;
    private PlayerUtilities motor;
    private ConfigurableJoint joint;

    private void Start()
    {
        motor = GetComponent<PlayerUtilities>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }

    private void Update()
    {
        //If pause menu is active, don't continue doing anything
        if (PauseMenu.isOn)
        {
            //Re-enable cursor when pause menu is active
            if (Cursor.lockState != CursorLockMode.None)
                Cursor.lockState = CursorLockMode.None;

            //Halt movement, so any movement input right when we hit "pause" will be cancelled
            motor.Move(Vector3.zero);
            motor.Rotate(Vector3.zero);
            motor.RotateCamera(0);
            motor.ApplyThruster(Vector3.zero);
            return;
        }
            

        //Check to see if cursor is locked, if not, lock it
        if(Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        /* 
         Set target position for spring
         to make physics behave correctly when it comes to applying gravity while flying over objects
         */
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 100f, environmentMask))
        {
            joint.targetPosition = new Vector3(0f, -hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }

        //Calculate movemnt velocity as a 3D vector
        float xMovement = Input.GetAxis("Horizontal");
        float zMovement = Input.GetAxis("Vertical");

        Vector3 moveHorizontal = transform.right * xMovement;
        Vector3 moveVertical = transform.forward * zMovement;

        //Final movement vector
        Vector3 velocity = (moveHorizontal + moveVertical) * speed; //combine vector

        //Animate movement
        animator.SetFloat("ForwardVelocity", zMovement);
        animator.SetFloat("HorizontalVelocity", xMovement);

        //apply movement
        motor.Move(velocity);

        //Calculate rotation as a 3D vector(for player turning)
        float yRotation = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yRotation, 0f) * sensitivity;

        //Apply rotation
        motor.Rotate(rotation);

        //Calculate camera rotation as a 3D vector
        float xRotation = Input.GetAxisRaw("Mouse Y");

        float cameraRotationX = xRotation * sensitivity;

        //Apply camera rotation
        motor.RotateCamera(cameraRotationX);

        //Calculate thruster force
        Vector3 thruster = Vector3.zero;
        if (Input.GetButton("Jump") && fuelAmount > 0f)
        {
            fuelAmount -= fuelDecreaseSpeed * Time.deltaTime;

            //Only add a thruster force if the fuel amount is greater than a small amount, so the player can't hang in the air after using fuel
            if(fuelAmount >= 0.01f)
            {
                thruster = Vector3.up * thrusterForce;

                //If jumping we want to disable the configurable joint settings
                SetJointSettings(0f);
            }
        }
        else
        {
            fuelAmount += fuelRegenSpeed * Time.deltaTime;
            SetJointSettings(jointSpring);
        }

        //Clamp fuelAmount between 0 and 1
        fuelAmount = Mathf.Clamp(fuelAmount, 0f, 1f);

        //Apply thruster force
        motor.ApplyThruster(thruster);
    }

    public float GetThrusterFuelAmount()
    {
        return fuelAmount;
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive {
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce
        };
    }
}
