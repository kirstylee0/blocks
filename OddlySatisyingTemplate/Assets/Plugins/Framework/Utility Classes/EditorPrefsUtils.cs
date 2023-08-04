﻿#if UNITY_EDITOR
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public class EditorPreferencesHeaderAttribute : Attribute
    {
        public string Header;

        public EditorPreferencesHeaderAttribute(string header)
        {
            Header = header;
        }
    }

    public static class EditorPrefsUtils
    {

        private static string _projectName;

        public static bool GetBool(string name, bool defaultValue = false)
        {
#if UNITY_EDITOR
            return EditorPrefs.GetBool(GetKey(name), defaultValue);
#else
        Debug.LogError("Trying to use editor preference in build: "+name);
        return defaultValue;
#endif
        }

        public static string GetString(string name, string defaultValue = "")
        {
#if UNITY_EDITOR
            return EditorPrefs.GetString(GetKey(name), defaultValue);
#else
        Debug.LogError("Trying to use editor preference in build: "+name);
        return defaultValue;
#endif
        }

        public static int GetInt(string name, int defaultValue = 0)
        {
#if UNITY_EDITOR
            return EditorPrefs.GetInt(GetKey(name), defaultValue);
#else
        Debug.LogError("Trying to use editor preference in build: "+name);
        return defaultValue;
#endif
        }

        public static float GetFloat(string name, float defaultValue = 0f)
        {
#if UNITY_EDITOR
            return EditorPrefs.GetFloat(GetKey(name), defaultValue);
#else
        Debug.LogError("Trying to use editor preference in build: "+name);
        return defaultValue;
#endif
        }

        public static Color GetColour(string name, Color defaultColour)
        {
#if UNITY_EDITOR
            return StringToColour(EditorPrefs.GetString(GetKey(name), ColourToString(defaultColour)));
#else
        Debug.LogError("Trying to use editor preference in build: "+name);
        return defaultColour;
#endif
        }

        public static Color GetColour(string name)
        {
            return GetColour(name, Color.white);
        }

        public static T GetEnum<T>(string name, T defaultValue = default(T)) where T : struct, IComparable, IConvertible, IFormattable
        {
#if UNITY_EDITOR
            return (T)(object)EditorPrefs.GetInt(GetKey(name), (int)(object)defaultValue);
#else
        Debug.LogError("Trying to use editor preference in build: "+name);
        return default(T);
#endif
        }

        public static Object GetAsset(string name, Object defaultValue = null)
        {
#if UNITY_EDITOR

            string guid = EditorPrefs.GetString(GetKey(name), null);

            if (guid == null) return defaultValue;
            if (guid == "null") return null;

            return AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid));
#else
        Debug.LogError("Trying to use editor preference in build: "+name);
        return defaultValue;
#endif
        }

        public static void SetAsset(string name, Object asset)
        {
#if UNITY_EDITOR

            if (asset == null)
            {
                EditorPrefs.SetString(GetKey(name), "null");
            }
            else if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long localID))
            {
                EditorPrefs.SetString(GetKey(name), guid);
            }
            else
            {
                throw new ArgumentException();
            }
#else
        Debug.LogError("Trying to use editor preference in build: "+name);
#endif
        }

        public static List<T> GetAssetList<T>(string name, List<T> defaultValue = null) where T : Object
        {
#if UNITY_EDITOR

            string data = EditorPrefs.GetString(GetKey(name), null);

            if (data == null) return defaultValue;

            List<T> assets = new List<T>();
            string[] elements = data.Split(',');

            for (int i = 1; i < elements.Length; i++)
            {
                if (elements[i] == "null")
                {
                    assets.Add(null);
                }
                else
                {
                    assets.Add((T)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(elements[i])));
                }

            }

            return assets;
#else
        Debug.LogError("Trying to use editor preference in build: "+name);
        return defaultValue;
#endif
        }





        public static void SetAssetList(string name, List<Object> list)
        {
#if UNITY_EDITOR
            StringBuilder data = new StringBuilder(list.Count.ToString());
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                {
                    data.Append(",null");
                }
                else if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(list[i], out string guid, out long localID))
                {
                    data.Append(',');
                    data.Append(guid);
                }
                else
                {
                    throw new ArgumentException();
                }
            }

            EditorPrefs.SetString(GetKey(name), data.ToString());
#else
        Debug.LogError("Trying to use editor preference in build: "+name);
#endif
        }

        public static void SetStringList(string name, List<string> list)
        {
#if UNITY_EDITOR
            StringBuilder data = new StringBuilder(list.Count.ToString());
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                {
                    data.Append(",null");
                }
                else
                {
                    data.Append(",\"" + list[i] + "\"");
                }
            }

            EditorPrefs.SetString(GetKey(name), data.ToString());
#else
        Debug.LogError("Trying to use editor preference in build: "+name);
#endif
        }

        public static List<string> GetStringList(string name, List<string> defaultValue = null)
        {
#if UNITY_EDITOR

            string data = EditorPrefs.GetString(GetKey(name), null);

            if (data == null) return defaultValue;

            List<string> list = new List<string>();
            string[] elements = data.Split(',');

            for (int i = 1; i < elements.Length; i++)
            {
                if (elements[i] == "null")
                {
                    list.Add(null);
                }
                else
                {
                    list.Add(elements[i].Trim('"'));
                }
            }

            return list;
#else
        Debug.LogError("Trying to use editor preference in build: "+name);
        return defaultValue;
#endif
        }

        static string GetKey(string name)
        {
            if (_projectName == null)
            {
                string[] s = Application.dataPath.Split('/');
                _projectName = s[s.Length - 2];
            }

            return _projectName + " " + name;
        }

        static Color StringToColour(string colourString)
        {
            if (ColorUtility.TryParseHtmlString("#" + colourString, out Color colour))
            {
                return colour;
            }
            return Color.white;
        }

        static string ColourToString(Color colour)
        {
            return ColorUtility.ToHtmlStringRGBA(colour);
        }

#if UNITY_EDITOR

        private static Dictionary<string, ReorderableList> _listControls;

        public static SettingsProvider CreateSettingsProvider(Type settingsType, string categoryName)
        {

            return new SettingsProvider("Preferences/" + categoryName, SettingsScope.User)
            {
                guiHandler = (searchContext) =>
                {
                    PropertyInfo[] fields = settingsType.GetProperties(BindingFlags.Static | BindingFlags.Public);

                    EditorGUIUtility.labelWidth = 200f;



                    for (int i = 0; i < fields.Length; i++)
                    {
                        EditorPreferencesHeaderAttribute header = fields[i].GetCustomAttribute<EditorPreferencesHeaderAttribute>();
                        if (header != null)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField(header.Header, EditorStyles.boldLabel);
                        }


                        DrawControl(fields[i].PropertyType, fields[i].Name, fields[i].GetValue(null));
                    }
                },

                keywords = GetKeywords()
            };

            IEnumerable<string> GetKeywords()
            {
                yield return categoryName;

                PropertyInfo[] fields = settingsType.GetProperties(BindingFlags.Static | BindingFlags.Public);

                for (int i = 0; i < fields.Length; i++)
                {
                    yield return fields[i].Name;
                }


            }
        }

        static void DrawControl(Type type, string name, object defaultValue)
        {
            string key = GetKey(name);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(StringUtils.Titelize(name)));


            if (type == typeof(bool)) EditorPrefs.SetBool(key, EditorGUILayout.Toggle((bool)defaultValue, GUILayout.ExpandWidth(true)));
            if (type == typeof(string)) EditorPrefs.SetString(key, EditorGUILayout.TextField(EditorPrefs.GetString(key, (string)defaultValue)));
            if (type == typeof(float)) EditorPrefs.SetFloat(key, EditorGUILayout.FloatField(EditorPrefs.GetFloat(key, (float)defaultValue)));
            if (type == typeof(int)) EditorPrefs.SetInt(key, EditorGUILayout.IntField(EditorPrefs.GetInt(key, (int)defaultValue)));
            if (type == typeof(Color))
            {
                string defaultColour = ColourToString((Color)defaultValue);

                Color oldColour = StringToColour(EditorPrefs.GetString(key, defaultColour));
                Color newColour = EditorGUILayout.ColorField(oldColour);

                EditorPrefs.SetString(key, ColourToString(newColour));

            }
            if (type.IsEnum) EditorPrefs.SetInt(key, (int)(object)EditorGUILayout.EnumPopup((Enum)Enum.ToObject(type, EditorPrefs.GetInt(key, (int)defaultValue))));

            if (typeof(Object).IsAssignableFrom(type))
            {
                SetAsset(name, EditorGUILayout.ObjectField(GetAsset(name, (Object)defaultValue), type, false));
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) && typeof(Object).IsAssignableFrom(type.GenericTypeArguments[0]))
            {

                if (_listControls == null)
                {
                    _listControls = new Dictionary<string, ReorderableList>();
                }

                if (!_listControls.TryGetValue(key, out ReorderableList list))
                {
                    IList value = GetAssetList(name, defaultValue != null ? ((IList)defaultValue).Cast<Object>().ToList() : null);
                    list = AssetListControl(value, type.GenericTypeArguments[0], StringUtils.Titelize(name));
                    _listControls.Add(key, list);
                }

                GUILayout.BeginVertical();
                list.DoLayoutList();
                GUILayout.EndVertical();

                SetAssetList(name, (List<Object>)list.list);
            }

            GUILayout.EndHorizontal();
        }

        static ReorderableList AssetListControl(IList elements, Type elementType, string header)
        {
            ReorderableList list = new ReorderableList(elements, elementType, true, false, true, true);

            list.onAddCallback = reorderableList => { list.list.Add(null); };
            list.onRemoveCallback = reorderableList => { list.list.RemoveAt(list.index); };

            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {

                if (isFocused) list.index = index;
                list.list[index] = EditorGUI.ObjectField(rect, GUIContent.none, (Object)list.list[index], elementType, false);
            };

            list.drawFooterCallback = (rect) =>
            {
                list.footerHeight = 19f;
                list.draggable = true;
                ReorderableList.defaultBehaviours.DrawFooter(rect, list);
            };

            list.elementHeightCallback = (index) => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            return list;
        }

#endif
    }
}
