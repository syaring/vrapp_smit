﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class Line : MonoBehaviour
    {
        GameObject dotsO;
        GameObject dotsP;
        GameObject dotsM;

        private Vector3 originPoint;        //position value of dotsO
        private Quaternion contRotation;  //rotation value of OVR Controller

        private List<GameObject> _dotsOClone;
        private List<GameObject> _dotsPClone;
        private List<GameObject> _dotsMClone;

        float contState;   //controller state
        public Color color;
        public float width;

        //for creating mesh
        int vtxIdx;     //vertex Index
        int inputIdx;   //input Index
        Vector3 v1; Vector3 v2; Vector3 v3; Vector3 v4; //mesh vertex
        List<GameObject> _mesh;
        
        Renderer rend;
    
        public void Awake()
        {

            vtxIdx = 0;
            inputIdx = 0;

            dotsO = GameObject.FindWithTag("dotsO");
            dotsP = GameObject.FindWithTag("dotsP");
            dotsM = GameObject.FindWithTag("dotsM");

            _dotsOClone = new List<GameObject>();
            _dotsPClone = new List<GameObject>();
            _dotsMClone = new List<GameObject>();

            _mesh = new List<GameObject>();

            rend = GetComponent<Renderer>();

            //color = new Color(255, 0, 0);   //temporary fixed
            //width = 0.05f;      //temporary fixed
        }


        public void Update()
        {
            contState = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch);

            contRotation = GameObject.FindWithTag("rtouch").transform.rotation;

            if(contState > 0.0f)
            {
                inputIdx++;
                
                if(inputIdx % 5 == 0)
                {
                    //position of dotsO
                    originPoint = new Vector3(dotsO.transform.position.x, dotsO.transform.position.y, dotsO.transform.position.z);

                    _dotsOClone.Add(Instantiate(dotsO, originPoint, contRotation));
                    _dotsPClone.Add(Instantiate(dotsP, (originPoint + _dotsOClone[vtxIdx].transform.up * width), contRotation));
                    _dotsMClone.Add(Instantiate(dotsM, (originPoint + _dotsOClone[vtxIdx].transform.up * -width), contRotation));

                    _mesh.Add(new GameObject());

                    if (vtxIdx > 0)
                    {
                        v1 = _dotsOClone[vtxIdx - 1].transform.position;
                        v2 = _dotsPClone[vtxIdx - 1].transform.position;
                        v3 = _dotsOClone[vtxIdx].transform.position;
                        v4 = _dotsPClone[vtxIdx].transform.position;

                        _mesh[vtxIdx - 1].gameObject.AddComponent<MeshFilter>();
                        _mesh[vtxIdx - 1].gameObject.AddComponent<MeshRenderer>();
                        _mesh[vtxIdx - 1].gameObject.AddComponent<MeshCollider>();

                        rend = _mesh[vtxIdx - 1].GetComponent<Renderer>();

                        rend.material = new Material(Shader.Find("Transparent/Diffuse"));
                        rend.material.color = color;

                        _mesh[vtxIdx - 1].GetComponent<MeshFilter>().mesh.vertices = new Vector3[] { v1, v2, v3, v4 };
                        _mesh[vtxIdx - 1].GetComponent<MeshFilter>().mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
                        _mesh[vtxIdx - 1].GetComponent<MeshFilter>().mesh.triangles = new int[] { 0, 1, 2, 3, 2, 1, 2, 1, 0, 1, 2, 3 };
                    }
                    vtxIdx++;
                }
            }

            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
                enabled = false;
        }

    }
}