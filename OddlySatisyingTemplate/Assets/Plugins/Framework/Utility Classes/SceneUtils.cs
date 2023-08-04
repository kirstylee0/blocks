﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Framework
{
    public static class SceneUtils
    {
        public static Camera MainCamera
        {
            get
            {

                if (_mainCamera == null || !_mainCamera.isActiveAndEnabled)
                {
                    _mainCamera = Camera.main;
                }

                if (_mainCamera == null)
                {
                    _mainCamera = FindObjectOfType<Camera>(false);
                }

                return _mainCamera;
            }
        }

        private static Camera _mainCamera;

        public static T FindNearest<T>(Vector3 point, bool includeInactive = false) where T : Component
        {
            float minDistance = Mathf.Infinity;
            T nearest = null;

            T[] objects = FindObjectsOfType<T>(true);
            for (int i = 0; i < objects.Length; i++)
            {
                float distance = (point - objects[i].transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    nearest = objects[i];
                    minDistance = distance;
                }
            }

            return nearest;
        }

        public static GameObject[] FindGameObjectsWithType<T>(bool includeInactive)
        {
            return FindGameObjectsWithType(typeof(T), includeInactive);
        }

        public static GameObject[] FindGameObjectsWithType(Type type, bool includeInactive)
        {
            HashSet<GameObject> objects = new HashSet<GameObject>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    GameObject[] rootObjects = scene.GetRootGameObjects();
                    for (int j = 0; j < rootObjects.Length; j++)
                    {
                        if (includeInactive || rootObjects[i].activeInHierarchy)
                        {
                            Component[] components = rootObjects[j].GetComponentsInChildren(type, includeInactive);
                            for (int k = 0; k < components.Length; k++)
                            {
                                if (!objects.Contains(components[k].gameObject))
                                {
                                    objects.Add(components[k].gameObject);
                                }
                            }
                        }
                    }
                }
            }

            return objects.ToArray();
        }

        public static T[] FindObjectsOfType<T>(bool includeInactive)
        {
            return FindObjectsOfType(typeof(T), includeInactive).Cast<T>().ToArray();
        }

        public static Component[] FindObjectsOfType(Type type, bool includeInactive)
        {
            List<Component> components = new List<Component>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    GameObject[] rootObjects = scene.GetRootGameObjects();
                    for (int j = 0; j < rootObjects.Length; j++)
                    {
                        if (includeInactive || rootObjects[i].activeInHierarchy)
                        {
                            components.AddRange(rootObjects[j].GetComponentsInChildren(type, includeInactive));
                        }
                    }
                }
            }

            return components.ToArray();
        }



        public static T FindObjectOfType<T>(bool includeInactive)
        {
            return (T)(object)FindObjectOfType(typeof(T), includeInactive);
        }

        public static Component FindObjectOfType(Type type, bool includeInactive)
        {
            if (!includeInactive) return (Component)Object.FindObjectOfType(type);

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    GameObject[] rootObjects = scene.GetRootGameObjects();

                    for (int j = 0; j < rootObjects.Length; j++)
                    {
                        Component component = rootObjects[j].GetComponentInChildren(type, true);
                        if (component != null)
                        {
                            return component;
                        }
                    }
                }
            }

            return null;
        }

        public static Transform GetCommonAncestor(Transform[] transforms)
        {
            if (transforms.Length > 0)
            {
                Transform currentTransform = transforms[0].parent;
                while (currentTransform != null)
                {
                    if (IsCommonAncestor(currentTransform, transforms))
                    {
                        return currentTransform;
                    }

                    currentTransform = currentTransform.parent;
                }
            }

            return null;
        }

        public static bool IsCommonAncestor(Transform parent, Transform[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i] == parent) return false;
                if (!transforms[i].IsChildOf(parent)) return false;
            }

            return true;
        }

        public static bool IsAncestor(Transform parent, Transform child)
        {

            Transform current = child.parent;
            while (current != null)
            {
                if (current == parent)
                {
                    return true;
                }

                current = current.parent;
            }

            return false;
        }

        public static int GetChildDepth(Transform parent, Transform child)
        {
            if (parent == child) return 0;

            int depth = 1;
            Transform current = child.parent;
            while (current != null)
            {
                if (current == parent) return depth;

                depth++;
                current = current.parent;
            }

            return -1;
        }

    }
}
