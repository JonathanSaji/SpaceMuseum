using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoExperienceManager : MonoBehaviour
{
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private float domeRadius = 50f;
    [SerializeField] private int renderTextureSize = 3840;

    private VideoPlayer _videoPlayer;
    private GameObject _videoDome;
    private RenderTexture _renderTexture;
    private InputAction _leftGrip, _leftClick, _rightGrip, _rightClick;
    private bool _playing;

    private void Awake()
    {
        _renderTexture = new RenderTexture(renderTextureSize, renderTextureSize / 2, 0);

        _videoDome = CreateVideoDome();
        _videoDome.SetActive(false);
        CreateVideoPlayer();
        CreateExitInputs();
    }

    private void CreateExitInputs()
    {
        _leftGrip  = new InputAction("leftGrip",  binding: "<XRController>{LeftHand}/{Grip}");
        _leftClick = new InputAction("leftClick", binding: "<XRController>{LeftHand}/{Primary2DAxisClick}");
        _rightGrip  = new InputAction("rightGrip",  binding: "<XRController>{RightHand}/{Grip}");
        _rightClick = new InputAction("rightClick", binding: "<XRController>{RightHand}/{Primary2DAxisClick}");

        _leftGrip.Enable();
        _leftClick.Enable();
        _rightGrip.Enable();
        _rightClick.Enable();
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
        _playing = true;
        _videoDome.SetActive(true);
        _videoPlayer.Play();
        DisableLocomotion();
    }

    public void Stop()
    {
        _playing = false;
        _videoPlayer.Stop();
        _videoDome.SetActive(false);
    }

    private void Update()
    {
        if (!_playing) return;

        if (_leftGrip.IsPressed() && _leftClick.IsPressed() &&
            _rightGrip.IsPressed() && _rightClick.IsPressed())
        {
            SceneManager.LoadScene("Main VR Origin");
        }
    }

    private static void DisableLocomotion()
    {
        foreach (var mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            var name = mb.GetType().Name;
            if (name is "ContinuousMoveProvider" or "ContinuousTurnProvider")
                mb.enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (_renderTexture != null)
            _renderTexture.Release();
    }
}
