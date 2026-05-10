using UnityEngine;
using UnityEngine.Video;

public class VideoExperienceManager : MonoBehaviour
{
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private float domeRadius = 50f;
    [SerializeField] private int renderTextureSize = 3840;

    private VideoPlayer _videoPlayer;
    private GameObject _videoDome;
    private RenderTexture _renderTexture;

    private void Awake()
    {
        _renderTexture = new RenderTexture(renderTextureSize, renderTextureSize / 2, 0);

        _videoDome = CreateVideoDome();
        _videoDome.SetActive(false);
        CreateVideoPlayer();
    }

    private void Start()
    {
        if (playOnStart)
            Play();
    }

    private GameObject CreateVideoDome()
    {
        var dome = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dome.name = "VideoDome";
        dome.transform.SetParent(transform, false);
        dome.transform.localScale = Vector3.one * (domeRadius * 2f);

        var mesh = dome.GetComponent<MeshFilter>().mesh;
        var normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
            normals[i] = -normals[i];
        mesh.normals = normals;

        dome.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
        {
            mainTexture = _renderTexture
        };

        Destroy(dome.GetComponent<SphereCollider>());
        return dome;
    }

    private void CreateVideoPlayer()
    {
        _videoPlayer = gameObject.AddComponent<VideoPlayer>();
        _videoPlayer.clip = Resources.Load<VideoClip>("Videos/shark_encounter_360_h265");
        _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        _videoPlayer.targetTexture = _renderTexture;
        _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        _videoPlayer.playOnAwake = false;
        _videoPlayer.waitForFirstFrame = true;
        _videoPlayer.skipOnDrop = true;
        _videoPlayer.isLooping = true;

        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        _videoPlayer.SetTargetAudioSource(0, audioSource);
    }

    public void Play()
    {
        _videoDome.SetActive(true);
        _videoPlayer.Play();
    }

    public void Stop()
    {
        _videoPlayer.Stop();
        _videoDome.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_renderTexture != null)
            _renderTexture.Release();
    }
}
