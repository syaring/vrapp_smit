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

    static bool stateDraw;
    static bool thumbStateR, thumbStateL;

    
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
        if (Input.GetKeyDown("r"))
            lineColor = new Color(255, 0, 0);
        else if (Input.GetKeyDown("g"))
            lineColor = new Color(0, 255, 0);
        else if (Input.GetKeyDown("b"))
            lineColor = new Color(0, 0, 255);


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
}

    