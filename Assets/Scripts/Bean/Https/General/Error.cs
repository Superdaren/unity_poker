using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 错误类
 */
public class Error
{
    public static string PokerErrorDomain = "PokerErrorDomain";

    public enum ErrorCode{
        Error = 500,
        Cancel
    }

	private string _domain = PokerErrorDomain;
	public string domain 
	{
		get { return this._domain; }
		set { this._domain = value; }
	}

    private int _code; 
    public int code
	{
		get { return this._code; }
		set { this._code = value; }
	}

    private string _msg;
	public string msg
	{
		get { return this._msg; }
		set { this._msg = value; }
	}

	private Dictionary<object, object> _userInfo;
	public Dictionary<object, object> userInfo
	{
		get { return this._userInfo; }
		set { this._userInfo = value; }
	}

    public Error(string domain, int code, Dictionary<object, object> userInfo){
        _domain = domain;
        _code = code;
        _userInfo = userInfo;
    }

    public Error(int code, string msg)
	{
		_code = code;
        _msg = msg;
	}
}
