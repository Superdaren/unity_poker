using System.Collections.Generic;
            
public class RoomList
{
    public int ret;
    public List<Room> list;
}

public class Room
{
    public int id;
    public int big_blind;
    public int small_blind;
    public int min_carry;
    public int max_carry;
	public int max;
	public int online;
}