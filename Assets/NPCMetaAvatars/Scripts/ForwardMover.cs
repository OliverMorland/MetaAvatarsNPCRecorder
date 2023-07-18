using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPCMetaAvatars
{
    public class ForwardMover : MonoBehaviour
    {
        [Range(0, 10f)][SerializeField] float m_speed = 1f;
        [Range(1f, 100f)][SerializeField] float m_maxDistance = 10f;
        [SerializeField] float m_distanceTravelled = 0f;
        Vector3 m_startPosition;

        private void Start()
        {
            m_startPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            transform.position += transform.forward * Time.deltaTime * m_speed;
            m_distanceTravelled += Time.deltaTime * m_speed;

            if (m_distanceTravelled > m_maxDistance)
            {
                transform.position = m_startPosition;
                m_distanceTravelled = 0;
            }
        }
    }
}
