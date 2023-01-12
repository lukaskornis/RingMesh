using UnityEditor;
using UnityEngine;

namespace SimVik
{
    /// <summary>
    ///     An optimised ring mesh generator with a given radius, thickness, angle and detail.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class OptiRingMesh : MonoBehaviour
    {
        [Min(0f)] public float radius = 5f;
        [Min(0f)][Tooltip("Inwards wall thickness")] public float thickness = 1f;
        [Min(3)] public int segments = 12;
        [Tooltip("How many times texture will repeat along the circle")][Min(1)] public int uvScale = 1;
        [Range(0f, 360f)] public float angleRange = 360f;
        public bool useXZPlane = true;
        [Tooltip("Update on value change during play mode. Can be performance taxing!")]
        public bool autoUpdate;

        private MeshFilter _meshFilter;
        private Mesh _mesh;

        private Vector3[] _vertices;
        private Vector2[] _uvs;
        private int[] _triangles;

        private float _lastRadius;
        private float _lastThickness;
        private int _lastSegments;
        private float _lastAngleRange;
        private float _lastUvScale;
        private bool _lastUseXZPlane;


        private void Update()
        {
            if (autoUpdate) DetermineAction();
        }


        private void DetermineAction()
        {
            // generate mesh if empty or segments have changed
            if (_mesh == null || segments != _lastSegments || uvScale != _lastUvScale || useXZPlane != _lastUseXZPlane)
            {
                GenerateMesh();
                _lastSegments = segments;
                _lastUvScale = uvScale;
                _lastUseXZPlane = useXZPlane;
            }
            // resize mesh if radius or thickness has changed
            else if (radius != _lastRadius || thickness != _lastThickness)
            {
                ResizeMesh();
                _lastRadius = radius;
                _lastThickness = thickness;
            }
            // update mesh if angle range has changed
            else if (angleRange != _lastAngleRange)
            {
                HideMeshByAngle();
                _lastAngleRange = angleRange;
            }
        }


        /// <summary>
        ///     generates ring mesh completely from scratch
        /// </summary>
        public void GenerateMesh()
        {
            // allocate memory for arrays
            _vertices = new Vector3[segments * 2];
            _uvs = new Vector2[segments * 2];
            _triangles = new int[segments * 6];

            GenerateVertices();
            GenerateTriangles();
            SetMesh();
        }


        private void GenerateVertices()
        {
            for (int i = 0; i < segments; i++)
            {
                // calculate angle
                float angle = i / (float)segments * angleRange * Mathf.Deg2Rad;

                // calculate vertex positions in xz or xy plane
                float outerA = Mathf.Cos(angle) * radius;
                float outerB = Mathf.Sin(angle) * radius;
                float innerA = Mathf.Cos(angle) * (radius - thickness);
                float innerB = Mathf.Sin(angle) * (radius - thickness);

                if (useXZPlane)
                {
                    _vertices[i * 2] = new Vector3(outerA, 0f, outerB);
                    _vertices[i * 2 + 1] = new Vector3(innerA, 0f, innerB);
                } else
                {
                    _vertices[i * 2] = new Vector3(outerA, outerB, 0f);
                    _vertices[i * 2 + 1] = new Vector3(innerA, innerB, 0f);
                }

                // set uv coordinates to 0 when angle is 0 and 1 when angle is 180 and back to 0 when angle is 360
                if (angle <= Mathf.PI)
                {
                    // make uvx scale
                    float uvx = Mathf.Repeat(angle / Mathf.PI * uvScale, 1);
                    _uvs[i * 2] = new Vector2(uvx, 0f);
                    _uvs[i * 2 + 1] = new Vector2(uvx, 1f);
                } else
                {
                    // start uvx from 1 to 0
                    float uvx = 1f - Mathf.Repeat((angle - Mathf.PI) / Mathf.PI * uvScale, 1);
                    _uvs[i * 2] = new Vector2(uvx, 0f);
                    _uvs[i * 2 + 1] = new Vector2(uvx, 1f);
                }
            }
        }


        private void GenerateTriangles()
        {
            for (int i = 0; i < segments; i++)
            {
                int index1 = i * 2;
                int index2 = i * 2 + 1;
                int index3 = (i * 2 + 2) % (segments * 2);
                int index4 = (i * 2 + 3) % (segments * 2);

                AddQuad(i * 6, index1, index2, index3, index4);
            }
        }


        private void SetMesh()
        {
            _mesh = new Mesh();
            _mesh.name = "RingMesh";

            _mesh.SetVertices(_vertices);
            _mesh.SetUVs(0, _uvs);
            _mesh.SetTriangles(_triangles, 0);
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();

            // set mesh filter mesh
            _meshFilter = GetComponent<MeshFilter>();
            _meshFilter.mesh = _mesh;
        }


        /// <summary>
        ///     Resize the circle by adjusting the mesh vertex positions while preserving the existing mesh topology.
        /// </summary>
        public void ResizeMesh()
        {
            // update vertices
            for (int i = 0; i < segments; i++)
            {
                // calculate angle
                float angle = i / (float)segments * angleRange * Mathf.Deg2Rad;

                // calculate vertex positions in xz or xy plane
                float outerA = Mathf.Cos(angle) * radius;
                float outerB = Mathf.Sin(angle) * radius;
                float innerA = Mathf.Cos(angle) * (radius - thickness);
                float innerB = Mathf.Sin(angle) * (radius - thickness);

                if (useXZPlane)
                {
                    _vertices[i * 2] = new Vector3(outerA, 0f, outerB);
                    _vertices[i * 2 + 1] = new Vector3(innerA, 0f, innerB);
                } else
                {
                    _vertices[i * 2] = new Vector3(outerA, outerB, 0f);
                    _vertices[i * 2 + 1] = new Vector3(innerA, innerB, 0f);
                }
            }

            // update mesh properties
            _mesh.SetVertices(_vertices);

            // update mesh filter mesh
            _meshFilter.mesh = _mesh;
        }


        /// <summary>
        ///     Visually hide mesh segments that are outside the angle range. It does not change mesh vertex positions.
        /// </summary>
        public void HideMeshByAngle()
        {
            // hide the triangles that are outside the angle range
            int obsoleteSegments = segments - (int)(angleRange / 360f * segments);
            float angleDelta = angleRange - _lastAngleRange;

            // check every segment if it is outside the angle range
            for (int i = 0; i < segments; i++)
            {
                int ii = i * 6;
                if (i < segments - obsoleteSegments)
                    // inside - draw triangles;
                    AddQuad(ii, i * 2, i * 2 + 1, (i * 2 + 2) % (segments * 2), (i * 2 + 3) % (segments * 2));
                else
                    // outside - hide triangles
                    AddQuad(ii, 0, 0, 0, 0);
            }

            _mesh.SetTriangles(_triangles, 0);
        }


        private void AddQuad(int i, int a, int b, int c, int d)
        {
            // set triangle indices
            _triangles[i] = a;
            _triangles[i + 1] = b;
            _triangles[i + 2] = c;

            _triangles[i + 3] = b;
            _triangles[i + 4] = d;
            _triangles[i + 5] = c;
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            EditorApplication.update += Validate;
        }

        private void Validate()
        {
            EditorApplication.update -= Validate;
            if (this == null || !EditorUtility.IsDirty(this)) return;
            DetermineAction();
        }
#endif
    }
}