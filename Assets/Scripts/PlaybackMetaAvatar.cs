using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Avatar2;
using System.IO;


public class PlaybackMetaAvatar : OvrAvatarEntity
{
    [SerializeField] string m_recordingPath;
    List<byte[]> m_playBackBytes = new List<byte[]>();
    const int BYTE_ARRAY_LENGTH = 468;
    [SerializeField] bool m_isPlaying = false;
    float m_cycleStartTime = 0;
    float m_interval = 0.1f;
    [SerializeField] int m_counter = 0;
    [SerializeField] bool m_shouldLoop = false;
    [SerializeField] string m_localAvatarPath = "0";
    [SerializeField] bool m_playOnLoad = true;


    private void Start()
    {
        m_counter = 0;
        LoadLocalAvatar();
    }

    private void LoadLocalAvatar()
    {
        // Zip asset paths are relative to the inside of the zip.
        // Zips can be loaded from the OvrAvatarManager at startup or by calling OvrAvatarManager.Instance.AddZipSource
        // Assets can also be loaded individually from Streaming assets
        var path = new string[1];
        bool isFromZip = true;

        string assetPostfix = "_"
            + OvrAvatarManager.Instance.GetPlatformGLBPostfix(isFromZip)
            + OvrAvatarManager.Instance.GetPlatformGLBVersion(_creationInfo.renderFilters.highQualityFlags != CAPI.ovrAvatar2EntityHighQualityFlags.None, isFromZip)
            + OvrAvatarManager.Instance.GetPlatformGLBExtension(isFromZip);

        path[0] = m_localAvatarPath + assetPostfix;
        if (isFromZip)
        {
            LoadAssetsFromZipSource(path);
        }
        else
        {
            LoadAssetsFromStreamingAssets(path);
        }
    }

    protected override void OnSkeletonLoaded()
    {
        base.OnSkeletonLoaded();
        if (m_playOnLoad)
        {
            Play();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Play();
        }

        if (m_isPlaying)
        {
            float elapsedTime = Time.time - m_cycleStartTime;
            if (elapsedTime > m_interval)
            {
                if (m_counter < m_playBackBytes.Count)
                {
                    ApplyStreamData(m_playBackBytes[m_counter]);
                    SetPlaybackTimeDelay(m_interval);
                    m_counter++;
                }
                else
                {
                    if (m_shouldLoop)
                    {
                        m_counter = 0;
                        Recreate();
                    }
                    else
                    {
                        Stop();
                    }
                }
                m_cycleStartTime = Time.time;
            }
        }
    }

    void Recreate()
    {
        Teardown();
        CreateEntity();
        LoadLocalAvatar();
    }

    public void Play()
    {
        m_isPlaying = true;
        m_playBackBytes = GetBytesFromFile(m_recordingPath);
    }

    List<byte[]> GetBytesFromFile(string path)
    {
        List<byte[]> byteArrayList = new List<byte[]>();

        using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            byte[] buffer = new byte[BYTE_ARRAY_LENGTH];
            int bytesRead;
            while ((bytesRead = fileStream.Read(buffer, 0, BYTE_ARRAY_LENGTH)) > 0)
            {
                byte[] byteArray = new byte[bytesRead];
                System.Array.Copy(buffer, byteArray, bytesRead);
                byteArrayList.Add(byteArray);
            }
        }
        return byteArrayList;
    }

    public void Stop()
    {
        m_isPlaying = false;
        m_counter = 0;
    }

    public void Pause()
    {
        m_isPlaying = false;
    }
}
