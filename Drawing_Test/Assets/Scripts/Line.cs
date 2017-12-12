using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class Line : MonoBehaviour
    {
        //dotsO, dotsP, dotsM will be none visible in the scene
        //using GUI point's center will be same with dotsO's center
        GameObject dotsO;
        GameObject dotsP;
        GameObject dotsM;

        private Vector3 originPoint;      //position value of dotsO
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
        Vector3 curDir; Vector3 pasDir;
        static float angle;

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

            curDir = Vector3.zero;
            pasDir = Vector3.zero;
        }


        public void Update()
        {

            contState = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch);


            contRotation = rt.transform.rotation;

            //it is more accurate that using dotsO than using controller for standard
            pasPos = curPos;
            curPos = dotsO.transform.position;
            //curPos = rt.transform.position;

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
            
            //for comfirmation, direction changed, color changed green ... not so much accurate
            //if (curDir.x * pasDir.x <= 0 || curDir.y * pasDir.y <= 0 || curDir.z * pasDir.z <= 0)
            
            //it is more accurate when calculate the angle of two vectors
            angle = Vector3.Angle(curDir, pasDir);
        
            if( angle > 10.0f)
                color = new Color(0, 255, 0);

           
            if (contState > 0.0f)
            {
                
                inputIdx++;
                
                if(inputIdx % 3 == 0)
                {
                    //print(curDir);
                    //position of dotsO
                    originPoint = new Vector3(dotsO.transform.position.x, dotsO.transform.position.y, dotsO.transform.position.z);

                    _dotsOClone.Add(Instantiate(dotsO, originPoint, contRotation));
                    _dotsPClone.Add(Instantiate(dotsP, (originPoint + _dotsOClone[vtxIdx].transform.up * width), contRotation));
                    _dotsMClone.Add(Instantiate(dotsM, (originPoint + _dotsOClone[vtxIdx].transform.up * -width), contRotation));

                    //print(curDir);
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

                        //modify later...(hold)
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