using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleControl : MonoBehaviour, IControllable
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

        objRenderer.material.color = Color.green;
    }

    public void DeSelect()
    {
        objRenderer.material.color = initialColor;
    }

    public void MoveTo(Touch t)
    {
        Ray newPositionRay = Camera.main.ScreenPointToRay(t.position);
        RaycastHit[] hits = Physics.RaycastAll(newPositionRay);
        int groundMask = LayerMask.NameToLayer("Ground");

        float closest = Mathf.Infinity;

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.layer == groundMask)
            {
                if(hit.distance < closest)
                {
                    closest = hit.distance;
                    dragPosition = hit.point;
                    dragPosition += new Vector3(0, 1, 0);
                }
            }
        }
    }
}
