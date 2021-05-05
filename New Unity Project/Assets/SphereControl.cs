using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereControl : MonoBehaviour, IControllable
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

        objRenderer.material.color = Color.blue;
    }

    public void DeSelect()
    {
        objRenderer.material.color = initialColor;
    }

    public void MoveTo(Touch t)
    {
        Ray newPositionRay = Camera.main.ScreenPointToRay(t.position);
        Vector3 destination = newPositionRay.GetPoint(initialDistance);
        dragPosition = destination;
    }
}
