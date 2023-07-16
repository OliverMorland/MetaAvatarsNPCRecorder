using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Avatar2;
using System.IO;

[RequireComponent(typeof(OvrAvatarEntity))]
public class MetaAvatarRecorder : MonoBehaviour
{
    OvrAvatarEntity m_entity;
    [SerializeField] bool m_isRecording = false;

    float m_cycleStartTime = 0;
    float m_interval = 0.1f;
    List<byte[]> m_recordedBytesList = new List<byte[]>();

    const string DIR_NAME = "AvatarRecordings";
    [SerializeField] string m_fileNameToSaveTo = "MyRecording.bin";

    private void Awake()
    {
        m_entity = GetComponent<OvrAvatarEntity>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartRecording();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StopRecording();
        }

        if (m_isRecording)
        {
            float elapsedTime = Time.time - m_cycleStartTime;
            if (elapsedTime > m_interval)
            {
                byte[] recordedBytes = m_entity.RecordStreamData(m_entity.activeStreamLod);
                m_recordedBytesList.Add(recordedBytes);
                m_cycleStartTime = Time.time;
            }
        }
    }

    public void StartRecording()
    {
        Debug.Log("Starting Recording");
        m_recordedBytesList.Clear();
        m_isRecording = true;
    }

    public void StopRecording()
    {
        Debug.Log("Stopping Recording");
        m_isRecording = false;
        SaveRecording();
    }

    void SaveRecording()
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, DIR_NAME);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        string path = Path.Combine(directoryPath, m_fileNameToSaveTo);
        using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            foreach (byte[] byteArray in m_recordedBytesList)
            {
                Debug.Log("Recorded byte array length: " + byteArray.Length);
                fileStream.Write(byteArray, 0, byteArray.Length);
            }
        }
        Debug.Log("Saved a recording of length " + m_recordedBytesList.Count);

    }
}
