using System.Collections.Generic;
using System.IO;
using UnityEditor;



namespace UnityEngine.Splines{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(SplineContainer))]
    [RequireComponent((typeof(MeshRenderer)))]

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
                out Vector3[] positions, out Vector3[] forwards, out Vector3[] upVectors);
            if (!success) {
                Debug.LogError("SplineMeshExtrude: GenerateMesh: Error encountered when sampling spline. Aborting");
                return mesh;
            }

            List<Vector3> vertices = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            int offset = 0;
            for (int i = 1; i < positions.Length; i++) {
                Vector3 right = Vector3.Cross(forwards[i], upVectors[i]).normalized;
                //Add Vertices 
                vertices.Add(positions[i] + (upVectors[i] * width) + (right * width));
                vertices.Add(positions[i] + (-upVectors[i] * width)+ (right * width));
                vertices.Add(positions[i] + (-upVectors[i] * width)+ (-right * width));
                vertices.Add(positions[i] + (upVectors[i] * width)+ (-right * width));
    
                
                if (i == positions.Length) {
                    right = Vector3.Cross(forwards[0], upVectors[0]).normalized;
                    vertices.Add(positions[0] + (upVectors[0] * width) + (right * width));
                    vertices.Add(positions[0] + (-upVectors[0] * width)+ (right * width));
                    vertices.Add(positions[0] + (-upVectors[0] * width)+ (-right * width));
                    vertices.Add(positions[0] + (upVectors[0] * width) + (-right * width));

                }
                else {
                    right = Vector3.Cross(forwards[i-1], upVectors[i-1]).normalized;
                    vertices.Add(positions[i-1] + (upVectors[i-1] * width) + (right * width));
                    vertices.Add(positions[i-1] + (-upVectors[i-1] * width)+ (right * width));
                    vertices.Add(positions[i-1] + (-upVectors[i-1] * width)+ (-right * width));
                    vertices.Add(positions[i-1] + (upVectors[i-1] * width) + (-right * width));

                }

                if (Vector3.Dot(forwards[i], forwards[i - 1]) <= 0.1) {
                    
                }

                offset = 8 * (i - 1);

                //Add triangles
                //Face 1
                tris.Add(offset + 0);
                tris.Add(offset + 1);
                tris.Add(offset + 5);

                tris.Add(offset + 0);
                tris.Add(offset + 5);
                tris.Add(offset + 4);
                

                //Face 2
                tris.Add(offset + 4);
                tris.Add(offset + 7);
                tris.Add(offset + 0);

                tris.Add(offset + 7);
                tris.Add(offset + 3);
                tris.Add(offset + 0);

                //Face 3
                tris.Add(offset + 7);
                tris.Add(offset + 2);
                tris.Add(offset + 3);

                tris.Add(offset + 7);
                tris.Add(offset + 6);
                tris.Add(offset + 2);

                //Face 4
                tris.Add(offset + 6);
                tris.Add(offset + 5);
                tris.Add(offset + 2);
                
                tris.Add(offset + 5);
                tris.Add(offset + 1);
                tris.Add(offset + 2);

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
            Rebuild();
        }

        



        private Mesh CreateMeshAsset() {
            var mesh = new Mesh();
            mesh.name = name;
            mesh = GetComponent<MeshFilter>().sharedMesh;

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

            MeshCollider collider = GetComponent<MeshCollider>();
            collider.sharedMesh = generatedMesh;
            CreateMeshAsset();
        }

        private void OnDrawGizmos() {       

            SplineSampler.SampleSplineInterval(spline, transform, extrusionInterval,
                out Vector3[] positions, out Vector3[] tangents, out Vector3[] upVectors);

            Handles.matrix = transform.localToWorldMatrix;
            for (int i = 1; i < positions.Length; i++) {
                Handles.color = Color.yellow;
                Handles.SphereHandleCap(0, positions[i], Quaternion.identity, 1f, EventType.Repaint);
                Handles.color = Color.blue;
                Handles.ArrowHandleCap(0,positions[i],Quaternion.LookRotation(Vector3.up,upVectors[i]), 5f, EventType.Repaint);
                Handles.color = Color.red;
                Handles.ArrowHandleCap(0,positions[i],Quaternion.LookRotation(tangents[i], Vector3.up), 5f, EventType.Repaint);
                Handles.color = Color.green;
                Vector3 right = Vector3.Cross(tangents[i], upVectors[i]).normalized;
                Vector3 direction = positions[i-1] + positions[i];
                Handles.ArrowHandleCap(0,positions[i],Quaternion.LookRotation(tangents[i],direction), 5f, EventType.Repaint);
                Handles.RectangleHandleCap(0,positions[i], Quaternion.LookRotation(tangents[i], upVectors[i]), width, EventType.Repaint);
                Handles.color = Color.magenta;
                Handles.ArrowHandleCap(0,positions[i],Quaternion.LookRotation(right,Vector3.up), 5f, EventType.Repaint);
            }
        }
    }
}