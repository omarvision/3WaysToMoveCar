using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ParkingLotManager : MonoBehaviour
{
    public Camera cam = null;
    private GameObject car = null;

    private void Start()
    {
        string carToLoad = Globals.ParkingLot_PrefabCar;
        if (carToLoad.Length > 0)
        {
            GameObject prefab = Resources.Load<GameObject>(carToLoad);
            
            //place car
            car = Instantiate(prefab);
            car.transform.position = Vector3.zero;
            car.transform.rotation = Quaternion.identity;

            //set camera
            CameraController camscript = cam.GetComponent<CameraController>();
            camscript.moveto = FindChildWithNameStartingWith(car, "moveto");
            camscript.lookat = car;
        }        
    }
    private void OnMenu(InputValue value)
    {
        SceneManager.LoadScene("Menu");
    }
    private GameObject FindChildWithNameStartingWith(GameObject parent, string childnamestartwith)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.name.ToLower().StartsWith(childnamestartwith.ToLower()) == true)
            {
                return child.gameObject;
            }
        }
        return null;
    }
}
