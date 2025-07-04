using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TouchDetect : MonoBehaviour
{
    [SerializeField] ARAnchorManager arnchorManager;
    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] GameObject plot;
    [SerializeField] TMP_Text debug_text;
    [SerializeField][UnityEngine.Range(0, 1)] float scale;
    [SerializeField] Vector3 offset;


    private TouchControls controls;
    private bool isPlaced;
    Vector3 orignalScale;

    void Awake()
    {
        orignalScale = plot.transform.localScale;
        
        plot.SetActive(false);
        controls = new TouchControls();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.TouchInput.Touch.performed += Touch_performed;
    }

    void OnDisable()
    {
        controls.TouchInput.Touch.performed -= Touch_performed;
        controls.Disable();
    }

    private void Touch_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (isPlaced) return;

        Vector2 touchPosition = obj.ReadValue<Vector2>();
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        if (raycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            debug_text.text = "isPlaced:- " + isPlaced;
            isPlaced = true;

            Pose hit = hits[0].pose;

            ARPlane hitPlane = planeManager.GetPlane(hits[0].trackableId);

            if (hitPlane != null) 
            {
                ARAnchor anchor=arnchorManager.AttachAnchor(hitPlane,hit);

                if (anchor != null)
                {
                    plot.SetActive(true);
                    plot.transform.position = hit.position + offset;
                    //plot.transform.rotation = hit.rotation;
                    //plot.transform.localScale = Vector3.one * scale;
                    isPlaced =true;

                    foreach (var plane in planeManager.trackables)
                    {
                        plane.gameObject.SetActive(false);
                    }
                }
            }


        }
    }

    public void ResetPlacement()
    {
        plot.transform.localScale = orignalScale;

        isPlaced = false;

        // re-enable plane detection
        planeManager.enabled = true;

        // reactivate existing planes so they are visible again
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(true);
        }

        debug_text.text = $"{plot.transform.localScale}";

        // optionally hide the plot again:
        plot.SetActive(false);
    }

}