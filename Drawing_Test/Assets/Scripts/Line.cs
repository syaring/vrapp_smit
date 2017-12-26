﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class Line : MonoBehaviour
    {
        //dotsO will be none visible in the scene
        //using GUI point's center will be same with dotsO's center
        GameObject dotsO;

        private Vector3 originPoint;      //position value of dotsO
        private Quaternion contRotation;  //rotation value of OVR Controller

        private List<GameObject> _dotsOClone;

        float contState;   //controller state
        public Color originColor;
        public float originWidth;

        private GameObject rHand;

        private Color color;
        private float width;

        //for creating mesh
        int vtxIdx;     //vertex Index
        int inputIdx;   //input Index
        Vector3 v1; Vector3 v2; Vector3 v3; Vector3 v4; //mesh vertex
        GameObject lineMesh;
        List<GameObject> _mesh;

        Renderer rend;

        Vector3 curPos; Vector3 pasPos;
        Vector3 heading;
        float speed = 0;
        Vector3 curDir; Vector3 pasDir;
        static float angle;

        public MeshFilter mf;

        public void Awake()
        {
            vtxIdx = 0;
            inputIdx = 0;

            rHand = GameObject.FindWithTag("rtouch");
            dotsO = GameObject.FindWithTag("dotsO");

            _dotsOClone = new List<GameObject>();

            _mesh = new List<GameObject>();

            rend = GetComponent<Renderer>();

            curPos = Vector3.zero;
            pasPos = Vector3.zero;

            curDir = Vector3.zero;
            pasDir = Vector3.zero;
        }


        public void Update()
        {

            contState = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch);


            contRotation = rHand.transform.rotation;

            //it is more accurate that using dotsO than using controller for standard
            pasPos = curPos;
            curPos = dotsO.transform.position;
     
            heading = curPos - pasPos; //drawing direction
            speed = heading.magnitude;  //drawing speed

            pasDir = curDir;
            curDir = heading;
            //curDir = heading.normalized;

            width = originWidth * (1.01f - speed);

            color = originColor;

            //for comfirmation, speed up, color changed blue
            if (width < originWidth)
                color = new Color(0, 0, 255);

            //it is more accurate when calculate the angle of two vectors
            angle = Vector3.Angle(curDir, pasDir);

            if (angle > 10.0f || angle < -10.0f)
                color = new Color(0, 255, 0);


            if (contState > 0.0f)
            {

                inputIdx++;

                //inputIdx determines the number of meshes
                //inputIdx is smaller, meshes are increase
                if (inputIdx % 3 == 0)
                {

                    _dotsOClone.Add(Instantiate(dotsO, dotsO.transform.position, contRotation));

                    //print(curDir);
                    _mesh.Add(new GameObject());
         
                    if (vtxIdx > 0)
                    {
                        v1 = _dotsOClone[vtxIdx - 1].transform.position - _dotsOClone[vtxIdx - 1].transform.up * width;
                        v2 = _dotsOClone[vtxIdx - 1].transform.position + _dotsOClone[vtxIdx - 1].transform.up * width;
                        v3 = _dotsOClone[vtxIdx].transform.position - _dotsOClone[vtxIdx].transform.up * width;
                        v4 = _dotsOClone[vtxIdx].transform.position + _dotsOClone[vtxIdx].transform.up * width;

                        _mesh[vtxIdx - 1].gameObject.AddComponent<MeshFilter>();
                        _mesh[vtxIdx - 1].gameObject.AddComponent<MeshRenderer>();
                        _mesh[vtxIdx - 1].gameObject.AddComponent<MeshCollider>();

                        rend = _mesh[vtxIdx - 1].GetComponent<Renderer>();
                        //rend.material = new Material(Shader.Find("Transparent/Diffuse"));
                        rend.material = new Material(Shader.Find("UCLA Game Lab/Wireframe/Double-Sided"));
                        rend.material.color = color;
                        
                        //modify later...(hold)
                        _mesh[vtxIdx - 1].GetComponent<MeshFilter>().mesh.vertices = new Vector3[] { v1, v2, v3, v4 };
                        _mesh[vtxIdx - 1].GetComponent<MeshFilter>().mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
                        _mesh[vtxIdx - 1].GetComponent<MeshFilter>().mesh.triangles = new int[] { 0, 1, 2, 3, 2, 1, 2, 1, 0, 1, 2, 3 };

                        

                    }
                    vtxIdx++;
                    
                }
            }

            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                enabled = false;
            }

        }

        public void DestroyMesh()
        {
            //temporary
            for(int i = 0; i < _dotsOClone.Count; i++)
            {
                Destroy(_dotsOClone[i]);
                Destroy(_mesh[i]);
            }
        }
    }
}