#if UNITY_EDITOR
using System.Collections;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using UnityEditor;
using UnityEngine;
using MalcomKim.ProbRobScene;

public class DemoPRS : MonoBehaviour
{
	[Header("ROS Connection Settings")]
	[SerializeField]
    public string m_HostIP = "127.0.0.1";
	public int k_HostPort = 10000;
	public string k_RosConnectName = "ROSConnect";
	ROSConnection m_RosConnection;
	
	[Header("Prefab Settings")]
	[SerializeField]
	public string PrefabDirectory;
	public string k_PrefabSuffix = ".prefab";
    
	// Robot Settings
	[Header("Robot Settings")]
	[SerializeField]
	public string UrdfRelativeFilepath;
	public float k_ControllerStiffness = 10000;
	public float k_ControllerDamping = 100;
	public float k_ControllerForceLimit = 1000;
    public float k_ControllerSpeed = 30;
	public float k_ControllerAcceleration = 10;
	public bool immovable = true;
	
	[Header("Joints Settings")]
	[SerializeField]
    public float k_JointAssignmentWait = 0.1f;
	public int k_NumRobotJoints = 6;
	public string[] LinkNames =
        { "world/base_link/shoulder_link", "/arm_link", "/elbow_link", "/forearm_link", "/wrist_link", "/hand_link" };
	public string k_BaseLinkName = "base_link";

	[Header("Python Settings")]
	[SerializeField]
    public string PythonPath;
	
	[Header("PRS file Settings")]
	[SerializeField]
	public string PrsPath;
	
	[Header("GameObject Name Settings")] 
	[SerializeField]
	public string ModelsName;
	public string RealSceneName;
	public string k_PublisherName = "Publisher";
	public string k_RosServiceName = "niryo_moveit";
	public string k_NiryoOneName = "RealScene/niryo_one";
	public string k_TargetName = "RealScene/Target";
	public string k_TargetPlacementName = "RealScene/TargetPlacement";
	
	[Header("Simulation Settings")] 
	public int numSimulation;
	public int MaxTimePerSim;
	public int TimeLeft;
	
	int counter = 0;
	float timer = 0f;
	
	// Camera Seetings
    const string k_CameraName = "Main Camera";
    Vector3 m_CameraPosition = new Vector3(0, 1.4f, -0.7f);
    Quaternion m_CameraRotation = Quaternion.Euler(new Vector3(45, 0, 0));

	// Other global variables
	bool m_HasPublished;
	GameObject Models;
	GameObject RealScene;
	string runtimePath;
	RobotSetting rs;
	RobotController rc;
	TextAsset textPRS;
	

    void Awake()
    {
        EditorApplication.LockReloadAssemblies();
		SetupScenePRS();
        CreateRosConnection();
		SetTrajectoryPlannerPublisher(); 
		// SetSourceDestinationPublisher();
    }

    void Update()
    {
        if (!m_HasPublished)
        {
            StartCoroutine(publish());
            m_HasPublished = true;
        }
		
		if(counter > numSimulation){
			EditorApplication.isPlaying = false; 
			if(Application.isBatchMode){
				EditorApplication.Exit(0);
			}
			
		}
		
		timer += Time.deltaTime;
		int seconds = (int)(timer % 60);
		TimeLeft = MaxTimePerSim - seconds;
		if ( TimeLeft <= 0 ){
			StateLog(2);
			restart();
		}
    }

	
	void SetupScenePRS(){
		Models = new GameObject(ModelsName);
		ModelItemImporter.ImportPrefabs(PrefabDirectory,k_PrefabSuffix, Models);
		ModelItemImporter.ImportRobot(UrdfRelativeFilepath, Models);
		SceneBuilder.BuildModels(PrsPath, Models);
		
		runtimePath = OsUtils.getPackageRuntimeAbsPath();
		textPRS = (TextAsset)AssetDatabase.LoadAssetAtPath(PrsPath, typeof(TextAsset));
		rs = new RobotSetting(k_ControllerStiffness,
							k_ControllerDamping,
							k_ControllerForceLimit,
							k_ControllerSpeed,
							k_ControllerAcceleration,
							immovable,
							k_BaseLinkName);
		
		RealScene = SceneBuilder.BuildScene(textPRS,runtimePath,PythonPath,RealSceneName, rs,Models);
		
		rc = new RobotController(GameObject.Find(k_NiryoOneName),
								k_NumRobotJoints,
								k_JointAssignmentWait,
								LinkNames);
		Models.SetActive(false);
		
		
		var camera = GameObject.Find(k_CameraName);
        camera.transform.position = m_CameraPosition;
        camera.transform.rotation = m_CameraRotation;
	}
	
	
	public void StateLog(int state){
		if(state == 0){
			Debug.Log("<color=green>[Result] Success! </color>");
		}
		else if(state == 1){
			Debug.Log("<color=red>[Result] Fail, no trajectory returned from MoverService. </color>");
		}
		else if(state == 2){
			Debug.Log("<color=red>[Result] Fail, timeout. </color>");
		}
	}
	
	
	public void restart(){
		StartCoroutine(ResetScene());
		timer = 0f;
	}
	
	IEnumerator ResetScene(){
		yield return StartCoroutine(Rebuild());
		StartCoroutine(publish());
	}
	
	public IEnumerator Rebuild(){
		RealScene = SceneBuilder.UpdateScene(textPRS,runtimePath,PythonPath,RealSceneName,rs);
		yield return StartCoroutine(rc.MoveToInitialPosition());
	}
	
	IEnumerator publish(){
		counter += 1;
		
		yield return new WaitForSeconds(1.0f);
		GameObject publisher = GameObject.Find(k_PublisherName);
		
		// SourceDestinationPublisher sdp = publisher.GetComponent<SourceDestinationPublisher>();
		// sdp.Publish();
		
		TrajectoryPlanner tp = publisher.GetComponent<TrajectoryPlanner>();
		tp.PublishJoints();
		
	}
	
	void SetTrajectoryPlannerPublisher()
    {
        GameObject demo = GameObject.Find(k_PublisherName);
        dynamic planner = demo.GetComponent<TrajectoryPlanner>();
        planner.RosServiceName = k_RosServiceName;
        planner.NiryoOne = GameObject.Find(k_NiryoOneName);
        planner.Target = GameObject.Find(k_TargetName);
        planner.TargetPlacement = GameObject.Find(k_TargetPlacementName);
    }
	
	void SetSourceDestinationPublisher(){
		GameObject demo = GameObject.Find(k_PublisherName);
		dynamic publisher = demo.GetComponent<SourceDestinationPublisher>();
        publisher.NiryoOne = GameObject.Find(k_NiryoOneName);
        publisher.Target = GameObject.Find(k_TargetName);
        publisher.TargetPlacement = GameObject.Find(k_TargetPlacementName);
	}
	
	
	void SetTagetPlacement(){
		GameObject tp = GameObject.Find(k_TargetPlacementName);
		dynamic script = tp.GetComponent<TargetPlacement>();
		script.m_Target = GameObject.Find(k_TargetName);
	}


    void CreateRosConnection()
    {
        // Create RosConnect
        var rosConnect = new GameObject(k_RosConnectName);
        m_RosConnection = rosConnect.AddComponent<ROSConnection>();
        m_RosConnection.RosIPAddress = m_HostIP;
        m_RosConnection.RosPort = k_HostPort;
    }

 
}
#endif