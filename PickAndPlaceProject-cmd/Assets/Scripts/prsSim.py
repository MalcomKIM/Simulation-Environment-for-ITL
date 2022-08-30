import sys
import subprocess

# start Unity
unity_path = 'C:\\Program Files\\Unity\\Hub\\Editor\\2021.3.4f1\\Editor\\Unity.exe'
project_path = 'E:\\pick_and_place\\PickAndPlaceProject-cmd'
logfile_path = 'E:\\pick_and_place\\build.log'

execute_method_demo = 'CommandHelper.StartDemoApplication'
execute_method_custom = 'CommandHelper.StartCustomApplication'

unity_cmd = [unity_path, 
        '-projectPath', 
        project_path,
        '-executeMethod',
        execute_method_custom
        # ,
        # '-batchmode',
        # '-logfile',
        # logfile_path
        ]
subprocess.Popen(unity_cmd)

# start ROS
ros_cmd_demo = 'docker run -it --rm -p 10000:10000 malcomkim/pick-and-place:v2 roslaunch niryo_moveit part_3.launch'.split()
ros_cmd_custom = 'docker run -it --rm -p 10000:10000 malcomkim/pick-and-place:v2 roslaunch niryo_moveit custom.launch'.split()
subprocess.Popen(ros_cmd_custom)

