using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManagerScript : MonoBehaviour
{
    private IControllable selectedObject;
    private enum TouchType {Tap, Drag, Zoom, Scale, Rotate, Determining, None}
    TouchType touch;

    // Tap
    private float tapTimeThreshold;
    private float tapTimeStart;
    private float tapTimeEnd;
    private bool hasMoved;

    // Scaling
    private float initialDistance;
    private Vector3 initialScale;

    // Zoom
    private float currentZoom;
    private const float MAX_ZOOM = 5;
    private const float MIN_ZOOM = -5;

    // Scale + Zoom
    private float determineScaleZoomTotal;
    private const float SCALE_ZOOM_THRESHOLD = 1;

    // Roatating
    private Quaternion initialRotation;
    private float initialAngel;
    private float determineRotateTotal;
    private const float ROTATE_THRESHOLD = .01f;

    // Accelerometer
    private bool accelEnabled;
    private const float SPEED = 10;

    void Awake()
    {
        selectedObject = null;
        touch = TouchType.None;

        tapTimeThreshold = 0.5f;
        tapTimeStart = 0;
        tapTimeEnd = 0;
        hasMoved = false;

        initialDistance = 0;
        initialScale = Vector3.one;

        currentZoom = 0;
        determineScaleZoomTotal = 0;

        initialRotation = Quaternion.identity;
        initialAngel = 0;
        determineRotateTotal = 0;

        accelEnabled = false;
    }

    void Start()
    {
        // No gyroscope on my phone so no gyroscope implementation
        Input.gyro.enabled = true;
        if (SystemInfo.supportsGyroscope)
        {
            Debug.Log("gyro");
        }
        else
        {
            Debug.Log("no gyro");
        }
    }

    void Update()
    {
        touch = DetermineTouch();
        Debug.Log(touch);

        if (Input.touchCount == 0)
        {
            if (selectedObject != null && accelEnabled)
            {
                // Accelerometer translate is asuming phone is parallel to the floor
                selectedObject.gameObject.transform.Translate(new Vector3(-Input.acceleration.y, 0, Input.acceleration.x) * SPEED);
            }
        }

        switch(touch)
        {
            case TouchType.Tap:
                Ray ourRay = Camera.main.ScreenPointToRay(Input.touches[0].position);

                RaycastHit info;
                if (Physics.Raycast(ourRay, out info))
                {
                    IControllable hitObject = info.transform.GetComponent<IControllable>();

                    if(hitObject != null)
                    {
                        if(selectedObject == null)
                        {
                            Select(hitObject);
                        }
                        else if(selectedObject != hitObject)
                        {
                            DeSelectControl();
                            Select(hitObject);
                        }
                        else
                        {
                            DeSelectControl();
                        }
                    }
                }
                break;

            case TouchType.Drag:
                if(selectedObject != null)
                {
                    selectedObject.MoveTo(Input.touches[0]);
                }
                else
                {
                    DragCamera();
                }
                break;

            case TouchType.Zoom:
                float change = DetermineChange();

                float newZoom = currentZoom + change;

                if (newZoom > MAX_ZOOM)
                {
                    float overZoom = newZoom - MAX_ZOOM;
                    change -= overZoom;
                }
                else if (newZoom < MIN_ZOOM)
                {
                    float underZoom = newZoom - MIN_ZOOM;
                    change -= underZoom;
                }

                Camera.main.transform.position += Camera.main.transform.forward * change;
                currentZoom += change;
                break;

            case TouchType.Scale:
                float factor = DetermineFactor();
                selectedObject.gameObject.transform.localScale = initialScale * factor;
                break;

            case TouchType.Rotate:
                float theta = DetermineTheta();

                if (selectedObject != null)
                {
                    Quaternion rotation = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, Camera.main.transform.forward);
                    selectedObject.gameObject.transform.rotation = rotation * initialRotation;
                }
                else
                {
                    Quaternion rotation = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, Camera.main.transform.up);
                    Camera.main.transform.rotation = rotation * initialRotation;
                }
                break;

            case TouchType.Determining:
                break;

            case TouchType.None:
                break;

            default:
                Debug.Log("Wacky TouchType");
                break;
        }
    }

    private TouchType DetermineTouch()
    {
        if (Input.touchCount == 1)
        {
            switch (Input.touches[0].phase)
            {
                case TouchPhase.Began:
                    tapTimeStart = Time.time;
                    break;

                case TouchPhase.Moved:
                    hasMoved = true;
                    return TouchType.Drag;

                case TouchPhase.Ended:
                    tapTimeEnd = Time.time;

                    if (IsTap())
                    {
                        return TouchType.Tap;
                    }

                    hasMoved = false;
                    break;

                default:
                    return TouchType.Determining;
            }
        }

        else if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                Vector3 v2 = touchOne.position - touchZero.position;
                initialAngel = Mathf.Atan2(v2.y, v2.x);

                if (selectedObject != null)
                {
                    initialRotation = selectedObject.gameObject.transform.rotation;
                    initialScale = selectedObject.gameObject.transform.localScale;
                }
                else
                {
                    initialRotation = Camera.main.transform.rotation;
                }

                return TouchType.Determining;
            }

            if (touchZero.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Ended)
            {
                determineScaleZoomTotal = 0;
                determineRotateTotal = 0;
            }

            // Ensures that a 2-finger TouchType stays the same until a finger is lifted
            switch(touch)
            {
                case TouchType.Rotate:
                    return TouchType.Rotate;

                case TouchType.Scale:
                    return TouchType.Scale;

                case TouchType.Zoom:
                    return TouchType.Zoom;
            }

            if (Mathf.Approximately(initialDistance, 0))
                return TouchType.Determining;

            float change = DetermineChange();
            float theta = DetermineTheta();

            if(change < 0)
                determineScaleZoomTotal += (change * -1);
            else
                determineScaleZoomTotal += change;

            if(theta < 0)
                determineRotateTotal += (theta * -1);
            else
                determineRotateTotal += theta;

            if (determineScaleZoomTotal >= SCALE_ZOOM_THRESHOLD && selectedObject != null)
            {
                return TouchType.Scale;
            }

            if (determineScaleZoomTotal >= SCALE_ZOOM_THRESHOLD)
            {
                return TouchType.Zoom;
            }

            if (determineRotateTotal >= ROTATE_THRESHOLD)
            {
                return TouchType.Rotate;
            }

            return TouchType.Determining;
        }

        return TouchType.None;     
    }

    private float DetermineChange()
    {
        float initDist = Vector2.Distance(Input.touches[0].position - Input.touches[0].deltaPosition, Input.touches[1].position - Input.touches[1].deltaPosition);
        float currentDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);

        float change = (currentDistance - initDist) * -1;
        change *= Time.deltaTime;

        return change;
    }

    private float DetermineFactor()
    {
        float currentDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
        float factor = currentDistance / initialDistance;

        return factor;
    }

    private float DetermineTheta()
    {
        Vector3 v = Input.touches[1].position - Input.touches[0].position;
        float theta = Mathf.Atan2(v.y, v.x);
        theta = theta - initialAngel;

        return theta;
    }

    private bool IsTap()
    {
        float touchTime = tapTimeEnd - tapTimeStart;

        if (touchTime <= tapTimeThreshold && !hasMoved)
            return true;
        else
            return false;
    }

    public void ResetCameraZoom()
    {
        float change = (0 - currentZoom);

        Camera.main.transform.position += Camera.main.transform.forward * change;
        currentZoom = 0;
    }

    public void ToggleAccelerometer(bool toggle)
    {
        accelEnabled = toggle;
    }

    private void Select(IControllable hitObject)
    {
        hitObject.OnSelect();
        selectedObject = hitObject;
    }

    private void DeSelectControl()
    {
        selectedObject.DeSelect();
        selectedObject = null;
    }

    private void DragCamera()
    {
        Vector2 touchDeltaPos = Input.touches[0].deltaPosition * Time.deltaTime;
        Camera.main.transform.Translate(-touchDeltaPos.x, -touchDeltaPos.y, 0);
    }
}
