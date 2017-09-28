using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Gambling
{
    // 房间id 
    public Int32 room_id;
    // 桌子id
    public string table_id;
    //  庄家
    public Int32 button;
    // 大盲注
    public Int32 bb;
    // 大盲注位置
    public Int32 bb_pos;
    // 小盲注
    public Int32 sb;
    // 小盲注位置 
    public Int32 sb_pos;
    // 开始时间
    public Int32 start;
    // 结束时间
    public Int32 end;
    // 最多玩家数
    public Int32 max;
    // 牌
    public Card[] cards;
    // 玩家
    public Player[] players;


}
