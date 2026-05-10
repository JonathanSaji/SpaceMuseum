using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class Video360Player : MonoBehaviour
{
    [Header("Video Source")]
    [Tooltip("Absolute file path to the .mp4 on disk (use h264; AV1 will crash Unity).")]
    public string videoFilePath;

    [Header("Sphere Settings")]
    [Tooltip("Radius of the sky-sphere around the viewer.")]
    public float sphereRadius = 50f;
    [Tooltip("Longitude segments — 64 is smooth enough for VR.")]
    public int longitudeSegments = 64;
    [Tooltip("Latitude segments.")]
    public int latitudeSegments = 32;

    [Header("Performance")]
    [Tooltip("Max render-texture width. Lower = less GPU memory. 2048 is safe for most machines.")]
    public int maxTextureWidth = 2048;
    [Tooltip("Skip frames when the decoder falls behind instead of queuing them.")]
    public bool skipOnDrop = true;

    VideoPlayer _vp;
    RenderTexture _rt;
    GameObject _sphere;

    void Start()
    {
        _vp = GetComponent<VideoPlayer>();

        int texW = maxTextureWidth;
        int texH = texW / 2;
        _rt = new RenderTexture(texW, texH, 0, RenderTextureFormat.ARGB32);
        _rt.Create();

        _vp.source = VideoSource.Url;
        _vp.url = "file://" + videoFilePath;
        _vp.renderMode = VideoRenderMode.RenderTexture;
        _vp.targetTexture = _rt;
        _vp.playOnAwake = false;
        _vp.skipOnDrop = skipOnDrop;
        _vp.isLooping = true;
        _vp.audioOutputMode = VideoAudioOutputMode.Direct;

        _sphere = CreateInvertedSphere();

        var mat = new Material(Shader.Find("Unlit/Texture"));
        mat.mainTexture = _rt;
        _sphere.GetComponent<MeshRenderer>().material = mat;

        _vp.prepareCompleted += _ => _vp.Play();
        _vp.Prepare();
    }

    GameObject CreateInvertedSphere()
    {
        var go = new GameObject("360Sphere");
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;

        var mesh = new Mesh();
        mesh.name = "InvertedSphere";

        int lonSeg = longitudeSegments;
        int latSeg = latitudeSegments;
        int vertCount = (lonSeg + 1) * (latSeg + 1);
        var vertices = new Vector3[vertCount];
        var normals = new Vector3[vertCount];
        var uvs = new Vector2[vertCount];

        int idx = 0;
        for (int lat = 0; lat <= latSeg; lat++)
        {
            float theta = Mathf.PI * lat / latSeg;
            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);

            for (int lon = 0; lon <= lonSeg; lon++)
            {
                float phi = 2f * Mathf.PI * lon / lonSeg;
                float x = sinTheta * Mathf.Cos(phi);
                float y = cosTheta;
                float z = sinTheta * Mathf.Sin(phi);

                vertices[idx] = new Vector3(x, y, z) * sphereRadius;
                normals[idx] = new Vector3(-x, -y, -z);
                uvs[idx] = new Vector2(1f - (float)lon / lonSeg, (float)lat / latSeg);
                idx++;
            }
        }

        int triCount = lonSeg * latSeg * 6;
        var triangles = new int[triCount];
        int ti = 0;
        for (int lat = 0; lat < latSeg; lat++)
        {
            for (int lon = 0; lon < lonSeg; lon++)
            {
                int current = lat * (lonSeg + 1) + lon;
                int next = current + lonSeg + 1;

                // Inverted winding order for inside-facing
                triangles[ti++] = current;
                triangles[ti++] = current + 1;
                triangles[ti++] = next;

                triangles[ti++] = next;
                triangles[ti++] = current + 1;
                triangles[ti++] = next + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshRenderer>();
        return go;
    }

    void OnDestroy()
    {
        if (_rt != null)
        {
            _rt.Release();
            Destroy(_rt);
        }
        if (_sphere != null)
            Destroy(_sphere);
    }
}
