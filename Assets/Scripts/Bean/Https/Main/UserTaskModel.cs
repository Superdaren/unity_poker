using UnityEngine;
using System.Collections;

/**
 * 任务模型
 */
public class UserTaskModel
{
	/**  任务已经完成数   **/
	public int already_completed;
	/**  过期时间    **/
	public string expire_time;
	/**  生效时间    **/
	public string effect_time;
	/**  任务图片    **/
	public string image;
	/**  任务图片描述   **/
    public string image_describe;
	/**  是非要赢才算     **/
    public int is_win;
	/**  上次更新时间时间戳    **/
    public int last_update;
	/**  任务名字    **/
    public string name;
	/**  任务父级id     **/
	public int parent_id;
	/**  任务要求描述    **/
	public string required_describe;
	/**  任务要求数量    **/
	public int required_num;
	/**  任务要求数量     **/
	public int reward_num;
	/**  任务id     **/
	public int task_id;
	/**  手牌等级 0 不需要,1开始需要手牌等级    **/
	public int task_required_hand_level;
	/**  任务要求id    **/
	public int task_required_id;
	/**  任务要求赛事类型     **/
	public int task_required_match_type;
	/**  玩家动作要求,0无要求,1打赏荷官    **/
	public int task_required_player_action;
	/**  任务要求房间类型   **/
	public int task_required_room_type;
	/**  任务奖品类型id 1金币    **/
    public int task_reward_type_id;
	/**  任务过期类型     **/
	public int task_type_expire_type;
	/**  任务类型id    **/
	public int task_type_id;
	/**  任务类型名字    **/
	public string task_type_name;
	/**  版本号   **/
	public int version;
}
