using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapshotController : MonoBehaviour
{
    public Transform target;
    public float xMax, xMin, yMax, yMin, xMinOffSet, xMaxOffSet, yMinOffSet, yMaxOffSet, zoomSensitivity;

    Camera camera;
    MenuController menuController;
    CameraController cameraController;

    Vector2 input;
    float distance = 5;

    void Start()
    {
        camera = FindObjectOfType<Camera>();
        menuController = FindObjectOfType<MenuController>();
        cameraController = FindObjectOfType<CameraController>();
        Debug.Log(name);
        Debug.Log(transform.parent.name);
    }
    
    void Update()
    {
        input = new Vector2((Input.mousePosition.x / camera.pixelWidth), (Input.mousePosition.y / camera.pixelHeight));
        distance += Input.GetAxis("Mouse ScrollWheel") * -zoomSensitivity;
        Debug.Log(input.x);

        Vector3 cameraPos = target.transform.position + (target.transform.rotation * Quaternion.Euler(Mathf.Lerp(yMin,yMax,input.y), Mathf.Lerp(xMin,xMax,input.x) + 180f, 0f) * new Vector3(0, 0, -distance));
        Vector3 lookOffSet = new Vector3(Mathf.Lerp(xMinOffSet, xMaxOffSet, input.x), Mathf.Lerp(yMinOffSet, yMaxOffSet, input.y), 0f);
        camera.transform.position = cameraPos;
        camera.transform.LookAt(target.TransformPoint(lookOffSet));

        if (Input.GetButtonDown("Fire1"))
        {
            TakeSnapshot();
            if (menuController)
            {
                menuController.Return();
            }
            Cancel();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }
    }

    public void TakeSnapshot()
    {
        camera.targetTexture = new RenderTexture(1920, 1080, 24);
        var snapShot = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
        RenderTexture.active = camera.targetTexture;
        camera.Render();
        snapShot.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
        byte[] bytes = snapShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/../Snapshots/" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png", bytes);
        Debug.Log("*snap*");
        camera.targetTexture = null;
    }

    public void Cancel()
    {
        if (cameraController)
        {
            cameraController.enabled = true;
            cameraController.ResetRot();
        }
        enabled = false;
    }

    public static void Enable()
    {
        FindObjectOfType<SnapshotController>().enabled = true;
    }
}