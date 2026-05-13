// Assets/Scripts/Audio/PlayerFootstepAudio.cs
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootstepAudio : MonoBehaviour
{
    [Header("Footstep Clips")]
    public AudioClip[] footstepClips;

    [Header("Step Timing")]
    public float walkStepInterval = 0.45f;
    public float runStepInterval = 0.32f;

    [Header("Movement")]
    public float minMoveSpeed = 0.1f;
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("Sound Randomness")]
    public float minVolume = 0.8f;
    public float maxVolume = 1f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    [Header("Debug")]
    public bool showDebugLog = false;

    private AudioSource audioSource;
    private PlayerController playerController;
    private CharacterController characterController;

    private float stepTimer = 0f;
    private int lastClipIndex = -1;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;

        if (playerController == null)
        {
            Debug.LogError("PlayerFootstepAudio: 当前物体上没有 PlayerController，请把脚步声脚本挂到 Hero_Knight 根物体上。");
        }

        if (characterController == null)
        {
            Debug.LogError("PlayerFootstepAudio: 当前物体上没有 CharacterController，请检查脚本挂载位置。");
        }
    }

    private void Update()
    {
        if (playerController == null || characterController == null)
        {
            return;
        }

        float moveSpeed = playerController.GetCurrentMoveSpeed();

        bool isMoving = moveSpeed > minMoveSpeed;
        bool isGrounded = characterController.isGrounded;

        if (showDebugLog)
        {
            Debug.Log("Footstep Check | isMoving: " + isMoving +
                      " | moveSpeed: " + moveSpeed +
                      " | isGrounded: " + isGrounded);
        }

        if (isMoving && isGrounded)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = GetStepInterval();
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private float GetStepInterval()
    {
        if (Input.GetKey(runKey))
        {
            return runStepInterval;
        }

        return walkStepInterval;
    }

    private void PlayFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0)
        {
            Debug.LogWarning("PlayerFootstepAudio: 没有绑定脚步声音频。");
            return;
        }

        int clipIndex = GetRandomClipIndex();
        AudioClip clip = footstepClips[clipIndex];

        if (clip == null)
        {
            Debug.LogWarning("PlayerFootstepAudio: Footstep Clips 里有空音频。");
            return;
        }

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        float volume = Random.Range(minVolume, maxVolume);

        audioSource.PlayOneShot(clip, volume);

        if (showDebugLog)
        {
            Debug.Log("播放脚步声：" + clip.name);
        }
    }

    private int GetRandomClipIndex()
    {
        if (footstepClips.Length == 1)
        {
            return 0;
        }

        int index = Random.Range(0, footstepClips.Length);

        while (index == lastClipIndex)
        {
            index = Random.Range(0, footstepClips.Length);
        }

        lastClipIndex = index;
        return index;
    }
}
