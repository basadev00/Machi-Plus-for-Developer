using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
//中身はPLATEAU LOD1 UVmap Generator と一緒
public static class UVloadPreparation_Utilities
{
    public static float MakeUVdata(GameObject go, MeshRenderer meshRenderer, MeshFilter meshFilter)
    {
        Undo.RecordObject(meshRenderer, "Optimize Mesh");
        Undo.RecordObject(meshFilter, "Optimize Mesh");

        // Add the Processed component to the GameObject
        //Undo.AddComponent<Processed>(go);

        // make new instance of mesh
        meshFilter.sharedMesh = CreateMeshInstance(meshFilter.sharedMesh, false);

        // unwrap uvs as best fit around edge of building
        float uvZoom = UnwrapUVs(meshFilter.sharedMesh, 100);



        // select bottom triangles
        List<int> listofBottomTriangles = SelectFacesFacingDown(meshFilter.sharedMesh, 15);

        // delete bottom triables
        List<System.Tuple<int, int>> remainingEdges = DeleteSelectedTriangles(meshFilter.sharedMesh, SelectFacesFacingDown(meshFilter.sharedMesh, 15));

        // set base color to green for Windows
        SetTriangleVertexColors(meshFilter.sharedMesh, GetAllTriangles(meshFilter.sharedMesh), Color.green);

        //meshFilter.sharedMesh.RecalculateNormals();
        meshFilter.sharedMesh.Optimize();
        meshFilter.mesh = meshFilter.sharedMesh;

        // get height of building
        float height = GetMeshHeight(meshFilter.sharedMesh);

        return uvZoom;
    }

    public static Mesh CreateMeshInstance(Mesh inputMesh, bool recalculateNormals)
    {
        var newInstance = new Mesh
        {
            vertices = inputMesh.vertices,
            triangles = inputMesh.triangles,
            uv = inputMesh.uv,
            normals = inputMesh.normals,
            tangents = inputMesh.tangents,
            colors = inputMesh.colors
        };

        // Make sure the new mesh is properly set up
        newInstance.RecalculateBounds();
        if (recalculateNormals)
        {
            newInstance.RecalculateNormals();
        }

        newInstance.Optimize();

        return newInstance;
    }



    // Select faces facing down within a tolerance angle
    public static List<int> SelectFacesFacingDown(Mesh mesh, float toleranceAngle)
    {
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;
        var selectedFaces = new List<int>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 faceNormal = (normals[triangles[i]] + normals[triangles[i + 1]] + normals[triangles[i + 2]]) /
                                 3f;

            float angle = Vector3.Angle(Vector3.down, faceNormal);
            if (angle <= toleranceAngle)
            {
                selectedFaces.Add(triangles[i]);
                selectedFaces.Add(triangles[i + 1]);
                selectedFaces.Add(triangles[i + 2]);
            }
        }

        return selectedFaces;
    }




    // Change the vertex color of the specified triangles, retaining the original vertex colors of other triangles
    public static void SetTriangleVertexColors(Mesh mesh, List<int> selectedTriangles, Color color)
    {
        int[] triangles = mesh.triangles;
        Color[] originalColors = mesh.colors;

        if (originalColors.Length == 0)
        {
            originalColors = new Color[mesh.vertexCount];
            for (int i = 0; i < originalColors.Length; i++)
            {
                originalColors[i] = Color.white;
            }
        }

        var newColors = (Color[])originalColors.Clone();

        foreach (int vertexIndex in selectedTriangles)
        {
            newColors[vertexIndex] = color;
        }

        mesh.colors = newColors;
    }


    public static float UnwrapUVs(Mesh mesh, float tileSize)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        var uvs = new Vector2[vertices.Length];

        //図面間位置調整用]
        float uSideBase = 0.0f;
        float uSideShift = 0.0f;

        float vSideBase = 0.0f;

        float uMinX = 1000.0f;
        float vMaxY = 0.0f;
        float vMaxZ = 0.0f;
        float vMinZ = 1000.0f;
        float vTopBase = 0.0f;

        //上面のベースのuv地点を確定
        //for (int i = 0; i < vertices.Length; i += 3)
        for (int i = 0; i < triangles.Length; i++)
        {
            uMinX = System.Math.Min(uMinX, vertices[triangles[i]].x / tileSize);

            vMaxY = System.Math.Max(vMaxY, vertices[triangles[i]].y / tileSize);
            vMaxZ = System.Math.Max(vMaxZ, vertices[triangles[i]].z / tileSize);
            vMinZ = System.Math.Min(vMinZ, vertices[triangles[i]].z / tileSize);
            vTopBase = vMaxY + Math.Abs((vMaxZ - vMinZ));
        }

        //TriangleからUV作成
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            Vector3 edge1 = v1 - v0;
            Vector3 edge2 = v2 - v0;

            Vector3 normal = Vector3.Cross(edge1, edge2).normalized;


            // Check if the face is a roof (Y-axis)
            if (Mathf.Abs(normal.y) > 0.9f)
            {
                uvs[triangles[i]] = new Vector2(v0.x / tileSize - uMinX, vMaxY + (v0.z / tileSize - vMinZ));
                uvs[triangles[i + 1]] = new Vector2(v1.x / tileSize - uMinX, vMaxY + (v1.z / tileSize - vMinZ));
                uvs[triangles[i + 2]] = new Vector2(v2.x / tileSize - uMinX, vMaxY + (v2.z / tileSize - vMinZ));


                vSideBase = Math.Max(Math.Max(vSideBase, uvs[triangles[i]].y), Math.Max(uvs[triangles[i + 1]].y, uvs[triangles[i + 2]].y));
            }
            else
            {
                Vector3 uAxis, vAxis;

                if (Mathf.Abs(normal.x) > 0.9f)
                {
                    //細い面（Z面）
                    uAxis = new Vector3(0, 0, 1);
                    vAxis = new Vector3(0, 1, 0);
                }
                else
                {
                    //広い面（X面）
                    uAxis = new Vector3(1, 0, 0);
                    vAxis = new Vector3(0, 1, 0);
                }



                float uCoord0 = Vector3.Dot(v0, uAxis) / tileSize;
                float uCoord1 = Vector3.Dot(v1, uAxis) / tileSize;
                float uCoord2 = Vector3.Dot(v2, uAxis) / tileSize;

                float vCoord0 = Vector3.Dot(v0, vAxis) / tileSize;
                float vCoord1 = Vector3.Dot(v1, vAxis) / tileSize;
                float vCoord2 = Vector3.Dot(v2, vAxis) / tileSize;

                //四角間u調整計算
                if (uvs[triangles[i + 1]] == Vector2.zero)
                {
                    uSideBase = uSideBase + uSideShift;
                }

                //左右反転しているTraiangle１個目
                if (vCoord1 == vCoord2 && uCoord1 - uCoord2 < 0)
                {
                    //左右反転修正
                    uSideShift = uCoord2 - uCoord1;
                    uvs[triangles[i]] = new Vector2(uSideShift + uSideBase, vCoord0);
                    uvs[triangles[i + 1]] = new Vector2(uSideShift + uSideBase, vCoord1);
                    uvs[triangles[i + 2]] = new Vector2(uSideBase, vCoord2);
                }
                //左右反転しているTraiangle２個目
                else if (vCoord0 == vCoord1 && uCoord1 - uCoord2 < 0)
                {
                    //左右反転修正
                    uvs[triangles[i]] = new Vector2(uSideBase, vCoord0);

                }
                //Traiangle（反転なし）
                else
                {
                    uSideShift = uCoord1 - uCoord2;

                    //triangle２個目
                    if (vCoord0 == vCoord1)
                    {
                        uvs[triangles[i]] = new Vector2(uSideBase, vCoord0);
                    }
                    //triangle１個目
                    else
                    {
                        uvs[triangles[i]] = new Vector2(uSideShift + uSideBase, vCoord0);
                    }
                    uvs[triangles[i + 1]] = new Vector2(uSideShift + uSideBase, vCoord1);
                    uvs[triangles[i + 2]] = new Vector2(uSideBase, vCoord2);
                }




            }

        }


        // UV座標の最大値を探す
        float maxValue = 0f;

        //最後の面の横長さを足し合わせる
        uSideBase = uSideBase + uSideShift;

        if (uSideBase > maxValue) maxValue = uSideBase;
        if (vSideBase > maxValue && uSideBase < vSideBase) maxValue = vSideBase;

        // 最大値が1(UV範囲の最大値)を超えている場合のみスケーリングを行う
        float scale = 0f;
        if (maxValue > 1)
        {
            scale = 1 / maxValue;
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = uvs[i] * scale;
            }
        }
        else
        {
            scale = 1 / Mathf.Max(maxValue, 0.1f);
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = uvs[i] * scale;
            }
        }

        //UnityEngine.Debug.Log("UvZoom =" + scale);

        // UV座標をメッシュに適用
        mesh.uv = uvs;


        return scale;
    }


    // Get a list of all the triangles in the mesh
    public static List<int> GetAllTriangles(Mesh mesh)
    {
        int[] triangles = mesh.triangles;
        var allTriangles = new List<int>(triangles);

        return allTriangles;
    }

    // Delete selected triangles from a mesh and return a list of boundary edges left behind
    public static List<Tuple<int, int>> DeleteSelectedTriangles(Mesh mesh, List<int> selectedTriangles)
    {
        int[] triangles = mesh.triangles;
        var newTriangles = new List<int>(triangles.Length - selectedTriangles.Count);
        var edgeCount = new Dictionary<Tuple<int, int>, int>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            var currentEdges = new List<Tuple<int, int>>
                {
                    new Tuple<int, int>(triangles[i], triangles[i + 1]),
                    new Tuple<int, int>(triangles[i + 1], triangles[i + 2]),
                    new Tuple<int, int>(triangles[i + 2], triangles[i])
                };

            if (selectedTriangles.Contains(triangles[i]) ||
                selectedTriangles.Contains(triangles[i + 1]) ||
                selectedTriangles.Contains(triangles[i + 2]))
            {
                foreach (Tuple<int, int> edge in currentEdges)
                {
                    if (edgeCount.ContainsKey(edge))
                    {
                        edgeCount[edge]++;
                    }
                    else
                    {
                        var reverseEdge = new Tuple<int, int>(edge.Item2, edge.Item1);
                        if (edgeCount.ContainsKey(reverseEdge))
                        {
                            edgeCount[reverseEdge]++;
                        }
                        else
                        {
                            edgeCount[edge] = 1;
                        }
                    }
                }
            }
            else
            {
                newTriangles.AddRange(currentEdges.Select(edge => edge.Item1));
            }
        }

        mesh.triangles = newTriangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        var boundaryEdges = edgeCount
            .Where(pair => pair.Value == 1)
            .Select(pair => pair.Key)
            .ToList();

        return boundaryEdges;
    }

    // Get Height of Mesh bounds
    public static float GetMeshHeight(Mesh mesh)
    {
        Bounds meshBounds = mesh.bounds;
        return meshBounds.size.y;
    }

}
#endif  
