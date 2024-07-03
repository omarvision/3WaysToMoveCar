using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

/* Hierarchy of Car physics gameobject parts:
 
    Car                 script (rigidbody 1000lbs, collider, playerinput)
        moveto          empty (for camera to follow)
        anchorFR        empty (for steer wheel angles)
            wheelFR     mesh (spherecollider)
            tireFR      trail renderer (position to bottom of wheel)
        anchorFL    
            wheelFL
            tireFL
        anchorBR  
            wheelBR
            tireBR
        anchorBL 
            wheelBL 
            tireBL
*/

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]   //remember: set mass=1000, drag=1, andgulardrag=5
[RequireComponent(typeof(PlayerInput))] //remember: set the component's inputaction asset
public class CarPhysics : MonoBehaviour
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
        public GameObject anchor;
        public GameObject wheel;
        public TrailRenderer tire;
        [HideInInspector]
        public float radius; //radius of the wheel. we will get from the bounds in Start       
    }

    public float Movespeed = 35;
    public float Turnspeed = 90;
    public float BrakeStrength = 5;
    public WheelInfo FR;
    public WheelInfo FL;
    public WheelInfo BR;
    public WheelInfo BL;
    public bool isCarOnMenu;

    private Rigidbody rb = null;
    private float origDrag;
    private InputInfo input;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        origDrag = rb.linearDamping;
        isCarOnMenu = (gameObject.scene.name == "Menu");

        //need the radius to calculate the spin of wheel
        FR.radius = FR.wheel.GetComponent<Renderer>().bounds.extents.y;
        FL.radius = FL.wheel.GetComponent<Renderer>().bounds.extents.y;
        BR.radius = BR.wheel.GetComponent<Renderer>().bounds.extents.y;
        BL.radius = BL.wheel.GetComponent<Renderer>().bounds.extents.y;

        //the wheels, dont collide with the car body
        Physics.IgnoreCollision(this.GetComponent<Collider>(), FR.wheel.GetComponent<Collider>());
        Physics.IgnoreCollision(this.GetComponent<Collider>(), FL.wheel.GetComponent<Collider>());
        Physics.IgnoreCollision(this.GetComponent<Collider>(), BR.wheel.GetComponent<Collider>());
        Physics.IgnoreCollision(this.GetComponent<Collider>(), BL.wheel.GetComponent<Collider>());
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
        //this is just to display debug text on screen
        GUIStyle style = new GUIStyle();
        style.fontSize = 28;
        style.normal.textColor = Color.black;
        GUI.Label(new Rect(10, 10, 300, 50), string.Format("steer {0}, gas {1}, brake {2}", input.steer.x, input.gas, input.brake), style);
        GUI.Label(new Rect(10, 30, 300, 50), string.Format("grounded {0}", input.grounded), style);
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
        //upright the car, in case it flipped
        this.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y, 0);
    }
    private void CarGrounded()
    {
        //Purpose:
        //  check each wheel contact with ground
        //  'tire' trail emitting = wheel contact with ground

        input.grounded = 0;

        if (Physics.Raycast(FR.anchor.transform.position, -this.transform.up, FR.radius) == true)
        {
            input.grounded++;
            FR.tire.emitting = true;
        }
        else
        {
            FR.tire.emitting = false;
        }

        if (Physics.Raycast(FL.anchor.transform.position, -this.transform.up, FL.radius) == true)
        {
            input.grounded++;
            FL.tire.emitting = true;
        }
        else
        {
            FL.tire.emitting = false;
        }

        if (Physics.Raycast(BR.anchor.transform.position, -this.transform.up, BR.radius) == true)
        {
            input.grounded++;
            BR.tire.emitting = true;
        }
        else
        {
            BR.tire.emitting = false;
        }

        if (Physics.Raycast(BL.anchor.transform.position, -this.transform.up, BL.radius) == true)
        {
            input.grounded++;
            BL.tire.emitting = true;
        }
        else
        {
            BL.tire.emitting = false;
        }

    }
    private void CarTurn()
    {
        if (input.grounded > 0)
        {
            //turn the car
            this.transform.Rotate(Vector3.up, Turnspeed * input.steer.x * Time.fixedDeltaTime);
        }
        //turn the front wheels
        FR.anchor.transform.localRotation = Quaternion.AngleAxis(45f * input.steer.x, Vector3.up);
        FL.anchor.transform.localRotation = Quaternion.AngleAxis(45f * input.steer.x, Vector3.up);
    }
    private void CarMove()
    {
        if (input.grounded > 0)
        {
            //gas
            rb.AddRelativeForce(Vector3.forward * Movespeed * input.gas * Time.fixedDeltaTime, ForceMode.VelocityChange);
            //brake
            rb.linearDamping = origDrag + (BrakeStrength * input.brake * Time.fixedDeltaTime);
        }
    }
    
    private void SpinWheels()
    {
        // convert linear speed to anglular speed. spin the wheel that much.
        float velocity = rb.linearVelocity.magnitude;
        float direction = Vector3.Dot(rb.linearVelocity, rb.transform.forward) > 0 ? 1f : -1f;
        FR.wheel.transform.Rotate(Vector3.right * direction, ((velocity / FR.radius) * Mathf.Rad2Deg) * Time.fixedDeltaTime);
        FL.wheel.transform.Rotate(Vector3.right * direction, ((velocity / FL.radius) * Mathf.Rad2Deg) * Time.fixedDeltaTime);
        BR.wheel.transform.Rotate(Vector3.right * direction, ((velocity / BR.radius) * Mathf.Rad2Deg) * Time.fixedDeltaTime);
        BL.wheel.transform.Rotate(Vector3.right * direction, ((velocity / BL.radius) * Mathf.Rad2Deg) * Time.fixedDeltaTime);
    }
    
}
