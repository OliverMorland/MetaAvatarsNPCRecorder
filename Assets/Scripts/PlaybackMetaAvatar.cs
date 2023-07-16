using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Avatar2;
using System.IO;
using System.ComponentModel;

[RequireComponent(typeof(OvrAvatarEntity))]
public class PlaybackMetaAvatar : MonoBehaviour
{
    [SerializeField] string m_recordingPath;
    OvrAvatarEntity m_entity;
    List<byte[]> m_playBackBytes = new List<byte[]>();
    const int BYTE_ARRAY_LENGTH = 468;
    [SerializeField] bool m_isPlaying = false;
    float m_cycleStartTime = 0;
    float m_interval = 0.1f;
    [SerializeField] int m_counter = 0;


    private void Awake()
    {
        m_entity = GetComponent<OvrAvatarEntity>();
    }

    private void Start()
    {
        m_counter = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Play();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReadBytesFromFile();
        }

        if (m_isPlaying)
        {
            float elapsedTime = Time.time - m_cycleStartTime;
            if (elapsedTime > m_interval)
            {
                if (m_counter < m_playBackBytes.Count)
                {
                    m_entity.ApplyStreamData(m_playBackBytes[m_counter]);
                    m_entity.SetPlaybackTimeDelay(m_interval);
                    m_counter++;
                }
                else
                {
                    Stop();
                }
                m_cycleStartTime = Time.time;
            }
        }
    }

    public void Play()
    {
        m_isPlaying = true;
        m_playBackBytes = GetBytesFromFile(m_recordingPath);
    }

    void ReadBytesFromFile()
    {
        List<byte[]> bytesFromFile = GetBytesFromFile(m_recordingPath);
        Debug.Log("Byte Arrays In File: " + bytesFromFile.Count);
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
