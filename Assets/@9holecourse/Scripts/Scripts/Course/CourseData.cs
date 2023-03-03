using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Course", menuName = "Scriptable Objects/Course", order = 3)]
public class CourseData : ScriptableObject
{
    public new string name;
    public string description;
}
