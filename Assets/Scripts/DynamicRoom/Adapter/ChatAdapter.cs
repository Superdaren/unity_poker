using System.Collections.Generic;

public class ChatAdapter : BaseAdapter<string>
{
    string[] chats = {
        "Come on , Baby!",
        "全部All In!!",
        "搏一搏，单车变摩托！",
        "Come on , Baby!",
        "全部All In!!",
        "Come on , Baby!",
        "全部All In!!",
        "Come on , Baby!",
        "搏一搏，单车变摩托！",
        "Come on , Baby!",
        "全部All In!!",
        "Come on , Baby!",
        "全部All In!!",
        "搏一搏，单车变摩托！",
        "Come on , Baby!",
        "全部All In!!",
        "Come on , Baby!",
        "全部All In!!",
        "Come on , Baby!",
        "搏一搏，单车变摩托！",
        "Come on , Baby!",
        "全部All In!!",
        "Come on , Baby!",
        "搏一搏，单车变摩托！",
    };

    private int pos;
    private ButtonControler mButtonControler;
    public void SetControler(int pos, ButtonControler mButtonControler)
    {
        this.pos = pos;
        this.mButtonControler = mButtonControler;
        SetDatas(new List<string>(chats), pos, mButtonControler);
    }
}
