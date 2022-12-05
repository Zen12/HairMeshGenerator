using System;
using System.Collections.Generic;
using System.Linq;
using BezierSolution;
using NaughtyAttributes;
using UnityEngine;

namespace _Project.Scripts.MeshGen
{
    [ExecuteAlways]
    public class PrepareForBuild : MonoBehaviour
    {
        [Header("Horizontal")]
        public BezierSpline First;
        public BezierSpline Second;
        public BezierSpline Third;
        public BezierSpline Fourth;
        
        [Header("Vertical")]
        public BezierSpline FirstVert;
        public BezierSpline SecondVert;
        public BezierSpline ThirdVert;
        public BezierSpline FourthVert;

        public float smoothTime = 0.9f;
        
        public bool AutoUpdate = false;


        [Button]
        public void ResetHair()
        {
            var allBranches = transform.parent.GetComponentsInChildren<HairGeneratorV2>();
            var targets1 = new List<Transform>();
            var targets2 = new List<Transform>();
            var targets3 = new List<Transform>();
            var targets4 = new List<Transform>();



            foreach (var treeBranchV2 in allBranches)
            {
                var allPoints = treeBranchV2.GetComponentsInChildren<BezierPoint>();
                targets1.Add(allPoints[0].transform);
                targets2.Add(allPoints[1].transform);
                targets3.Add(allPoints[2].transform);
                targets4.Add(allPoints[3].transform);
            }

            var points = First.GetComponentsInChildren<BezierPoint>();
            
            var bezierOffset = new Vector3(-0.1106853f, 0.2591437f, -0.03656287f);

            foreach (var po in points)
            {
                po.precedingControlPointLocalPosition = bezierOffset;
            }
            
            
            points[0].position = transform.position + Vector3.right * 0- Vector3.up * 0f + Vector3.forward * 0f;
            points[1].position = transform.position + Vector3.right * 1- Vector3.up * 0f + Vector3.forward * 0f;
            points[2].position = transform.position + Vector3.right * 2- Vector3.up * 0f + Vector3.forward * 0f;
            points[3].position = transform.position + Vector3.right * 3- Vector3.up * 0f + Vector3.forward * 0f;
            
            points = Second.GetComponentsInChildren<BezierPoint>();
            foreach (var po in points)
            {
                po.precedingControlPointLocalPosition =  bezierOffset;
            }

            points[0].position = transform.position + Vector3.right * 0 - Vector3.up * 1f + Vector3.forward * 1f;
            points[1].position = transform.position + Vector3.right * 1 - Vector3.up * 1f + Vector3.forward * 1f;
            points[2].position = transform.position + Vector3.right * 2 - Vector3.up * 1f + Vector3.forward * 1f;
            points[3].position = transform.position + Vector3.right * 3 - Vector3.up * 1f + Vector3.forward * 1f;
            
            points = Third.GetComponentsInChildren<BezierPoint>();
            foreach (var po in points)
            {
                po.precedingControlPointLocalPosition =  bezierOffset;
            }
            points[0].position = transform.position + Vector3.right * 0 - Vector3.up * 2+ Vector3.forward * 2f;
            points[1].position = transform.position + Vector3.right * 1 - Vector3.up * 2+ Vector3.forward * 2f;
            points[2].position = transform.position + Vector3.right * 2 - Vector3.up * 2+ Vector3.forward * 2f;
            points[3].position = transform.position + Vector3.right * 3 - Vector3.up * 2+ Vector3.forward * 2f;
            
            points = Fourth.GetComponentsInChildren<BezierPoint>();
            foreach (var po in points)
            {
                po.precedingControlPointLocalPosition = bezierOffset;
            }
            
            points[0].position = transform.position + Vector3.right * 0 - Vector3.up * 3+ Vector3.forward * 3f;
            points[1].position = transform.position + Vector3.right * 1 - Vector3.up * 3+ Vector3.forward * 3f;
            points[2].position = transform.position + Vector3.right * 2 - Vector3.up * 3+ Vector3.forward * 3f;
            points[3].position = transform.position + Vector3.right * 3 - Vector3.up * 3+ Vector3.forward * 3f;


            var secondOffset = new Vector3(-0.09779126f,
                -0.295617f,
                -0.05467199f);
            points = FirstVert.GetComponentsInChildren<BezierPoint>();
            foreach (var po in points)
            {
                po.precedingControlPointLocalPosition =  secondOffset;
            }
            points = SecondVert.GetComponentsInChildren<BezierPoint>();
            foreach (var po in points)
            {
                po.precedingControlPointLocalPosition =  secondOffset;
            }
            points = ThirdVert.GetComponentsInChildren<BezierPoint>();
            foreach (var po in points)
            {
                po.precedingControlPointLocalPosition =  secondOffset;
            }
            points = FourthVert.GetComponentsInChildren<BezierPoint>();
            foreach (var po in points)
            {
                po.precedingControlPointLocalPosition =  secondOffset;
            }
        }
        
        [Button]
        private void Build()
        {
            var allBranches = transform.parent.GetComponentsInChildren<HairGeneratorV2>();
            var targets1 = new List<Transform>();
            var targets2 = new List<Transform>();
            var targets3 = new List<Transform>();
            var targets4 = new List<Transform>();



            foreach (var treeBranchV2 in allBranches)
            {
                var allPoints = treeBranchV2.GetComponentsInChildren<BezierPoint>();
                targets1.Add(allPoints[0].transform);
                targets2.Add(allPoints[1].transform);
                targets3.Add(allPoints[2].transform);
                targets4.Add(allPoints[3].transform);
            }


            var currentStep = 0f;
            var step = 1f / (targets1.Count);
            
            foreach (var transform1 in targets1)
            {
                V2(transform1, currentStep, First, 0);
                currentStep += step;
            }

            currentStep = 0f;
            
                        
            foreach (var transform1 in targets2)
            {
                V2(transform1, currentStep, Second, 1);
                currentStep += step;
            }

            currentStep = 0f;

            
            foreach (var transform1 in targets3)
            {
                V2(transform1, currentStep, Third, 2);
                currentStep += step;
            }
            
            currentStep = 0f;

            
            foreach (var transform1 in targets4)
            {
                V2(transform1, currentStep, Fourth, 3);
                currentStep += step;
            }
            
            
            //lerp bezier
            for (int i = 1; i < targets1.Count; i++)
            {
                var transform1 = targets1[i];
                var transform1Previous = targets1[i - 1];
                var point = transform1.GetComponent<BezierPoint>();
                var pointPrevious = transform1Previous.GetComponent<BezierPoint>();
                point.precedingControlPointPosition = Vector3.Lerp(pointPrevious.precedingControlPointPosition,
                    point.precedingControlPointPosition, smoothTime);
                point.followingControlPointPosition = Vector3.Lerp(pointPrevious.followingControlPointPosition,
                    point.followingControlPointPosition, smoothTime);

                //transform1.position = Vector3.Lerp(transform1Previous.position, transform1.position, smoothTime);
            }

                        
            for (int i = 1; i < targets2.Count; i++)
            {
                var transform1 = targets2[i];
                var transform1Previous = targets2[i - 1];
                var point = transform1.GetComponent<BezierPoint>();
                var pointPrevious = transform1Previous.GetComponent<BezierPoint>();
                point.precedingControlPointPosition = Vector3.Lerp(pointPrevious.precedingControlPointPosition,
                    point.precedingControlPointPosition, smoothTime);
                point.followingControlPointPosition = Vector3.Lerp(pointPrevious.followingControlPointPosition,
                    point.followingControlPointPosition, smoothTime);
                
                //transform1.position = Vector3.Lerp(transform1Previous.position, transform1.position, smoothTime);

            }


                        
            for (int i = 1; i < targets3.Count; i++)
            {
                var transform1 = targets3[i];
                var transform1Previous = targets3[i - 1];
                var point = transform1.GetComponent<BezierPoint>();
                var pointPrevious = transform1Previous.GetComponent<BezierPoint>();
                point.precedingControlPointPosition = Vector3.Lerp(pointPrevious.precedingControlPointPosition,
                    point.precedingControlPointPosition, smoothTime);
                point.followingControlPointPosition = Vector3.Lerp(pointPrevious.followingControlPointPosition,
                    point.followingControlPointPosition, smoothTime);
                
                //transform1.position = Vector3.Lerp(transform1Previous.position, transform1.position, smoothTime);

            }


                        
            for (int i = 1; i < targets4.Count; i++)
            {
                var transform1 = targets4[i];
                var transform1Previous = targets4[i - 1];
                var point = transform1.GetComponent<BezierPoint>();
                var pointPrevious = transform1Previous.GetComponent<BezierPoint>();
                point.precedingControlPointPosition = Vector3.Lerp(pointPrevious.precedingControlPointPosition,
                    point.precedingControlPointPosition, smoothTime);
                point.followingControlPointPosition = Vector3.Lerp(pointPrevious.followingControlPointPosition,
                    point.followingControlPointPosition, smoothTime);
                
                //transform1.position = Vector3.Lerp(transform1Previous.position, transform1.position, smoothTime);

            }
            
        }

        private void V2(Transform transform1, float currentStep, BezierSpline spline, int index)
        {
            transform1.position = spline.GetPoint(currentStep);

            var segment = spline.GetSegmentAt(currentStep);
            var lerpTime = segment.localT;
            var vertIndex = spline.FindIndex(segment.point1);

            var hor1 = FirstVert[0];
            var hor2 = SecondVert[1];

            if (vertIndex == 0)
            {
                hor1 = FirstVert[index];
                hor2 = SecondVert[index];
            }

            if (vertIndex == 1)
            {
                hor1 = SecondVert[index];
                hor2 = ThirdVert[index];
            }

            if (vertIndex == 2)
            {
                hor1 = ThirdVert[index];
                hor2 = FourthVert[index];
            }
            
            var t1 = Vector3.Lerp(hor1.precedingControlPointPosition, hor2.precedingControlPointPosition, lerpTime);
            var t2 = Vector3.Lerp(hor1.followingControlPointPosition, hor2.followingControlPointPosition, lerpTime);
            var magn1 = hor2.precedingControlPointPosition.magnitude;
            var magn2 = hor2.followingControlPointPosition.magnitude;


            var point = transform1.GetComponent<BezierPoint>();
            point.precedingControlPointPosition = t1.normalized * magn1;
            point.followingControlPointPosition = t2.normalized * magn2;

            var target = transform1.GetComponent<PointVariables>();
            var ref1 = hor1.GetComponent<PointVariables>();
            var ref2 = hor2.GetComponent<PointVariables>();
            target.SetLerp(ref1, ref2, lerpTime);
        }

        private void Update()
        {
            FirstVert[0].position = First[0].position;
            FirstVert[1].position = Second[0].position;
            FirstVert[2].position = Third[0].position;
            FirstVert[3].position = Fourth[0].position;
            
            SecondVert[0].position = First[1].position;
            SecondVert[1].position = Second[1].position;
            SecondVert[2].position = Third[1].position;
            SecondVert[3].position = Fourth[1].position;
            
            ThirdVert[0].position = First[2].position;
            ThirdVert[1].position = Second[2].position;
            ThirdVert[2].position = Third[2].position;
            ThirdVert[3].position = Fourth[2].position;

                
            FourthVert[0].position = First[3].position;
            FourthVert[1].position = Second[3].position;
            FourthVert[2].position = Third[3].position;
            FourthVert[3].position = Fourth[3].position;
            
            if (AutoUpdate == false)
                return;
            Build();

        }

    }

    public static class BezierSplineExtension
    {
        public static int FindIndex(this BezierSpline spline, BezierPoint p)
        {
            for (int i = 0; i < spline.Count; i++)
            {
                if (spline[i] == p)
                    return i;
            }

            return -1;
        }
    }
}
