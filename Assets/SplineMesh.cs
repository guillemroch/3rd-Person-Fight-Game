using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Unity.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace UnityEngine.Splines{
    [ExecuteInEditMode]
    public class SplineMesh : MonoBehaviour{


        [SerializeField] private float extrusionInterval = 10f;
        [SerializeField] private bool smoothFaces = true;
        [SerializeField] private bool useWorldUp = true;

        [SerializeField] private float width;


        private MeshFilter meshFilter;
        private SplineContainer splineContainer;
        private Spline spline;

        private Vector3[] templateVertices;

        private Mesh GenerateMesh() {
            Mesh mesh = new Mesh();
            bool success = SplineSampler.SampleSplineInterval(spline, transform, extrusionInterval,
                out Vector3[] positions, out Vector3[] tangents, out Vector3[] upVectors);
            if (!success) {
                Debug.LogError("SplineMeshExtrude: GenerateMesh: Error encountered when sampling spline. Aborting");
                return mesh;
            }

            List<Vector3> vertices = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            int offset = 0;
            for (int i = 1; i < positions.Length; i++) {
                Vector3 right = Vector3.Cross(tangents[i], upVectors[i]).normalized;
                //Add Vertices 
                vertices.Add(positions[i] + (upVectors[i] * width) + (right * width));
                vertices.Add(positions[i] + (-upVectors[i] * width)+ (right * width));
                vertices.Add(positions[i] + (-upVectors[i] * width)+ (-right * width));
                vertices.Add(positions[i] + (upVectors[i] * width)+ (-right * width));
    
                
                if (i == positions.Length) {
                    right = Vector3.Cross(tangents[0], upVectors[0]).normalized;
                    vertices.Add(positions[0] + (upVectors[0] * width) + (right * width));
                    vertices.Add(positions[0] + (-upVectors[0] * width)+ (right * width));
                    vertices.Add(positions[0] + (-upVectors[0] * width)+ (-right * width));
                    vertices.Add(positions[0] + (upVectors[0] * width) + (-right * width));

                }
                else {
                    right = Vector3.Cross(tangents[i-1], upVectors[i-1]).normalized;
                    vertices.Add(positions[i-1] + (upVectors[i-1] * width) + (right * width));
                    vertices.Add(positions[i-1] + (-upVectors[i-1] * width)+ (right * width));
                    vertices.Add(positions[i-1] + (-upVectors[i-1] * width)+ (-right * width));
                    vertices.Add(positions[i-1] + (upVectors[i-1] * width) + (-right * width));

                }

                offset = 8 * (i - 1);

                //Add triangles
                //Face 1
                tris.Add(offset + 0);
                tris.Add(offset + 5);
                tris.Add(offset + 1);

                tris.Add(offset + 5);
                tris.Add(offset + 0);
                tris.Add(offset + 4);
                
                //Face 2
                tris.Add(offset + 0);
                tris.Add(offset + 7);
                tris.Add(offset + 4);

                tris.Add(offset + 0);
                tris.Add(offset + 3);
                tris.Add(offset + 7);

                //Face 3
                tris.Add(offset + 2);
                tris.Add(offset + 3);
                tris.Add(offset + 7);

                tris.Add(offset + 6);
                tris.Add(offset + 2);
                tris.Add(offset + 7);

                //Face 4
                tris.Add(offset + 2);
                tris.Add(offset + 5);
                tris.Add(offset + 6);

                tris.Add(offset + 2);
                tris.Add(offset + 1);
                tris.Add(offset + 5);
                
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(0, 0));
                
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(0, 1));
            }


            mesh.SetVertices(vertices);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.uv = uvs.ToArray();

            
            

            return mesh;
        }
        internal void Reset()
        {
            

            if (TryGetComponent<MeshFilter>(out var filter))
                filter.sharedMesh = GetComponent<MeshFilter>().mesh = CreateMeshAsset();

           
            Rebuild();
        }

        void Start()
        {


            Rebuild();
        }

        void OnEnable()
        {
            Spline.Changed += OnSplineChanged;
        }

        void OnDisable()
        {
            Spline.Changed -= OnSplineChanged;
        }

        void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
        {

        }

        void Update()
        {
           Rebuild();
        }



        internal Mesh CreateMeshAsset() {
            var mesh = new Mesh();
            mesh.name = name;
            mesh.Equals(new Mesh());

#if UNITY_EDITOR
            var scene = SceneManagement.SceneManager.GetActiveScene();
            var sceneDataDir = "Assets";

            if (!string.IsNullOrEmpty(scene.path)) {
                var dir = Path.GetDirectoryName(scene.path);
                sceneDataDir = $"{dir}/{Path.GetFileNameWithoutExtension(scene.path)}";
                if (!Directory.Exists(sceneDataDir))
                    Directory.CreateDirectory(sceneDataDir);
            }
            
            //Mesh existingMesh = UnityEditor.AssetDatabase.
            
            //if (!UnityEditor.AssetDatabase.FindAssets($"{sceneDataDir}/SplineMesh.asset").Exists( $"{sceneDataDir}/SplineMesh.asset"))
            //{
            
            
                UnityEditor.AssetDatabase.CreateAsset(mesh, $"{sceneDataDir}/SplineMesh.asset");
                UnityEditor.EditorGUIUtility.PingObject(mesh);
            //}
            
#endif
            return mesh;
        }

        private void Rebuild() {

            meshFilter = gameObject.GetComponent<MeshFilter>();
            if (!meshFilter)
                Debug.LogError(
                    $"SplineMeshExtrude: Awake: Gameobject {gameObject.name} does not have an attached mesh filter.");

            splineContainer = gameObject.GetComponent<SplineContainer>();
            spline = splineContainer.Spline;

            Mesh generatedMesh = GenerateMesh();
            meshFilter.mesh = generatedMesh;
        }

        private void OnDrawGizmos() {

            SplineSampler.SampleSplineInterval(spline, transform, extrusionInterval,
                out Vector3[] positions, out Vector3[] tangents, out Vector3[] upVectors);

            Handles.matrix = transform.localToWorldMatrix;
            foreach (var position in positions) {
                Handles.SphereHandleCap(0, position, Quaternion.identity, 1f, EventType.Repaint);
            }
        }
    }
}