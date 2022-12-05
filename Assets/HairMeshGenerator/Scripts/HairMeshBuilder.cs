using System.Collections.Generic;
using BezierSolution;
using NaughtyAttributes;
using UnityEngine;


public class HairMeshBuilder : MonoBehaviour
{
    public Transform Root;
    public Transform BoneRoot;
    [SerializeField] private HairBranchConfig _config;


    [Button]
    public void Generate()
    {
        foreach (var tr in BoneRoot.GetComponentsInChildren<Transform>())
        {
            if (tr != transform)
            {
                DestroyImmediate(tr.gameObject);
            }
        }

        var allBezier = Root.GetComponentsInChildren<BezierSpline>();

        var skinned = GetComponent<MeshFilter>();
        var mesh = skinned.sharedMesh;

        if (mesh == null)
        {
            mesh = new Mesh();
        }

        var verts = new List<Vector3>();
        var indexes = new List<int>();
        var uvs = new List<Vector2>();

        var bones = new List<Transform>();

        var mats = new List<Matrix4x4>();
        var boneWeights = new List<BoneWeight>();

        foreach (var spline in allBezier)
        {
            if (spline.transform.parent != Root)
                continue;
            var rootBone = new GameObject().transform;
            rootBone.SetParent(spline.transform);
            rootBone.position = spline.GetPoint(0);
            GenerateMesh(_config, spline, rootBone, verts, indexes, uvs, bones, mats, boneWeights);
        }

        mesh.SetVertices(verts);
        mesh.SetIndices(indexes, MeshTopology.Triangles, 0);
        mesh.SetUVs(0, uvs);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        
        skinned.sharedMesh = mesh;

        return;
        mesh.boneWeights = boneWeights.ToArray();
        mesh.bindposes = mats.ToArray();
    }

    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
    }

    public static void GenerateMesh(HairBranchConfig config, BezierSpline spline, Transform rootBone,
        List<Vector3> verts,
        List<int> indeses,
        List<Vector2> uv1,
        
        List<Transform> bones,
        List<Matrix4x4> bindPoses,
        List<BoneWeight> boneWeights
        )
    {

        var angle = 0f;
        var center = spline.GetPoint(0);




        float currentStep = 0f;
        var currentBoneStep = 0f;
        var tangent = spline.GetTangent(0f);

        for (int i = 0; i < rootBone.transform.childCount; i++)
        {
            var c = rootBone.transform.GetChild(i);
            DestroyImmediate(c.gameObject);
        }

        rootBone.transform.position = spline.GetPoint(0f);
        var currentBone = new GameObject($"Bone {bones.Count}").transform;
        currentBone.parent = rootBone.transform;

        var currentVertex = verts.Count;
        var startVert = verts.Count;
        var startTime = 0f;
        
        bones.Add(currentBone);

        
        while (currentStep < 1f + 0.1)
        {
            angle += 360f / config.widthSegments;
            var segment = spline.GetSegmentAt(currentStep);
            var p1 = segment.point1.gameObject.GetComponent<PointVariables>();
            var p2 = segment.point2.gameObject.GetComponent<PointVariables>();
            center = spline.GetPoint(currentStep);

            var relativeLerp = InverseLerp(p1.transform.position, p2.transform.position, center);
            var radiusX = Mathf.Lerp(p1.Radius1, p2.Radius1, relativeLerp);
            var radiusY = Mathf.Lerp(p1.Radius2, p2.Radius2, relativeLerp);
            
            var rotLerp = Mathf.Lerp(p1.rotationLerpPower, p2.rotationLerpPower, relativeLerp);
            
            tangent = Vector3.Lerp(tangent, spline.GetTangent(currentStep), rotLerp);

            var rotateStep = 1f / config.widthSegments;
            var currentRotate = 0f;


            const float maxAngle = 180f;
            
            while (angle <= maxAngle)
            {
                var pos = EllipseEdge(center, radiusX, radiusY, angle, tangent);
                
                uv1.Add(new Vector2(currentRotate, 1 - currentStep));

                currentRotate += rotateStep;
                verts.Add(pos);

                if (currentStep < 0.99f)
                {
                    if (angle <= maxAngle - maxAngle / config.widthSegments)
                    {
                        {
                            indeses.Add(currentVertex + config.widthSegments);
                            indeses.Add(currentVertex);
                            indeses.Add(currentVertex + 1);
                        }
                        if (angle <= maxAngle - maxAngle / config.widthSegments * 2)
                        {
                            indeses.Add(currentVertex + config.widthSegments + 1);

                            indeses.Add(currentVertex + config.widthSegments);
                            indeses.Add(currentVertex + 1);
                        }
                    }
                }

                angle += maxAngle / config.widthSegments;
                currentVertex++;
            }

            angle = 0f;

            var timeStep = 0f;
            while (true)
            {
                timeStep += 0.001f;
                var nextPos = spline.GetPoint(currentStep + timeStep);
                var dist = Vector3.Distance(center, nextPos);
                if (dist > config.step)
                {
                    break;
                }

                if (timeStep > 1)
                {
                    timeStep = 0.1f;
                    break;
                }
                
            }
            currentStep += timeStep;
            currentBoneStep += timeStep;
            
            if (currentBoneStep >= config.boneStep)
            {
                var lastBone = currentBone;

                var p = spline.GetPoint(startTime);
                var dir = spline.GetPoint(currentBoneStep) - p;
                dir.Normalize();
                    
                currentBoneStep = 0f;

                lastBone.position = p;
                lastBone.rotation = Quaternion.LookRotation(-dir);
                
                
                bindPoses.Add(currentBone.worldToLocalMatrix);

                currentBone = new GameObject($"Bone {bones.Count}").transform;
                currentBone.parent = lastBone;
                bones.Add(currentBone);

                var step = 1.0f / (float) (currentVertex - startVert);
                var currentStepBone = 0f;
                
                for (int i = startVert; i < currentVertex; i++)
                {
                    if (bones.Count - 3 > 0)
                    {
                        boneWeights.Add(new BoneWeight
                        {
                            weight0 = currentStepBone,
                            boneIndex0 = bones.Count - 2,

                            weight1 = 1 - currentStepBone,
                            boneIndex1 = bones.Count - 3,
                        });
                    }
                    else
                    {
                        boneWeights.Add(new BoneWeight
                        {
                            weight0 = 1f,
                            boneIndex0 = bones.Count - 2,
                        });
                    }

                    currentStepBone += step;
                }

                startTime = currentStep;
                startVert = currentVertex;
            }
        }
        
    }

    public static Vector3 GetPosFromCircle(Vector3 center, float radius, float theta)
    {
        var x = Mathf.Cos(theta * Mathf.Deg2Rad) * radius + center.x;
        var y = center.y;
        var z = Mathf.Sin(theta * Mathf.Deg2Rad) * radius + center.z;
        return new Vector3(x, y, z);
    }
    
    public static Vector3 GetPosFromEllipse(Vector3 center, float radiusX, float radiusY, float theta)
    {
        var x = Mathf.Cos(theta) * radiusX;
        var y = Mathf.Sin(theta) * radiusY;
        var z = (Mathf.Cos(theta) * radiusX) * (Mathf.Sin(theta) * radiusY);
        return new Vector3(x, y, z) + center;
    }
    
    public static Vector3 EllipseEdge(Vector3 position, float radius1, float radius2, float alpha, Vector3 normal)
    {
        float tilt = 0;

        
        var v1 = new Vector3(0f + (radius1 * MCos(alpha) * MCos(tilt)) - ( radius2 * MSin(alpha) * MSin(tilt)),
            0f + (radius1 * MCos(alpha) * MSin(tilt)) + ( radius2 * MSin(alpha) * MCos(tilt)), 0);

        v1 = Quaternion.LookRotation(normal) * v1;
        
        return (v1 + position);
    }
    
    public static float MCos(float value)
    {
        return Mathf.Cos(Mathf.Deg2Rad * value);
    }

    public static float MSin(float value)
    {
        return Mathf.Sin(Mathf.Deg2Rad * value);
    }
}