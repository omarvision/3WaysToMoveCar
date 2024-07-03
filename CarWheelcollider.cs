using UnityEngine;
using UnityEngine.InputSystem;

/* Hierarchy of Car wheelcollider gameobject parts:
 
    Car                 script (rigidbody 1000lbs, playerinput)
        Body            mesh (collider)
        Anchors         empty
            anchorFR    empty
                tireFR  trailrenderer
            anchorFL
                tireFL
            anchorBR
                tireBR
            anchorBL
                tireBL
        Wheels          empty
            wheelFR     mesh (no collider)
            wheelFL
            wheelBR
            wheelBL
        Colliders 
            colliderFR  wheelcolliders
            colliderFL
            colliderBR
            colliderBL
*/

[RequireComponent(typeof(Rigidbody))] 
[RequireComponent(typeof(PlayerInput))]
public class CarWheelcollider : MonoBehaviour
{
    public struct InputInfo
    {
        public Vector2 steer;
        public float gas;
        public float brake;
        public int grounded;
    }
    [System.Serializable]
    public struct WheelInfo
    {
        public Transform wheel;
        public WheelCollider collider;
        public TrailRenderer tire;
        [HideInInspector]
        public float maxDistance;       //calculated in start
    }

    public float motor = 5000;
    public float steer = 50;
    public float brake = 5000;
    public WheelInfo FR;
    public WheelInfo FL;
    public WheelInfo BR;
    public WheelInfo BL;
    public bool isCarOnMenu;

    private InputInfo input;

    private void Start()
    {
        isCarOnMenu = (gameObject.scene.name == "Menu");

        //distance to raycast to ground
        FR.maxDistance = FR.wheel.GetComponent<Renderer>().bounds.extents.y;
        FL.maxDistance = FL.wheel.GetComponent<Renderer>().bounds.extents.y;
        BR.maxDistance = BR.wheel.GetComponent<Renderer>().bounds.extents.y;
        BL.maxDistance = BL.wheel.GetComponent<Renderer>().bounds.extents.y;
    }
    private void FixedUpdate()
    {
        CarGrounded();

        if (isCarOnMenu == false)
        {
            CarTurn();
            CarMove();
        }        

        SpinWheels();
    }
    private void OnGUI()
    {
        if (isCarOnMenu == true)
            return;
        GUIStyle style = new GUIStyle();
        style.fontSize = 28;
        style.normal.textColor = Color.black;
        GUI.Label(new Rect(10, 130, 300, 50), string.Format("steer {0}, gas {1}, brake {2}", input.steer.x, input.gas, input.brake), style);
    }
    private void OnSteer(InputValue value)
    {
        input.steer = value.Get<Vector2>();
    }
    private void OnGas(InputValue value)
    {
        input.gas = value.Get<float>();
    }
    private void OnBrake(InputValue value)
    {
        input.brake = value.Get<float>();
    }
    private void OnReset(InputValue value)
    {
        //upright car
        this.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y, 0);
    }
    private void CarTurn()
    {
        FR.collider.steerAngle = steer * input.steer.x;
        FL.collider.steerAngle = steer * input.steer.x;
    }
    private void CarMove()
    {
        FR.collider.motorTorque = motor * input.gas;
        FL.collider.motorTorque = motor * input.gas;
        BR.collider.motorTorque = motor * input.gas;
        BL.collider.motorTorque = motor * input.gas;

        FR.collider.brakeTorque = brake * input.brake;
        FL.collider.brakeTorque = brake * input.brake;
        BR.collider.brakeTorque = brake * input.brake;
        BL.collider.brakeTorque = brake * input.brake;
    }
    private void SpinWheels()
    {
        Vector3 position;
        Quaternion rotation;

        FR.collider.GetWorldPose(out position, out rotation);
        FR.wheel.position = position;
        FR.wheel.rotation = rotation;

        FL.collider.GetWorldPose(out position, out rotation);
        FL.wheel.position = position;
        FL.wheel.rotation = rotation;

        BR.collider.GetWorldPose(out position, out rotation);
        BR.wheel.position = position;
        BR.wheel.rotation = rotation;

        BL.collider.GetWorldPose(out position, out rotation);
        BL.wheel.position = position;
        BL.wheel.rotation = rotation;
    }
    private void CarGrounded()
    {
        //Purpose:
        //  check each wheel contact with ground
        //  'tire' trail emitting = wheel contact with ground

        input.grounded = 0;

        if (Physics.Raycast(FR.wheel.transform.position, -this.transform.up, FR.maxDistance) == true)
        {
            input.grounded++;
            FR.tire.emitting = true;
        }
        else
        {
            FR.tire.emitting = false;
        }

        if (Physics.Raycast(FL.wheel.transform.position, -this.transform.up, FL.maxDistance) == true)
        {
            input.grounded++;
            FL.tire.emitting = true;
        }
        else
        {
            FL.tire.emitting = false;
        }

        if (Physics.Raycast(BR.wheel.transform.position, -this.transform.up, BR.maxDistance) == true)
        {
            input.grounded++;
            BR.tire.emitting = true;
        }
        else
        {
            BR.tire.emitting = false;
        }

        if (Physics.Raycast(BL.wheel.transform.position, -this.transform.up, BL.maxDistance) == true)
        {
            input.grounded++;
            BL.tire.emitting = true;
        }
        else
        {
            BL.tire.emitting = false;
        }

    }
}
