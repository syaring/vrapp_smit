using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Assets;

public class DrawingManager : MonoBehaviour
{

    int lineIdx;
    Line line;
    List<Line> _line;
    
    float lineWidth;
    Color lineColor;

    private bool stateDraw;
    private bool thumbStateR, thumbStateL;   //thumb stick direction
    private bool stateA, stateB;
    private bool stateThumb; //thumb stick button

    //for colorpicker
    public GameObject ColorPicker;
    private ColorPickerTriangle cp;
    private GameObject cpgo;
    private Material mat;
    private bool isPick = false;

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

        thumbStateR = OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.RTouch);
        thumbStateL = OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.RTouch);

        stateA = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch);
   
        stateThumb = OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch);

        if (stateA)
        {
            if (isPick)
                StopColorPick();
            else
                StartColorPick();
        }

        //change line width
        if (thumbStateR)
        {
            lineWidth += 0.01f;
        }
        if (thumbStateL)
        {
            if(lineWidth > 0.0f)
                lineWidth -= 0.01f;
        }

        //change color
        if (isPick)
        {
            lineColor = cp.TheColor;
        }

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
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
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
        //set ColorPicker's position
        Vector3 pos = GameObject.FindWithTag("dotsO").transform.position;
        pos += Camera.main.transform.forward * 1.0f;
        //pos += GameObject.FindWithTag("rtouch").transform.forward * 1.0f;
        Quaternion quat = GameObject.FindWithTag("dotsO").transform.rotation;

        //create ColorPicker Instance
        cpgo = (GameObject)Instantiate(ColorPicker, pos, quat, Camera.main.transform);
        cpgo.transform.localScale = Vector3.one * 0.3f;
        cpgo.transform.LookAt(Camera.main.transform);

        cp = cpgo.GetComponent<ColorPickerTriangle>();
        cp.SetNewColor(lineColor); //set by current color
        isPick = true;
        
        //GetColorInfo from ColorPickerTriangle
    }

    private void StopColorPick()
    {
        Destroy(cpgo);
        isPick = false;
    }
}