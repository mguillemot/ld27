/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using System;
using System.Globalization;
using UnityEngine;
using Object = UnityEngine.Object;


public abstract class GameComponent : MonoBehaviour
{

    #region Utils

    public static T FindObjectOfType<T>() where T : Object
    {
        return (T)FindObjectOfType(typeof(T));
    }

    public static T[] FindObjectsOfType<T>() where T : Object
    {
        return (T[])FindObjectsOfType(typeof(T));
    }

    public static GameObject InstantiatePrefabAtOrigin(GameObject prefab, string name = null)
    {
        return InstantiatePrefabAt(prefab, Vector3.zero, Quaternion.identity, name);
    }

    public static GameObject InstantiatePrefabAt(GameObject prefab, Vector3 position, string name = null)
    {
        return InstantiatePrefabAt(prefab, position, Quaternion.identity, name);
    }

    public static GameObject InstantiatePrefabAt(GameObject prefab, Vector3 position, Quaternion rotation, string name = null)
    {
        var obj = (GameObject)Instantiate(prefab, position, rotation);
        obj.name = name ?? prefab.name;
        return obj;
    }

    public static GameObject CreateGameObjectParented(Transform parent, string name)
    {
        var obj = new GameObject(name);
        var t = obj.transform;
        t.parent = parent;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
        return obj;
    }

    public static GameObject InstantiateGameObjectParented(GameObject prefab, Transform parent, string name = null)
    {
        var obj = (GameObject)Instantiate(prefab);
        var t = obj.transform;
        var pt = prefab.transform;
        t.parent = parent;
        t.localPosition = Vector3.zero;
        t.localRotation = pt.localRotation;
        t.localScale = pt.localScale;
        obj.name = name ?? prefab.name;
        return obj;
    }

    public bool isPrefab
    {
        get { return (GetInstanceID() >= 0); }
    }

    #endregion

    #region Managers

    public static GameManager gameManager
    {
        get { return GameManager.instance; }
    }

    #endregion

    #region Logging

    public void Log(object msg, params object[] args)
    {
        if (args.Length == 0)
        {
            Debug.Log(string.Format("[{0}] [{1}] {2}", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), gameObject.name, msg));
        }
        else
        {
            Debug.Log(string.Format("[{0}] [{1}] {2}", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), gameObject.name, string.Format(msg.ToString(), args)));
        }
    }

    public void LogDebug(object msg, params object[] args)
    {
        if (args.Length == 0)
        {
            Debug.LogWarning(string.Format("[{0}] [{1}] ==== {2}", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), gameObject.name, msg));
        }
        else
        {
            Debug.LogWarning(string.Format("[{0}] [{1}] ==== {2}", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), gameObject.name, string.Format(msg.ToString(), args)));
        }
    }

    public void LogWarning(object msg, params object[] args)
    {
        if (args.Length == 0)
        {
            Debug.LogWarning(string.Format("[{0}] [{1}] {2}", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), gameObject.name, msg));
        }
        else
        {
            Debug.LogWarning(string.Format("[{0}] [{1}] {2}", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), gameObject.name, string.Format(msg.ToString(), args)));
        }
    }

    public void LogError(object msg, params object[] args)
    {
        if (args.Length == 0)
        {
            Debug.LogError(string.Format("[{0}] [{1}] {2}", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), gameObject.name, msg));
        }
        else
        {
            Debug.LogError(string.Format("[{0}] [{1}] {2}", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), gameObject.name, string.Format(msg.ToString(), args)));
        }
    }

    #endregion

}
