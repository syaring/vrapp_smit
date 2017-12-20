﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Assets;
//using Assets.Scripts;

public class DrawingManager : MonoBehaviour
{

    List<Line> _line;
    //GameObject go;
    int lineIdx;
    float lineWidth = 0.05f;
    Color lineColor = new Color(255, 0, 0);

    bool stateDraw;
    bool thumbStateR, thumbStateL;
    Vector2 stateWidth;

    Line line;

    private void Start()
    { 
        lineIdx = 0;
        _line = new List<Line>();
        //go = new GameObject();
    }

    private void Update()
    {
        stateDraw = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        thumbStateR = OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.RTouch);
        thumbStateL = OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.RTouch);


        if (thumbStateR)
        {
            lineWidth += 0.01f;
            //print(lineWidth);
        }
        if (thumbStateL)
        {
            if(lineWidth > 0.0f)
                lineWidth -= 0.01f;
            //print(lineWidth);
        }


        if (Input.GetKeyDown("r"))
            lineColor = new Color(255, 0, 0);
        if (Input.GetKeyDown("g"))
            lineColor = new Color(0, 255, 0);
        if (Input.GetKeyDown("b"))
            lineColor = new Color(0, 0, 255);

        
        if (stateDraw) 
        {
            //drawing line
            GameObject go = new GameObject("line" + lineIdx);
            //Debug.Log("creat" + lineIdx);

            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.AddComponent<MeshCollider>();
            //go.transform.parent = line.transform;
            line = go.AddComponent<Line>();


            line.originColor = lineColor;
            line.originWidth = lineWidth/2;

            _line.Add(line);

            //print("number of line"+_line.Count);
            lineIdx++;
          
        }

        if (Input.GetKeyDown("d"))
        {
            //code for delete line from latest
            Destroy(GameObject.Find("line" + --lineIdx));
            _line[lineIdx].DestroyMesh();
            _line.RemoveAt(lineIdx);
        }

    }
}

    