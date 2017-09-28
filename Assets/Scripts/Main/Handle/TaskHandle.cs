using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;

public class TaskHandle
{
	/**
     * 获取任务列表
     */
    public static void getTaskList(Action<Error, UserTaskMongoModel> action)
	{
        HttpUtil.Http.Get(URLManager.taskListUrl()).OnSuccess(result =>
		{
			if (result != null)
			{
                TaskListResult taskList = JsonMapper.ToObject<TaskListResult>(result);
                action(null, taskList.data);
            } else {
                action(new Error(500, null), null);
            }
		}).OnFail(result =>
		{
			action(new Error(500, null), null);
        }).GoSync();
	}

	/**
     * 领取任务奖励
     */
    public static void taskReceive(string taskId, Action<Error, ReceiveTaskRewardResult> action)
	{
        HttpUtil.Http.Get(URLManager.taskReceiveUrls(taskId)).OnSuccess(result =>
		{
			if (result != null)
			{
                ReceiveTaskRewardResult taskList = JsonMapper.ToObject<ReceiveTaskRewardResult>(result);
				action(null, taskList);
			}
			else
			{
				action(new Error(500, null), null);
			}
		}).OnFail(result =>
		{
			action(new Error(500, null), null);
        }).Go();
	}
}
