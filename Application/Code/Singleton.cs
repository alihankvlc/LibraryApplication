﻿using UnityEngine;
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Instance;
    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<T>();

                if (m_Instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    m_Instance = singletonObject.AddComponent<T>();
                }
            }
            return m_Instance;
        }
    }
}