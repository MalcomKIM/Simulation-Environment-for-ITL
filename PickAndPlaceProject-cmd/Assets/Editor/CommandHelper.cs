using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MalcomKim.ProbRobScene;

public class CommandHelper : DemoPRS
{
     public static void StartDemoApplication()
     { 
	 
		 EditorSceneManager.OpenScene("Assets/Scenes/DemoScene.unity");
		 
		 var ini = new IniReader("Assets/Editor/project.ini");
		 var script = GameObject.Find("Publisher").GetComponent<DemoPRS>();
		 
		 foreach(string section in ini.GetSections()) {
			 foreach(string key in ini.GetKeys(section)){
				 
				 FieldInfo fi = script.GetType().GetField(key);
				 
				 string v = ini.GetValue(key,section);
				 if(fi.FieldType == typeof(string[])){
					 fi.SetValue(script, v.Split(' '));
				 }
				 else{
					 fi.SetValue(script, Convert.ChangeType(v, fi.FieldType));
				 }
			 }
		 }
		 
         EditorApplication.isPlaying = true;
		 
     }
	 
	 public static void StartCustomApplication()
     { 
	 
		 EditorSceneManager.OpenScene("Assets/Scenes/CustomScene.unity");
		 
		 var ini = new IniReader("Assets/Editor/project.ini");
		 var script = GameObject.Find("Publisher").GetComponent<CustomPRS>();
		 
		 foreach(string section in ini.GetSections()) {
			 foreach(string key in ini.GetKeys(section)){
				 
				 FieldInfo fi = script.GetType().GetField(key);
				 
				 string v = ini.GetValue(key,section);
				 if(fi.FieldType == typeof(string[])){
					 fi.SetValue(script, v.Split(' '));
				 }
				 else{
					 fi.SetValue(script, Convert.ChangeType(v, fi.FieldType));
				 }
			 }
		 }
		 
         EditorApplication.isPlaying = true;
		 
     }
}
