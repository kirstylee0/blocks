﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;


namespace Framework
{
    public static class ReferenceFinder
    {

        public class AssetReference
        {
            public Object Obj;
            public SceneAsset Scene;
            public string Path;

            public AssetReference(Object obj, SceneAsset scene, string path)
            {
                Obj = obj;
                Scene = scene;
                Path = path;
            }
        }

        public class ValueReference
        {
            public SceneAsset Scene;
            public string Path;
            public object Value;
            public Object Obj;
            public string ValueString;

            public ValueReference(Object obj, object value, SceneAsset scene, string path)
            {
                Obj = obj;
                Value = value;
                Scene = scene;
                Path = path;
                ValueString = value == null ? "NULL" : value.ToString();
            }
        }

        static bool FilterPath(string path, bool includePlugins)
        {
            if (includePlugins) return true;
            if (path.Contains("/Plugins/")) return false;
            if (path.StartsWith("Packages/")) return false;

            return true;
        }


        static List<string> FindAssets(string typeFilter, string searchFilter, bool includePlugins)
        {
            string search = string.IsNullOrWhiteSpace(typeFilter) ? "" : "t:" + typeFilter;

            if (!string.IsNullOrEmpty(searchFilter))
            {
                search += " " + searchFilter;
            }

            List<string> paths = new List<string>();
            string[] guids = AssetDatabase.FindAssets(search);
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (FilterPath(path, includePlugins))
                {
                    paths.Add(path);
                }
            }

            return paths;
        }

        public static List<AssetReference> FindReferencesInAssets(Object asset, string searchFilter, bool includePlugins)
        {

            string guid;
            long localid;
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out guid, out localid);

            List<string> paths = FindAssets("", searchFilter, includePlugins);

            try
            {
                List<AssetReference> references = new List<AssetReference>();

                for (int i = 0; i < paths.Count; i++)
                {

                    if (!Directory.Exists(paths[i]) && !paths[i].EndsWith(".meta"))
                    {
                        string title = "Searching for " + asset.name + " references";
                        string info = "Searching asset (" + (i + 1) + " / " + paths.Count + ")";
                        float progress = ((float)i) / paths.Count;

                        if (EditorUtility.DisplayCancelableProgressBar(title, info, progress))
                        {
                            break;
                        }

                        string contents = File.ReadAllText(paths[i]);
                        if (contents.Contains(guid))
                        {
                            references.Add(new AssetReference(AssetDatabase.LoadMainAssetAtPath(paths[i]), null, paths[i]));
                        }
                    }
                }

                return references;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

        }


        public static List<AssetReference> FindReferencesInPrefabs(Object asset, string searchFilter, bool includePlugins)
        {

            string guid;
            long localid;
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out guid, out localid);

            List<string> paths = FindAssets("Prefab", searchFilter, includePlugins);

            try
            {
                List<AssetReference> references = new List<AssetReference>();

                for (int i = 0; i < paths.Count; i++)
                {
                    string title = "Searching for " + asset.name + " references";
                    string info = "Searching prefab (" + (i + 1) + " / " + paths.Count + ")";
                    float progress = ((float)i) / paths.Count;

                    if (EditorUtility.DisplayCancelableProgressBar(title, info, progress))
                    {
                        break;
                    }


                    string contents = File.ReadAllText(paths[i]);
                    if (contents.Contains(guid))
                    {
                        references.Add(new AssetReference(AssetDatabase.LoadMainAssetAtPath(paths[i]), null, paths[i]));
                    }

                }

                return references;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

        }

        public static List<AssetReference> FindReferencesInScriptableObjects(Object asset, string searchFilter, bool includePlugins)
        {

            string guid;
            long localid;
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out guid, out localid);

            List<string> paths = FindAssets("ScriptableObject", searchFilter, includePlugins);

            try
            {
                List<AssetReference> references = new List<AssetReference>();

                for (int i = 0; i < paths.Count; i++)
                {
                    string title = "Searching for " + asset.name + " references";
                    string info = "Searching ScriptableObject (" + (i + 1) + " / " + paths.Count + ")";
                    float progress = ((float)i) / paths.Count;

                    if (EditorUtility.DisplayCancelableProgressBar(title, info, progress))
                    {
                        break;
                    }


                    string contents = File.ReadAllText(paths[i]);
                    if (contents.Contains(guid))
                    {
                        references.Add(new AssetReference(AssetDatabase.LoadMainAssetAtPath(paths[i]), null, paths[i]));
                    }
                }

                return references;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

        }

        public static List<ValueReference> FindReferencesInPrefabs(Type type, FieldInfo field, bool includePlugins)
        {

            List<string> paths = FindAssets("Prefab", "", includePlugins);

            try
            {
                List<ValueReference> references = new List<ValueReference>();

                for (int i = 0; i < paths.Count; i++)
                {
                    string title = "Searching for " + field.Name + " (" + type.Name + ")" + " values";
                    string info = "Searching prefab (" + (i + 1) + " / " + paths.Count + ")";
                    float progress = ((float)i) / paths.Count;

                    if (EditorUtility.DisplayCancelableProgressBar(title, info, progress))
                    {
                        break;
                    }

                    GameObject prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath(paths[i]);
                    Component[] components = prefab.GetComponentsInChildren(type, true);

                    for (int j = 0; j < components.Length; j++)
                    {
                        references.Add(new ValueReference(prefab, field.GetValue(components[j]), null, components[j].gameObject.GetPath()));
                    }


                }

                return references;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

        }

        public static List<ValueReference> FindReferencesInScriptableObjects(Type type, FieldInfo field, bool includePlugins)
        {

            List<string> paths = FindAssets(type.Name, "", includePlugins);

            try
            {
                List<ValueReference> references = new List<ValueReference>();

                for (int i = 0; i < paths.Count; i++)
                {
                    string title = "Searching for " + field.Name + " (" + type.Name + ")" + " values";
                    string info = "Searching asset (" + (i + 1) + " / " + paths.Count + ")";
                    float progress = ((float)i) / paths.Count;

                    if (EditorUtility.DisplayCancelableProgressBar(title, info, progress))
                    {
                        break;
                    }

                    ScriptableObject asset = (ScriptableObject)AssetDatabase.LoadMainAssetAtPath(paths[i]);
                    references.Add(new ValueReference(asset, field.GetValue(asset), null, paths[i]));
                }

                return references;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

        }


        public static List<AssetReference> FindReferencesInAllScenes(Object obj, string searchFilter, bool includePlugins)
        {

            if (obj is MonoScript script)
            {
                List<AssetReference> references = new List<AssetReference>();
                Type type = script.GetClass();

                EditorUtils.PerformActionInScenes("Searching for " + obj.name + " references", (path) => FilterPath(path, includePlugins), (scene, path) =>
                 {
                     references.AddRange(FindComponentReferencesInCurrentScene(type, searchFilter));
                 });

                return references;
            }

            if (obj is GameObject prefab)
            {
                if (prefab.IsPrefabAsset())
                {
                    List<AssetReference> references = new List<AssetReference>();

                    EditorUtils.PerformActionInScenes("Searching for " + obj.name + " references", (path) => FilterPath(path, includePlugins), (scene, path) =>
                      {
                          references.AddRange(FindPrefabInstancesInCurrentScene(prefab, searchFilter));
                      });

                    return references;
                }
            }

            throw new ArgumentException("Object is not a MonoScript or a prefab asset: " + obj);
        }

        public static List<AssetReference> FindReferencesInBuildSettingsScenes(Object obj, string searchFilter)
        {

            if (obj is MonoScript script)
            {
                List<AssetReference> references = new List<AssetReference>();
                Type type = script.GetClass();

                EditorUtils.PerformActionInBuildSettingsScenes("Searching for " + obj.name + " references", (scene, path) =>
                {
                    references.AddRange(FindComponentReferencesInCurrentScene(type, searchFilter));
                });

                return references;
            }

            if (obj is GameObject prefab)
            {
                if (prefab.IsPrefabAsset())
                {
                    List<AssetReference> references = new List<AssetReference>();

                    EditorUtils.PerformActionInBuildSettingsScenes("Searching for " + obj.name + " references", (scene, path) =>
                   {
                       references.AddRange(FindPrefabInstancesInCurrentScene(prefab, searchFilter));
                   });

                    return references;
                }
            }

            throw new ArgumentException("Object is not a MonoScript or a prefab asset: " + obj);
        }

        public static List<ValueReference> FindComponentFieldValuesInAllScenes(Type type, FieldInfo field, bool includePlugins)
        {
            List<ValueReference> references = new List<ValueReference>();

            EditorUtils.PerformActionInScenes("Searching for " + field.Name + " (" + type.Name + ")" + " values", (path) => FilterPath(path, includePlugins), (scene, path) =>
               {
                   references.AddRange(FindComponentFieldValuesInCurrentScene(type, field));
               });

            return references;

        }

        public static List<ValueReference> FindComponentFieldValuesInBuildSettingsScenes(Type type, FieldInfo field)
        {
            List<ValueReference> references = new List<ValueReference>();

            EditorUtils.PerformActionInBuildSettingsScenes("Searching for " + field.Name + " (" + type.Name + ")" + " values", (scene, path) =>
            {
                references.AddRange(FindComponentFieldValuesInCurrentScene(type, field));
            });

            return references;

        }

        public static List<AssetReference> FindReferencesInCurrentScene(Object obj, string searchFilter)
        {

            if (obj is MonoScript script)
            {
                return new List<AssetReference>(FindComponentReferencesInCurrentScene(script.GetClass(), searchFilter));
            }

            if (obj is GameObject prefab)
            {
                if (prefab.IsPrefabAsset())
                {
                    return new List<AssetReference>(FindPrefabInstancesInCurrentScene(prefab, searchFilter));
                }
            }

            throw new ArgumentException("Object is not a MonoScript or a prefab asset: " + obj);
        }


        public static List<AssetReference> FindPrefabInstancesInCurrentScene(GameObject prefab, string searchFilter)
        {
            List<AssetReference> references = new List<AssetReference>();

            SceneAsset scene = GetCurrentScene();
            GameObject[] objects = Object.FindObjectsOfType<GameObject>();

            for (int i = 0; i < objects.Length; i++)
            {
                if (PrefabUtility.GetPrefabInstanceStatus(objects[i]) == PrefabInstanceStatus.Connected && PrefabUtility.GetCorrespondingObjectFromOriginalSource(objects[i]) == prefab)
                {
                    references.Add(new AssetReference(objects[i], scene, objects[i].GetPath()));
                }

            }

            return references;
        }

        public static List<AssetReference> FindComponentReferencesInCurrentScene(Type type, string searchFilter)
        {
            List<AssetReference> references = new List<AssetReference>();

            SceneAsset scene = GetCurrentScene();
            Object[] usages = Object.FindObjectsOfType(type);

            for (int i = 0; i < usages.Length; i++)
            {
                references.Add(new AssetReference(usages[i], scene, (usages[i] as Component).gameObject.GetPath()));
            }

            return references;
        }

        public static List<ValueReference> FindComponentFieldValuesInCurrentScene(Type type, FieldInfo field)
        {
            List<ValueReference> references = new List<ValueReference>();

            SceneAsset scene = GetCurrentScene();
            Object[] usages = Object.FindObjectsOfType(type);

            for (int i = 0; i < usages.Length; i++)
            {
                references.Add(new ValueReference(usages[i], field.GetValue(usages[i]), scene, (usages[i] as Component).gameObject.GetPath()));
            }

            return references;
        }

        public static List<Tuple<SceneAsset, int>> CountReferencesInAllScenes(Object obj, string searchFilter)
        {
            List<Tuple<SceneAsset, int>> counts = new List<Tuple<SceneAsset, int>>();

            EditorUtils.PerformActionInAllScenes("Searching for " + obj.name + " references", (scene, path) =>
            {
                List<AssetReference> references = FindReferencesInCurrentScene(obj, searchFilter);
                if (references.Count > 0)
                {
                    counts.Add(new Tuple<SceneAsset, int>(scene, references.Count));
                }
            });

            return counts;
        }

        static SceneAsset GetCurrentScene()
        {
            return AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
        }

    }
}
