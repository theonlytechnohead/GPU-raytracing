using UnityEngine;

public class StreetViewCamera : MonoBehaviour {
    public float speed = 3.5f;
    private float X;
    private float Y;

    void Update () {
        if (Input.GetMouseButton(0)) {
            transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * speed, Input.GetAxis("Mouse X") * speed, 0));
            X = transform.rotation.eulerAngles.x;
            Y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(X, Y, 0);
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
            Vector3 position = transform.GetChild(0).localPosition;
            position.z += Input.GetAxis("Mouse ScrollWheel") * 5f;
            transform.GetChild(0).localPosition = position;
        }
    }
}