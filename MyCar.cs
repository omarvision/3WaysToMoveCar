using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class MyCar : MonoBehaviour
{
    public struct InputInfo
    {
        public Vector2 steer;
        public float gas;
        public float brake;
        public int grounded;
    }

    public float Movespeed = 24;
    public float Turnspeed = 90;

    private Rigidbody rb = null;
    private InputInfo input;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        CarTurn();
        CarMove();
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
    private void CarTurn()
    {
        //turn the car
        this.transform.Rotate(Vector3.up, Turnspeed * input.steer.x * Time.fixedDeltaTime);
    }
    private void CarMove()
    {
        //gas
        rb.AddRelativeForce(Vector3.forward * Movespeed * input.gas * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
}
