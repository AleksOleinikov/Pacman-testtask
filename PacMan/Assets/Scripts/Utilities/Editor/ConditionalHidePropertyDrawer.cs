﻿using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);        

        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (!condHAtt.HideInInspector || enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }        

        GUI.enabled = wasEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        if (!condHAtt.HideInInspector || enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            //The property is not being drawn
            //We want to undo the spacing added before and after the property
            return -EditorGUIUtility.standardVerticalSpacing;
            //return 0.0f;
        }


        /*
        //Get the base height when not expanded
        var height = base.GetPropertyHeight(property, label);

        // if the property is expanded go thru all its children and get their height
        if (property.isExpanded)
        {
            var propEnum = property.GetEnumerator();
            while (propEnum.MoveNext())
                height += EditorGUI.GetPropertyHeight((SerializedProperty)propEnum.Current, GUIContent.none, true);
        }
        return height;*/
    }

    private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
    {
        bool enabled = false;

        //Get the full relative property path of the sourcefield so we can have nested hiding
        string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
        //SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(condHAtt.ConditionalSourceField);

        if (sourcePropertyValue != null)
        {
            enabled = CheckPropertyType(sourcePropertyValue,condHAtt.ConditionalComparison);               
        }
        else
        {
            //Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
        }

        conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField2); //changes the path to the conditionalsource property path
        SerializedProperty sourcePropertyValue2 = property.serializedObject.FindProperty(conditionPath);
        //SerializedProperty sourcePropertyValue2 = property.serializedObject.FindProperty(condHAtt.ConditionalSourceField2);
        if (sourcePropertyValue2 != null)
        {
            enabled = enabled && CheckPropertyType(sourcePropertyValue2, condHAtt.ConditionalComparison);
        }
        else
        {
            //Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
        }

        if (condHAtt.Inverse) enabled = !enabled;

        return enabled;
    }

    private bool CheckPropertyType(SerializedProperty sourcePropertyValue, int [] comparioson=null)
    {
        switch (sourcePropertyValue.propertyType)
        {                
            case SerializedPropertyType.Boolean:
                return sourcePropertyValue.boolValue;                
            case SerializedPropertyType.ObjectReference:
                return sourcePropertyValue.objectReferenceValue != null;
            case SerializedPropertyType.Enum:
                for (int i = 0; i < comparioson.Length; i++)
                {
                    if(comparioson[i]== sourcePropertyValue.enumValueIndex)
                    { return true; }
                }
                return false;
            default:
                Debug.LogError("Data type of the property used for conditional hiding [" + sourcePropertyValue.propertyType + "] is currently not supported");
                return true;
        }
    }
}
