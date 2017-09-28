using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public enum PKTaskStatus {
	PKTaskTodayStatus        =  1,               // 今日任务
    PKTaskWeekStatus         =  2,               // 本周任务
    PKTaskChallengeStatus    =  3,                // 挑战任务
	PKTaskAchievementStatus  =  4,               // 成就任务

}

public class TaskController : MonoBehaviour
{
    public UserTaskMongoModel userTask;

    public List<UserTaskModel> todayTasks = new List<UserTaskModel>();
    public List<UserTaskModel> weekTasks = new List<UserTaskModel>();
    public List<UserTaskModel> achievementTasks = new List<UserTaskModel>();
    public List<UserTaskModel> challengeTasks = new List<UserTaskModel>();

    public UserTaskModel[] currentTasks;
    public List<GameObject> currentTasksObject = new List<GameObject>();

    public PKTaskStatus currentStatus;

	// Use this for initialization
	void Start()
	{
        // 请求任务列表
        requestTaskList();
	}

    /**
     * 请求任务列表
     */
    public void requestTaskList() {
		TaskHandle.getTaskList((error, result) =>
		{
			if (error == null)
			{
				userTask = result;
				foreach (UserTaskModel task in userTask.list)
				{
					PKTaskStatus status = (PKTaskStatus)Enum.Parse(typeof(PKTaskStatus), task.task_type_id.ToString());
					switch (status)
					{
						case PKTaskStatus.PKTaskTodayStatus:
							{
								todayTasks.Add(task);
								break;
							}
						case PKTaskStatus.PKTaskWeekStatus:
							{
								weekTasks.Add(task);
								break;
							}
						case PKTaskStatus.PKTaskAchievementStatus:
							{
								achievementTasks.Add(task);
								break;
							}
						case PKTaskStatus.PKTaskChallengeStatus:
							{
								challengeTasks.Add(task);
								break;
							}
					}
				}
                // 对任务进行排序
                todayTasks = sortTask(todayTasks);
                weekTasks = sortTask(weekTasks);
                achievementTasks = sortTask(achievementTasks);
                challengeTasks = sortTask(challengeTasks);

				currentTasks = challengeTasks.ToArray();
                currentStatus = PKTaskStatus.PKTaskChallengeStatus;
                loadTaskView(false);
			}
		});
    }

    /**
     * 加载任务界面
     */
    public void loadTaskView(bool isCurrentRefresh) {

        if(isCurrentRefresh){
            List<GameObject> temp = new List<GameObject>();
            for (int i = 0; i < currentTasksObject.Count; i++)
			{
                if(currentTasks.Length > i) {
					setTaskView(currentTasksObject[i], currentTasks[i]);
					temp.Add(currentTasksObject[i]);
                } else {
                    GameObject.Destroy(currentTasksObject[i]);
                }
			}

            currentTasksObject = temp;
        } else {
			// 加载之前先隐藏之前的view
            for (int i = 0; i < currentTasksObject.Count; i++)
			{
                GameObject.Destroy(currentTasksObject[i]);
			}

			currentTasksObject.Clear();

			GameObject commonUIPrefab = Resources.Load("Prefabs/TaskItem") as GameObject;
			GameObject contentFirst = GameObject.Find("ContentView/Viewport/Content");
			for (int i = 0; i < currentTasks.Length; i++)
			{
				UserTaskModel userTaskModel = currentTasks[i];
				GameObject taskView = Instantiate(commonUIPrefab) as GameObject;

				taskView.transform.parent = contentFirst.transform;
				taskView.name = "taskView" + userTaskModel.task_id;
				taskView.transform.localPosition = new Vector3(0, -200, 0);
				taskView.transform.localScale = new Vector3(1, 1, 0);

				setTaskView(taskView, userTaskModel);
				currentTasksObject.Add(taskView);
			}
        }
    }

    /**
     * 任务领取奖励
     */
    public void taskReceiveClick(UserTaskModel userTaskModel) {
        TaskHandle.taskReceive(userTaskModel.task_id.ToString(), (error, receiveResult) => {
            if(error == null) {
                if(receiveResult.ret == 1){
                    // 成功之后刷新界面
                    refreshNewTask(userTaskModel, receiveResult.data);
                    // 更新用户信息
                    UserInfoRefreshManager.refreshUserInfo(null);
                    PopUtil.ShowSignInSuccessView(userTaskModel.image_describe);
                }else {
                    PopUtil.ShowMessageBoxWithConfirm("提示", "领取失败!");
                }
            } else {
                PopUtil.ShowMessageBoxWithConfirm("提示", "领取失败!");
            }
        });
    }

    /**
     * 删除领取的任务,更新服务器返回来的任务数据
     */
    public void refreshNewTask(UserTaskModel receiveedTask, UserTaskModel newTask) {

        PKTaskStatus status = (PKTaskStatus)Enum.Parse(typeof(PKTaskStatus), receiveedTask.task_type_id.ToString());
		switch (status)
		{
			case PKTaskStatus.PKTaskTodayStatus:
				{
                    todayTasks.Remove(receiveedTask);
                    if (newTask.task_id != 0){
                        todayTasks.Add(newTask);
                    }
                    currentTasks = todayTasks.ToArray();
						break;
				}
			case PKTaskStatus.PKTaskWeekStatus:
				{
                    weekTasks.Remove(receiveedTask);
					if (newTask.task_id != 0)
					{
                        weekTasks.Add(newTask);
					}
                    currentTasks = weekTasks.ToArray();
					break;
				}
			case PKTaskStatus.PKTaskAchievementStatus:
				{
                    achievementTasks.Remove(receiveedTask);
					if (newTask.task_id != 0)
					{
                        achievementTasks.Add(newTask);
					}
                    currentTasks = achievementTasks.ToArray();
					break;
				}
			case PKTaskStatus.PKTaskChallengeStatus:
				{
                    challengeTasks.Remove(receiveedTask);
					if (newTask.task_id != 0)
					{
                        challengeTasks.Add(newTask);
					}
                    currentTasks = challengeTasks.ToArray();
					break;
				}
		}

        if(newTask.task_id != 0 ) {    // 如果有任务直接刷新当前任务的view
			GameObject taskView = GameObject.Find("taskView" + receiveedTask.task_id);
            taskView.name = "taskView" + newTask.task_id;
			setTaskView(taskView, newTask);
        } else {    // 没有任务,删除掉当前任务的view
            loadTaskView(true);
        }
    }

    /**
     * 设置taskView上面的内容
     */
    public void setTaskView(GameObject taskView, UserTaskModel userTaskModel) {
		Text title = taskView.Find<Text>("ContentView/Viewport/Content/" + taskView.name + "/Title");
		Image icon = taskView.Find<Image>("ContentView/Viewport/Content/" + taskView.name + "/Icon");
		Text name = taskView.Find<Text>("ContentView/Viewport/Content/" + taskView.name + "/Name");
		Text progress = taskView.Find<Text>("ContentView/Viewport/Content/" + taskView.name + "/Progress");
		Text describe = taskView.Find<Text>("ContentView/Viewport/Content/" + taskView.name + "/Describe");
		if (userTaskModel.already_completed >= userTaskModel.required_num)
		{
			GameObject receiveobj = GameObject.Find("ContentView/Viewport/Content/" + taskView.name + "/ReceiveBtn");
			receiveobj.SetActive(true);
			Button receiveBtn = taskView.Find<Button>("ContentView/Viewport/Content/" + taskView.name + "/ReceiveBtn");
			receiveBtn.onClick.AddListener(() =>
			{
				taskReceiveClick(userTaskModel);
			});
        } else {
			GameObject receiveobj = GameObject.Find("ContentView/Viewport/Content/" + taskView.name + "/ReceiveBtn");
            receiveobj.SetActive(false);
        }

		title.text = userTaskModel.name;
		name.text = userTaskModel.image_describe;
		progress.text = userTaskModel.already_completed + "/" + userTaskModel.required_num;
		describe.text = userTaskModel.required_describe;
    }

    /**
     * 可领取的任务排在前面
     */
    public List<UserTaskModel> sortTask(List<UserTaskModel> tasks){
        List<UserTaskModel> allList = new List<UserTaskModel>();
        List<UserTaskModel> uncompleteList = new List<UserTaskModel>();
        // 先筛选可以领取的
        foreach(UserTaskModel taskModel in tasks) {
            if(taskModel.already_completed >= taskModel.required_num) {
                allList.Add(taskModel);
            } else {
                uncompleteList.Add(taskModel);
            }
        }
        // 怼入还没完成的
		foreach (UserTaskModel taskModel in uncompleteList)
		{
            allList.Add(taskModel);
		}
        return allList;
    }

    /**
     *  点击任务类型
     */
    public void taskItemClick(int index) {
		PKTaskStatus status = (PKTaskStatus)Enum.Parse(typeof(PKTaskStatus), index.ToString());
        bool isCurrentStatus = false;
        if(currentStatus == status) {
            isCurrentStatus = true;
        }
		switch (status)
		{
			case PKTaskStatus.PKTaskTodayStatus:
				{
                    currentTasks = todayTasks.ToArray();
                    currentStatus = PKTaskStatus.PKTaskTodayStatus;
					break;
				}
			case PKTaskStatus.PKTaskWeekStatus:
				{
                    currentTasks = weekTasks.ToArray();
                    currentStatus = PKTaskStatus.PKTaskWeekStatus;
					break;
				}
			case PKTaskStatus.PKTaskAchievementStatus:
				{
                    currentTasks = achievementTasks.ToArray();
                    currentStatus = PKTaskStatus.PKTaskAchievementStatus;
					break;
				}
			case PKTaskStatus.PKTaskChallengeStatus:
				{
                    currentTasks= challengeTasks.ToArray();
                    currentStatus = PKTaskStatus.PKTaskChallengeStatus;
					break;
				}
		}
        loadTaskView(isCurrentStatus);
	}

    /**
     * 关闭按钮
     */
    public void closeBtnClick() {
        PKAnimateTool.closePopUpView(gameObject);
    }
}