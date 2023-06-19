using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private bool pressedRMB = false;
    private bool pressedCtrl = false;
    private Transform branch;
    private Vector3 initialBranchPosition = new Vector3();
    private Vector3 initialMousePosition = new Vector3();
    private Vector3 initialBranchScale = new Vector3(1, 1, 1);

    void Start()
    {
        branch = transform.GetChild(1);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            initialMousePosition = Input.mousePosition;
            initialBranchPosition = branch.localPosition;
            pressedRMB = true;
        }

        if (pressedRMB)
        {
            branch.localPosition = initialBranchPosition + (Input.mousePosition - initialMousePosition);
        }

        if (Input.GetMouseButtonUp(1))
        {
            pressedRMB = false;
        }


        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            pressedCtrl = true;
        }

        if (pressedCtrl)
        {
            float scrollDelta = Input.mouseScrollDelta.y;
            initialBranchScale += new Vector3(1, 1, 1) * scrollDelta * 0.1f;
            if (initialBranchScale.x > 0.1f)
            {
                branch.localScale = initialBranchScale;
            }
            else
            {
                initialBranchScale -= new Vector3(1, 1, 1) * scrollDelta * 0.1f;
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            pressedCtrl = false;
        }
    }
}
