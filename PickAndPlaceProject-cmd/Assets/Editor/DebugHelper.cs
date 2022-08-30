using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using MalcomKim.ProbRobScene;


public class DebugHelper : EditorWindow
{
	[MenuItem("ProbRobScene/Debug Helper")]
	static void Init()
	{
		EditorWindow window = GetWindow(typeof(DebugHelper));
		window.Show();
	}
	
	
	void OnGUI()
	{
		
		if (GUILayout.Button("Execute"))
		{
			CommandHelper.StartCustomApplication();
			
		}
		
	}
	
	private void OnInspectorUpdate()
	{
		Repaint(); 
	}
}

