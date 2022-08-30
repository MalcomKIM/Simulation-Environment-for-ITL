import sys
import subprocess

unity_path = 'C:\\Program Files\\Unity\\Hub\\Editor\\2021.3.4f1\\Editor\\Unity.exe'
project_path = 'E:\\pick_and_place\\PickAndPlaceProject-cmd'
logfile_path = 'E:\\pick_and_place\\build.log'
execute_method = 'CommandHelper.StartDemoApplication'
unity_cmd = [unity_path, 
        '-projectPath', 
        project_path,
        '-executeMethod',
        execute_method,
        #'-batchmode',
        '-logfile',
        logfile_path
        ]
subprocess.Popen(unity_cmd)

# start ROS
ros_cmd = 'docker run -it --rm -p 10000:10000 malcomkim/pick-and-place:v1 roslaunch niryo_moveit part_3.launch'.split()
subprocess.Popen(ros_cmd)

