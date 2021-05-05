using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeControl : MonoBehaviour, IControllable
{
    private float initialDistance;
    private Vector3 dragPosition;

    private Renderer objRenderer;
    private Color initialColor;

    private void Awake()
    {
        dragPosition = transform.position;
    }

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        initialColor = objRenderer.material.color;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, dragPosition, 0.5f);
    }

    public void OnSelect()
    {
        initialDistance = Vector3.Distance(Camera.main.transform.position, transform.position);

        objRenderer.material.color = Color.red;
    }

    public void DeSelect()
    {
        objRenderer.material.color = initialColor;
    }

    public void MoveTo(Touch t)
    {
        Vector3 touchedPos = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, initialDistance);
        Vector3 destination = Camera.main.ScreenToWorldPoint(touchedPos);
        dragPosition = destination;
    }
}
