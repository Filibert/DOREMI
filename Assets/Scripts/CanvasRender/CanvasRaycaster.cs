//Attach this script to your Canvas GameObject.
//Also attach a GraphicsRaycaster component to your canvas by clicking the Add Component button in the Inspector window.
//Also make sure you have an EventSystem in your hierarchy.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CanvasRaycaster : MonoBehaviour
{
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    List<RaycastResult> results;

    GameObject selectedInstrument;
    bool isClicking;
    int previousSize;

    void Start()
    {
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();

        //Set up the new Pointer Event
        m_PointerEventData = new PointerEventData(m_EventSystem);

        //Create a list of Raycast Results
        results = new List<RaycastResult>();

        isClicking = false;
    }

    void Update()
    {
        if (Input.GetAxis("Fire1") > 0.5)
        {
            if (!isClicking)
            {
                isClicking = true;
                //Set the Pointer Event Position to that of the mouse position
                m_PointerEventData.position = Input.mousePosition;

                //Raycast using the Graphics Raycaster and mouse click position
                m_Raycaster.Raycast(m_PointerEventData, results);
               
                if (results.Count > 0 && results.Count > previousSize)
                {
                    selectedInstrument = results[results.Count - 1].gameObject;
                    previousSize = results.Count;
                    Debug.Log(selectedInstrument.name);
                }
                else
                {
                    selectedInstrument = null;
                }
            }
        }
        else
        {
            isClicking = false;
        }
    }

    private void OnGUI()
    {

    }
}