using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    private enum enumMode
    {
        UserPickingCar,
        UserPlay
    }
    private enum enumCar
    {
        None,
        carPhysics,
        carRaycast,
        carWheelCollider
    }

    public GameObject carPhysics = null;
    public GameObject carRaycast = null;
    public GameObject carWheelcollider = null;
    public Button btnPhysics = null;
    public Button btnRaycast = null;
    public Button btnWheelcollider = null;
    public Button btnPlay = null;

    private Vector3 poscarPhysics;
    private Vector3 poscarRaycast;
    private Vector3 poscarWheelCollider;
    private enumCar SelectedCar = enumCar.None;
    private enumCar PickingCar = enumCar.None;
    private enumMode MenuMode = enumMode.UserPickingCar;

    private void Start()
    {
        poscarPhysics = carPhysics.transform.position + carPhysics.transform.right * 8;
        poscarRaycast = carRaycast.transform.position + carRaycast.transform.right * 8;
        poscarWheelCollider = carWheelcollider.transform.position + carWheelcollider.transform.right * 8;
    }
    private void Update()
    {
        bool bFinishRotating = false;
        bool bFinishDrivingOff = false;

        switch (MenuMode)
        {
            case enumMode.UserPickingCar: 
                switch (PickingCar)
                {
                    case enumCar.carPhysics:
                        bFinishRotating = RotateCarTo(ref carPhysics, 90);
                        RotateCarTo(ref carRaycast, 0);
                        RotateCarTo(ref carWheelcollider, 0);
                        Globals.ParkingLot_PrefabCar = "Prefabs\\Car physics";
                        break;
                    case enumCar.carRaycast:
                        RotateCarTo(ref carPhysics, 0);
                        bFinishRotating = RotateCarTo(ref carRaycast, 90);
                        RotateCarTo(ref carWheelcollider, 0);
                        Globals.ParkingLot_PrefabCar = "Prefabs\\Car raycast";
                        break;
                    case enumCar.carWheelCollider:
                        RotateCarTo(ref carPhysics, 0);
                        RotateCarTo(ref carRaycast, 0);
                        bFinishRotating = RotateCarTo(ref carWheelcollider, 90);
                        Globals.ParkingLot_PrefabCar = "Prefabs\\Car wheelcollider";
                        break;
                    default:
                        RotateCarTo(ref carPhysics, 0);
                        RotateCarTo(ref carRaycast, 0);
                        RotateCarTo(ref carWheelcollider, 0);
                        break;
                }
                if (bFinishRotating == true)
                {
                    SelectedCar = PickingCar;
                    btnPlay.enabled = true;                    
                }
                else
                {
                    SelectedCar = enumCar.None;
                    btnPlay.enabled = false;
                }                    
                break;
            case enumMode.UserPlay: 
                switch (SelectedCar)
                {
                    case enumCar.carPhysics:
                        bFinishDrivingOff = DriveOff(ref carPhysics, poscarPhysics);
                        break;
                    case enumCar.carRaycast:
                        bFinishDrivingOff = DriveOff(ref carRaycast, poscarRaycast);
                        break;
                    case enumCar.carWheelCollider:
                        bFinishDrivingOff = DriveOff(ref carWheelcollider, poscarWheelCollider);
                        break;
                }
                if (bFinishDrivingOff == true)
                {
                    LoadPlayScene();
                }
                break;
        }
    }
    private void OnNext(InputValue value)
    {
        switch (PickingCar)
        {
            case enumCar.carPhysics:
                btnCarRaycast_Click();
                break;
            case enumCar.carRaycast:
                btnCarWheelcollider_Click();
                break;
            case enumCar.carWheelCollider:
                btnCarPhysics_Click();
                break;
            default:
                btnCarPhysics_Click();
                break;
        }
    }
    private void OnPrevious(InputValue value)
    {
        switch (PickingCar)
        {
            case enumCar.carPhysics:
                btnCarWheelcollider_Click();
                break;
            case enumCar.carRaycast:
                btnCarPhysics_Click();
                break;
            case enumCar.carWheelCollider:
                btnCarRaycast_Click();
                break;
            default:
                btnCarWheelcollider_Click();
                break;
        }
    }
    private void OnPlay(InputValue value)
    {
        btnPlay_Click();
    }
    public void btnCarPhysics_Click()
    {
        SelectedCar = enumCar.None;
        PickingCar = enumCar.carPhysics;
    }
    public void btnCarRaycast_Click()
    {
        SelectedCar = enumCar.None;
        PickingCar = enumCar.carRaycast;
    }
    public void btnCarWheelcollider_Click()
    {
        SelectedCar = enumCar.None;
        PickingCar = enumCar.carWheelCollider;
    }
    public void btnPlay_Click()
    {
        MenuMode = enumMode.UserPlay;

        btnPhysics.enabled = false;
        btnRaycast.enabled = false;
        btnWheelcollider.enabled = false;
    }
    private bool RotateCarTo(ref GameObject car, float toAngle)
    {
        Quaternion target = Quaternion.Euler(0, toAngle, 0);
        car.transform.rotation = Quaternion.Slerp(car.transform.rotation, target, 5 * Time.deltaTime);
        
        if (Mathf.Abs(car.transform.rotation.y - target.y) < 0.1f)
            return true;
        
        return false;
    }
    private bool DriveOff(ref GameObject car, Vector3 target)
    {
        car.transform.position = Vector3.Lerp(car.transform.position, target, 3 * Time.deltaTime);
        float dist = Vector3.Distance(car.transform.position, target);
        if (dist < 1f)
            return true;
        return false;
    }
    private void LoadPlayScene()
    {
        Debug.Log(string.Format("LoadPlayScene car = {0}", Globals.ParkingLot_PrefabCar));
        SceneManager.LoadScene("ParkingLot");
    }
}
