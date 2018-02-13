using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Assets;

public class DrawingManager : MonoBehaviour
{
    //Oculus Touch button input state
    private bool stateDraw;
    private bool stateThumbstickR, stateThumbstickL;    //thumb stick direction
    private bool stateBtnA;
    private bool stateRTrigger;

    int lineIdx;
    Line line;
    List<Line> _line;
    float lineWidth;
    Color lineColor;

    //for colorpicker
    public GameObject ColorPicker;
    private ColorPickerTriangle colorPickerCmpnt;
    private GameObject colorPickerGameObject;
    private Material mat;
    private bool isColorPick = false;

    public GameObject curColor;
    
    private void Start()
    {
        lineWidth = 0.05f;
        lineColor = new Color(255, 0, 0);

        lineIdx = 0;
        _line = new List<Line>();
    }

    private void Update()
    {
        stateDraw = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        stateThumbstickR = OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.RTouch);
        stateThumbstickL = OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.RTouch);
        stateBtnA = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch);
        stateRTrigger = OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);


        //change line width
        if (stateThumbstickR)
        {
            lineWidth += 0.01f;
        }

        if (stateThumbstickL && lineWidth > 0.0f)
        {
            lineWidth -= 0.01f;
        }


        if (stateBtnA)
        {
            if (isColorPick)
                StopColorPick();
            else
                StartColorPick();
        }

        //change color
        if (isColorPick)
        {
            lineColor = colorPickerCmpnt.TheColor;
        }

        curColor.GetComponent<MeshRenderer>().material.color = lineColor;

        //Start drawing - push index trigger
        if (stateDraw) 
        {
            GameObject go = new GameObject("line" + lineIdx);

            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.AddComponent<MeshCollider>();

            line = go.AddComponent<Line>();

            line.originColor = lineColor;
            line.originWidth = lineWidth/2;

            _line.Add(line);

            lineIdx++;
          
        }

        
        //End Drawing - getup index trigger
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            if (lineIdx > 0) //exception handle
            {
                //Debug.Log("up");
                _line[lineIdx - 1].enabled = false;
            }
        }

        //Delete Line
        if (stateRTrigger)
        {
            if (lineIdx > 0) //exception handle
            {
                Destroy(GameObject.Find("line" + --lineIdx));
                _line[lineIdx].DestroyMesh();
                _line.RemoveAt(lineIdx);
            }
        }
    }

    private void StartColorPick()
    {
        Vector3 pos = GameObject.FindWithTag("dotsO").transform.position;
        //pos += GameObject.FindWithTag("rtouch").transform.forward * 1.0f;
        pos += Camera.main.transform.forward * 1.0f;
        Quaternion quat = GameObject.FindWithTag("dotsO").transform.rotation;

        colorPickerGameObject = (GameObject)Instantiate(ColorPicker, pos, quat, Camera.main.transform);
        colorPickerGameObject.transform.localScale = Vector3.one * 0.3f;
        colorPickerGameObject.transform.LookAt(Camera.main.transform);

        colorPickerCmpnt = colorPickerGameObject.GetComponent<ColorPickerTriangle>();
        colorPickerCmpnt.SetNewColor(lineColor); //set by current color
        isColorPick = true;
        
    }

    private void StopColorPick()
    {
        Destroy(colorPickerGameObject);
        isColorPick = false;
    }
}