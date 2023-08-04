﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Object = UnityEngine.Object;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using System.Diagnostics.Eventing.Reader;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.Assertions;

#endif

namespace Framework
{


    public static class EditorUtils
    {
#if UNITY_EDITOR
        public const float INDENT_WIDTH = 15;



        public delegate void SceneProcessingAction(SceneAsset scene, string scenePath);

        private static Mesh[] _defaultPrimitiveMeshes;
        private static GUIStyle _defaultEditorLabelStyle;

        public static ReorderableList CreateReorderableList(SerializedProperty property, bool displayFoldout, bool displayAddButton = true, bool displayRemoveButton = true)
        {
            return CreateReorderableList(property, property.displayName, displayFoldout, displayAddButton, displayRemoveButton);
        }

        public static ReorderableList CreateReorderableList(SerializedProperty property, string header, bool displayFoldout, bool displayAddButton = true, bool displayRemoveButton = true)
        {
            ReorderableList list = new ReorderableList(property.serializedObject, property, true, true, displayAddButton, displayRemoveButton);

            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                if (!displayFoldout || property.isExpanded)
                {
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight), list.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none, true);
                }
            };

            list.drawFooterCallback = (rect) =>
            {
                if (!displayFoldout || property.isExpanded)
                {
                    list.footerHeight = 13f;
                    list.draggable = true;
                    ReorderableList.defaultBehaviours.DrawFooter(rect, list);
                }
                else
                {
                    list.footerHeight = 0;
                    list.draggable = false;
                }
            };

            list.drawHeaderCallback = rect =>
            {
                if (displayFoldout)
                {
                    property.isExpanded = EditorGUI.Foldout(new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height), property.isExpanded, header, true);
                }
                else
                {
                    EditorGUI.LabelField(rect, header);
                }

            };

            list.elementHeightCallback = (index) =>
            {
                if (displayFoldout && !property.isExpanded) return 0;
                return EditorGUI.GetPropertyHeight(list.serializedProperty.GetArrayElementAtIndex(index)) + 2;
            };

            list.onAddCallback = reorderableList => { property.AppendArrayElement(false); };
            list.onRemoveCallback = reorderableList => { property.RemoveLastArrayElement(); };

            return list;
        }

        public static void DrawPropertyFields(SerializedObject serializedObject, params string[] ignoreFields)
        {
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();
            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (!ignoreFields.Contains(property.name))
                    {
                        EditorGUILayout.PropertyField(property, true);
                    }
                } while (property.NextVisible(false));
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        public static bool ConditionalButton(bool condition, string text, params GUILayoutOption[] options)
        {
            return ConditionalButton(condition, new GUIContent(text), options);
        }

        public static bool ConditionalButton(bool condition, GUIContent label, params GUILayoutOption[] options)
        {
            if (condition) return GUILayout.Button(label, options);

            EditorGUI.BeginDisabledGroup(true);
            GUILayout.Button(label, options);
            EditorGUI.EndDisabledGroup();

            return false;
        }

        public static bool ConditionalButton(Rect rect, bool condition, string text)
        {
            return ConditionalButton(rect, condition, new GUIContent(text));
        }

        public static bool ConditionalButton(Rect rect, bool condition, GUIContent label)
        {
            if (condition) return GUI.Button(rect, label);

            EditorGUI.BeginDisabledGroup(true);
            GUI.Button(rect, label);
            EditorGUI.EndDisabledGroup();

            return false;
        }

        public static void DisplayCancelableProgressBar(string title, string info, float progress, out bool canceled)
        {
            canceled = EditorUtility.DisplayCancelableProgressBar(title, info, progress);
        }

        public static void PerformActionInScene(string scenePath, Action action)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string originalScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

                try
                {
                    if (EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single).IsValid())
                    {
                        action();
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
                finally
                {
                    EditorSceneManager.OpenScene(originalScene, OpenSceneMode.Single);
                }
            }
        }

        public static void PerformActionInAllScenes(string progressBarTitle, SceneProcessingAction action)
        {
            PerformActionInScenes(progressBarTitle, (Predicate<string>)null, action);
        }

        public static void PerformActionInBuildSettingsScenes(string progressBarTitle, SceneProcessingAction action)
        {
            string[] paths = new string[EditorBuildSettings.scenes.Length];

            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = EditorBuildSettings.scenes[i].path;
            }

            PerformActionInScenes(progressBarTitle, paths, action);
        }


        public static void PerformActionInScenes(string progressBarTitle, IList<string> scenePaths, SceneProcessingAction action)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string originalScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

                try
                {
                    for (int i = 0; i < scenePaths.Count; i++)
                    {
                        if (scenePaths[i] == null) continue;

                        SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePaths[i]);

                        if (EditorUtility.DisplayCancelableProgressBar(progressBarTitle, "(" + (i + 1) + "/" + scenePaths.Count + ") " + scene.name, ((float)i) / scenePaths.Count))
                        {
                            break;
                        }


                        if (EditorSceneManager.OpenScene(scenePaths[i], OpenSceneMode.Single).IsValid())
                        {
                            action(scene, scenePaths[i]);
                        }

                    }

                    EditorUtility.ClearProgressBar();
                }
                catch (Exception e)
                {

                    UnityEngine.Debug.LogException(e);
                }
                finally
                {
                    EditorSceneManager.OpenScene(originalScene, OpenSceneMode.Single);
                    EditorUtility.ClearProgressBar();
                }
            }
        }


        public static void PerformActionInScenes(string progressBarTitle, Predicate<string> scenePathFilter, SceneProcessingAction action)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string originalScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

                try
                {
                    string[] scenesGUIDs = AssetDatabase.FindAssets("t:Scene");

                    List<string> scenePaths = new List<string>();
                    for (int i = 0; i < scenesGUIDs.Length; i++)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(scenesGUIDs[i]);
                        if (path.StartsWith("Assets/") && (scenePathFilter == null || scenePathFilter(path)))
                        {
                            scenePaths.Add(path);
                        }
                    }

                    for (int i = 0; i < scenePaths.Count; i++)
                    {
                        if (scenePaths[i] == null) continue;

                        SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePaths[i]);

                        if (EditorUtility.DisplayCancelableProgressBar(progressBarTitle, "(" + (i + 1) + " / " + scenePaths.Count + ") " + scene.name, ((float)i) / scenePaths.Count))
                        {
                            break;
                        }


                        if (EditorSceneManager.OpenScene(scenePaths[i], OpenSceneMode.Single).IsValid())
                        {
                            action(scene, scenePaths[i]);
                        }

                    }

                    EditorUtility.ClearProgressBar();
                }
                catch (Exception e)
                {

                    UnityEngine.Debug.LogException(e);
                }
                finally
                {
                    EditorSceneManager.OpenScene(originalScene, OpenSceneMode.Single);
                    EditorUtility.ClearProgressBar();
                }
            }
        }

        public static void SetLayerLocked(int layer, bool locked)
        {
            if (locked)
            {
                Tools.lockedLayers |= (1 << layer);
            }
            else
            {
                Tools.lockedLayers &= ~(1 << layer);
            }
        }

        public static void SetLayerVisible(int layer, bool visible)
        {
            if (visible)
            {
                Tools.visibleLayers |= (1 << layer);
            }
            else
            {
                Tools.visibleLayers &= ~(1 << layer);
            }
        }

        public static bool IsLayerLocked(int layer)
        {
            return ((LayerMask)Tools.lockedLayers).Contains(layer);
        }

        public static bool IsLayerVisible(int layer)
        {
            return ((LayerMask)Tools.visibleLayers).Contains(layer);
        }

        public static Material GetDefaultMaterial()
        {
            return AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
        }

        public static Mesh GetDefaultPrimitiveMesh(PrimitiveType primitiveType)
        {
            if (_defaultPrimitiveMeshes == null)
            {
                _defaultPrimitiveMeshes = new Mesh[EnumUtils.GetCount<PrimitiveType>()];
                Object[] objects = AssetDatabase.LoadAllAssetsAtPath("Library/unity default resources");

                for (int i = 0; i < objects.Length; i++)
                {
                    for (int j = 0; j < _defaultPrimitiveMeshes.Length; j++)
                    {
                        if (objects[i].name == ((PrimitiveType)j).ToString())
                        {
                            _defaultPrimitiveMeshes[j] = (Mesh)objects[i];
                        }
                    }
                }
            }

            return _defaultPrimitiveMeshes[(int)primitiveType];
        }

        public static float GetTextWidth(string text)
        {
            if (_defaultEditorLabelStyle == null)
            {
                _defaultEditorLabelStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).GetStyle("label");
            }

            _defaultEditorLabelStyle.CalcMinMaxWidth(new GUIContent(text), out float min, out float max);
            return min;
        }

        public static GameObject FindPrefab(string searchText, string path = null)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab " + searchText, new[] { path == null ? "" : path });
            if (guids.Length > 0)
            {
                return (GameObject)AssetDatabase.LoadMainAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]));
            }

            return null;
        }

        public static GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            Assert.IsNotNull(prefab);

            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

            if (parent != null)
            {
                go.transform.parent = parent;
            }

            go.transform.position = position;
            go.transform.rotation = rotation;

            return go;
        }

        public static Quaternion QuaternionField(GUIContent label, Quaternion quaternion, params GUILayoutOption[] options)
        {
            return QuaternionField(EditorGUILayout.GetControlRect(options), label, quaternion);
        }

        public static Quaternion QuaternionField(Rect rect, GUIContent label, Quaternion quaternion)
        {
            return Quaternion.Euler(EditorGUI.Vector3Field(rect, label, quaternion.eulerAngles));
        }

        public static void QuaternionPropertyField(Rect rect, GUIContent label, SerializedProperty property)
        {
            EditorGUI.BeginProperty(rect, label, property);
            property.quaternionValue = Quaternion.Euler(EditorGUI.Vector3Field(rect, label, property.quaternionValue.eulerAngles));
            EditorGUI.EndProperty();
        }

        public static void PropertyField(FieldInfo field, object fieldObject, GUIContent label, params GUILayoutOption[] options)
        {
            object value = field.GetValue(fieldObject);
            value = PropertyField(field.FieldType, label, value, options);
            field.SetValue(fieldObject, value);
        }

        public static T PropertyField<T>(GUIContent label, object value, params GUILayoutOption[] options)
        {
            return (T)PropertyField(typeof(T), label, value, options);
        }

        public static object PropertyField(Type type, GUIContent label, object value, params GUILayoutOption[] options)
        {

            // generic
            // object
            //layermask
            //array


            if (type.IsArray)
            {

            }

            if (type.IsEnum) return EditorGUILayout.EnumPopup(label, (Enum)value, options);

            if (type == typeof(int)) return EditorGUILayout.IntField(label, (int)value, options);
            if (type == typeof(bool)) return EditorGUILayout.Toggle(label, (bool)value, options);
            if (type == typeof(string)) return EditorGUILayout.TextField(label, (string)value, options);
            if (type == typeof(float)) return EditorGUILayout.FloatField(label, (float)value, options);
            if (type == typeof(char)) return EditorGUILayout.IntField(label, (int)value, options);
            if (type == typeof(Color)) return EditorGUILayout.ColorField(label, (Color)value, options);

            if (type == typeof(Vector2)) return EditorGUILayout.Vector2Field(label, (Vector2)value, options);
            if (type == typeof(Vector3)) return EditorGUILayout.Vector3Field(label, (Vector3)value, options);
            if (type == typeof(Vector4)) return EditorGUILayout.Vector4Field(label, (Vector4)value, options);

            if (type == typeof(Vector2Int)) return EditorGUILayout.Vector2IntField(label, (Vector2Int)value, options);
            if (type == typeof(Vector3Int)) return EditorGUILayout.Vector3IntField(label, (Vector3Int)value, options);

            if (type == typeof(Rect)) return EditorGUILayout.RectField(label, (Rect)value, options);
            if (type == typeof(RectInt)) return EditorGUILayout.RectIntField(label, (RectInt)value, options);

            if (type == typeof(Bounds)) return EditorGUILayout.BoundsField(label, (Bounds)value, options);
            if (type == typeof(BoundsInt)) return EditorGUILayout.BoundsIntField(label, (BoundsInt)value, options);

            if (type == typeof(AnimationCurve)) return EditorGUILayout.CurveField(label, (AnimationCurve)value, options);
            if (type == typeof(Gradient)) return EditorGUILayout.GradientField(label, (Gradient)value, options);
            if (type == typeof(Quaternion)) return QuaternionField(label, (Quaternion)value, options);


            return null;
        }




        public static Component MoveComponent(Component component, GameObject destinationObject)
        {
            if (component == null) return null;

            Component newComponent = destinationObject.AddComponent(component.GetType());
            if (newComponent != null)
            {
                EditorUtility.CopySerialized(component, newComponent);
                component.SmartDestroy();
                return newComponent;
            }

            return null;
        }

        public static PlayerPrefsEntry[] GetPlayerPrefsEntires()
        {
            string path = @"SOFTWARE\Unity\UnityEditor\" + PlayerSettings.companyName + @"\" + PlayerSettings.productName;
            Microsoft.Win32.RegistryKey registry = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path);
            List<PlayerPrefsEntry> entries = new List<PlayerPrefsEntry>();

            bool GetFloat(string key, out float value)
            {
                value = PlayerPrefs.GetFloat(key, -1f);

                if (value != -1f)
                {
                    value = PlayerPrefs.GetFloat(key, -1f);
                    return value != -2f;
                }

                return false;
            }

            bool GetInt(string key, out int value)
            {
                value = PlayerPrefs.GetInt(key, -1);

                if (value != -1)
                {
                    value = PlayerPrefs.GetInt(key, -2);
                    return value != -2;
                }

                return false;
            }

            if (registry != null)
            {
                string[] names = registry.GetValueNames();
                for (int i = 0; i < names.Length; i++)
                {

                    string key = names[i].Substring(0, names[i].IndexOf("_h"));
                    if (!key.StartsWith("unity", StringComparison.InvariantCultureIgnoreCase) && PlayerPrefs.HasKey(key))
                    {
                        if (GetFloat(key, out float floatValue))
                        {
                            entries.Add(new PlayerPrefsEntry(key, floatValue));
                        }
                        else if (GetInt(key, out int intValue))
                        {
                            entries.Add(new PlayerPrefsEntry(key, intValue));
                        }
                        else
                        {
                            entries.Add(new PlayerPrefsEntry(key, PlayerPrefs.GetString(key)));
                        }

                    }

                }
            }

            return entries.ToArray();
        }

#endif
    }
}

