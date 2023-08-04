using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(TypeReference))]
    public class TypeReferenceDrawer : PropertyDrawer
    {

        private Type[] _subTypes;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {

            SubtypeAttribute attribute = property.GetAttribute<SubtypeAttribute>();
            SerializedProperty typeNameProperty = property.FindPropertyRelative("_typeName");

            EditorGUI.BeginProperty(rect, label, property);

            List<GUIContent> options = new List<GUIContent>();
            int selectedValue = 0;

            if (attribute != null)
            {

                if (_subTypes == null)
                {
                    _subTypes = attribute.Type.GetAllSubtypesInUnityAssemblies();
                }

                for (int i = 0; i < _subTypes.Length; i++)
                {
                    options.Add(new GUIContent(_subTypes[i].Name));
                    if (typeNameProperty.stringValue == _subTypes[i].Name)
                    {
                        selectedValue = i;
                    }
                }

            }

            if (options.Count > 0)
            {
                EditorGUI.BeginChangeCheck();
                int newValue = EditorGUI.Popup(rect, label, selectedValue, options.ToArray());

                if (EditorGUI.EndChangeCheck())
                {
                    typeNameProperty.stringValue = options[newValue].text;
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
