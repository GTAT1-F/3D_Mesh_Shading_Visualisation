using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private float speed;

    private void Start()
    {
        speed = 40f;
        cam = GetComponent<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W)){
            cam.transform.position += cam.transform.forward * Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            cam.transform.position -= cam.transform.right * Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            cam.transform.position += cam.transform.right * Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            cam.transform.position -= cam.transform.forward * Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.E))
        {
            Vector3 rotation = cam.transform.localRotation.eulerAngles;
            rotation.y += Time.deltaTime * speed;
            transform.localRotation = Quaternion.Euler(rotation);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            Vector3 rotation = cam.transform.localRotation.eulerAngles;
            rotation.y -= Time.deltaTime * speed;
            cam.transform.localRotation = Quaternion.Euler(rotation);
        }
    }
}
