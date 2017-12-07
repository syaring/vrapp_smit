using System.Collections;
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
        public Color originColor;
        public float originWidth;

        private GameObject rt;

        private Color color;
        private float width;

        //for creating mesh
        int vtxIdx;     //vertex Index
        int inputIdx;   //input Index
        Vector3 v1; Vector3 v2; Vector3 v3; Vector3 v4; //mesh vertex
        List<GameObject> _mesh;
        
        Renderer rend;

        Vector3 curPos; Vector3 pasPos;
        Vector3 heading;
        float speed = 0;
    
        public void Awake()
        {

            vtxIdx = 0;
            inputIdx = 0;
            rt = GameObject.FindWithTag("rtouch");

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

            curPos = Vector3.zero;
            pasPos = Vector3.zero;
        }


        public void Update()
        {

            contState = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch);


            contRotation = rt.transform.rotation;
            //GameObject.FindWithTag("rtouch").transform.rotation;

            pasPos = curPos;
            curPos = rt.transform.position;
            //curPos = GameObject.FindWithTag("rtouch").transform.position;

            heading = pasPos - curPos;
            speed = heading.magnitude;
            //print(speed);

            if (contState > 0.0f)
            {

                if (speed > 0.01f && inputIdx != 0)
                {
                    color = new Color(0, 0, 255);
                    width = originWidth - speed*0.9f;
                }
                else
                {
                    color = originColor;
                    width = originWidth;
                }

                inputIdx++;
                
                //if(inputIdx % 5 == 0)
                if(inputIdx % 30 == 0)
                {
                    //position of dotsO
                    originPoint = new Vector3(dotsO.transform.position.x, dotsO.transform.position.y, dotsO.transform.position.z);

                    _dotsOClone.Add(Instantiate(dotsO, originPoint, contRotation));
                    _dotsPClone.Add(Instantiate(dotsP, (originPoint + _dotsOClone[vtxIdx].transform.up * width), contRotation));
                    _dotsMClone.Add(Instantiate(dotsM, (originPoint + _dotsOClone[vtxIdx].transform.up * -width), contRotation));

                    print(heading/speed);
                    //sprint(speed);
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