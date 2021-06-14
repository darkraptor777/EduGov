using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float[] yBounds;
    public float[] xBounds;
    public float[] zoomBounds;

    public float speed;
    public float zoomSpeed;

    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        float camSize = cam.orthographicSize;
        if (Input.GetKey("w"))
        {
            Debug.Log("W pressed");
            y += speed;
        }
        else if (Input.GetKey("s"))
        {
            Debug.Log("S pressed");
            y -= speed;
        }
        if (Input.GetKey("d"))
        {
            Debug.Log("D pressed");
            x += speed;
        }
        else if (Input.GetKey("a"))
        {
            Debug.Log("A pressed");
            x -= speed;
        }

        if (Input.GetKeyDown("q"))
        {
            Debug.Log("Q pressed");
            camSize -= zoomSpeed;
        }
        else if (Input.GetKeyDown("e"))
        {
            Debug.Log("E pressed");
            camSize += zoomSpeed;
        }

        if (x < xBounds[0]) x = xBounds[0];
        if (x > xBounds[1]) x = xBounds[1];

        if (y < yBounds[0]) y = yBounds[0];
        if (y > yBounds[1]) y = yBounds[1];

        if (camSize < zoomBounds[0]) camSize = zoomBounds[0];
        if (camSize > zoomBounds[1]) camSize = zoomBounds[1];

        transform.position = new Vector3(x, y, z);
        cam.orthographicSize = camSize;
    }
}
