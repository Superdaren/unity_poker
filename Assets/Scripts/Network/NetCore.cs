using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Google.Protobuf;

using Mono.Math;
using Mono.Security.Cryptography;
using NetProto;

public class NetCore : Singleton<NetCore>
{
    // Client socket.
    Socket socket = null;

    // ManualResetEvent instances signal completion.
    ManualResetEvent connectDone = new ManualResetEvent(false);
    ManualResetEvent sendDone = new ManualResetEvent(false);
    ManualResetEvent receiveDone = new ManualResetEvent(false);

    private bool isConnected = false;       // 是否连接
    public bool IsConnected { get { return isConnected; } }
    // 重连断开连接的方法[断开网络后先执行，再重连，主要用于断开后的UI操作]
    private Action disConnectAction;
    public Action DisConnectAction { set { disConnectAction = value; } }
    // 重连成功的方法[重连成功后执行，主要用于断开后的UI操作(只有在没有登录的情况下调用)]
    private Action reConnectSuccessAction;
    public Action ReConnectSuccessAction { set { reConnectSuccessAction = value; } }
    // 重连失败的方法[重连失败后执行，主要用于断开后的UI操作]
    private Action reConnectFailAction;
    public Action ReConnectFailAction { set { reConnectFailAction = value; } }

    UInt32 seqId;
    ICryptoTransform encryptor;
    ICryptoTransform decryptor;
    DiffieHellmanManaged dhEnc;
    DiffieHellmanManaged dhDec;
    Queue msgQueue;
    Dispatcher dispatcher;


    HeartBeatHandle heartBeatHandle;

    // 心跳次数
    float heartbeatCount = 0;

    class StateObject
    {
        // Packet size.
        public int header = 0;
        // Receive buffer.
        public byte[] buffer = new byte[Config.BUFFER_SIZE];
        // Received data.
        public MemoryStream ms = new MemoryStream();
    }

    /**
     * 初始化一些数据
     */
    protected NetCore()
    {
        seqId = 1;
        // 加密相关
        encryptor = null;
        decryptor = null;

        Byte[] _p = BitConverter.GetBytes(Config.DH1PRIME);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(_p);

        Byte[] _g = BitConverter.GetBytes(Config.DH1BASE);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(_g);

        dhEnc = new DiffieHellmanManaged(_p, _g, 31);
        dhDec = new DiffieHellmanManaged(_p, _g, 31);

        // 服务器返回的消息队列
        msgQueue = new Queue();

        // 注册回调
        dispatcher = new Dispatcher();
    }

    /**
     * 注册事件
     */
    public void registHandle(NetHandle handle)
    {
        dispatcher.Register(handle);
    }

    // =========================队列相关===================================
    struct Msg
    {
        public NetProto.Api.ENetMsgId id;
        public byte[] data;
    };

    void Start()
    {
        //Connect(NetProto.Config.address, NetProto.Config.port);

        heartBeatHandle = new HeartBeatHandle();
        registHandle(heartBeatHandle);
    }

    // Update is called once per frame
    void Update()
    {
        // pop 出消息
        while (MsgQueueCount() > 0)
        {
            Msg msg = PopMsg();
            dispatcher.InvokeHandler(msg.id, msg.data);
        }
        //心跳
        HandleHeartbeat();
        //检查是否需要重新Socket连接
        if (!isConnected)
        {
            if (disConnectAction != null)
            {
                // 断开网络后先执行disConnectAction，再重连
                disConnectAction();
            }
            CheckReConnect();
        }
        //定时申请和释放内存
        // 每隔60帧执行一次
        if (Time.frameCount % 60 == 0){ System.GC.Collect(); }
    }

    void HandleHeartbeat()
    {
        if (!isConnected) //只有网络连接才发心跳
            return;
        // 心跳
        heartbeatCount += Time.deltaTime;
        if (heartBeatHandle != null && heartbeatCount > Config.heartBeatInteval)
        {
            heartBeatHandle.HeartBeatReq();
        }
    }

    void PushMsg(NetProto.Api.ENetMsgId id, byte[] data)
    {
        lock (msgQueue.SyncRoot)
        {
            Msg msg = new Msg();
            msg.id = id;
            msg.data = data;

            msgQueue.Enqueue(msg);
        }
    }

    Msg PopMsg()
    {
        lock (msgQueue.SyncRoot)
        {
            return (Msg)msgQueue.Dequeue();
        }
    }

    int MsgQueueCount()
    {
        return msgQueue.Count;
    }


    // =========================加密相关===================================
    // 加密通讯
    public void Encrypt(Int32 send_seed, Int32 receive_seed)
    {
        Byte[] _send = BitConverter.GetBytes(send_seed);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(_send);
        Byte[] _recv = BitConverter.GetBytes(receive_seed);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(_recv);
        string key1;
        string key2;
        Byte[] _key1 = dhEnc.DecryptKeyExchange(_send);
        BigInteger bi1 = new BigInteger(_key1);
        key1 = Config.SALT + bi1.ToString();

        Byte[] _key2 = dhDec.DecryptKeyExchange(_recv);
        BigInteger bi2 = new BigInteger(_key2);
        key2 = Config.SALT + bi2.ToString();

        RC4 rc4enc = RC4.Create();
        RC4 rc4dec = RC4.Create();

        Byte[] seed1 = Encoding.ASCII.GetBytes(key1);
        Byte[] seed2 = Encoding.ASCII.GetBytes(key2);

        // en/decryptor不为null时自动启动加密

        // Get an encryptor.
        encryptor = rc4enc.CreateEncryptor(seed1, null);
        // Get a decryptor.
        decryptor = rc4dec.CreateDecryptor(seed2, null);
    }

    public Int32 GetSendSeed()
    {
        byte[] data = dhEnc.CreateKeyExchange();
        BigInteger i = new BigInteger(data);
        return i % Int32.MaxValue;
    }

    public Int32 GetReceiveSeed()
    {
        byte[] data = dhDec.CreateKeyExchange();
        BigInteger i = new BigInteger(data);
        return i % Int32.MaxValue;
    }


    void DecryptStream(byte[] encrypted)
    {
        Stream s;
        if (decryptor != null)
        {
            MemoryStream msDecrypt = new MemoryStream(encrypted);
            s = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        }
        else
        {
            s = new MemoryStream(encrypted);
        }

        BinaryReader binReader = new BinaryReader(s);
        byte[] opcode = binReader.ReadBytes(2);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(opcode);
        }
        Int16 id = BitConverter.ToInt16(opcode, 0);
        byte[] data = binReader.ReadBytes(encrypted.GetLength(0) - 2);

        //Debug.Log("receive NetMsgId : " + id);

        NetProto.Api.ENetMsgId msgId = (NetProto.Api.ENetMsgId)id;

        // 将数据压入到消息队列中.
        PushMsg(msgId, data);
        s.Close();
    }

    byte[] EncryptStream(byte[] toEncrypt)
    {

        if (encryptor != null)
        {
            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    // Write all data to the crypto stream and flush it.
                    csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
                    csEncrypt.FlushFinalBlock();

                    // Get the encrypted array of bytes.
                    return msEncrypt.ToArray();
                }
            }
        }
        else
        {
            return toEncrypt;
        }
    }

    // =========================socket相关===================================

    public void Connect(string host, int port)
    {
        if (isConnected)
            return;
        BeginConnect(host, port);
    }

    // handles the completion of the prior asynchronous
    // connect call.
    void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Complete the connection.
            socket.EndConnect(ar);

            //Debug.Log("Socket connected to " + socket.RemoteEndPoint.ToString());

            // Signal that the connection has been made.
            connectDone.Set();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    // Asynchronous connect using host name or address
    void BeginConnect(string host, int port)
    {
        IPHostEntry lipa = Dns.GetHostEntry(host);
        IPEndPoint lep = new IPEndPoint(lipa.AddressList[0], port);
        socket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp);

        connectDone.Reset();
        socket.BeginConnect(lep, new AsyncCallback(ConnectCallback), socket);

        // wait here until the connect finishes.  The callback
        // sets connectDone.
        bool signalled = connectDone.WaitOne(Config.CONNECTION_TIMEOUT, true);

        if (signalled)
        {
            // 开始接收数据
            Receive();
            seqId = 1;
            isConnected = true;
            reConnCount = 0; //无需重连
            Debug.Log("连接成功!");
        }
        else
        {
            Debug.Log("Connection timeout");
            if (reConnCount == 0) //第一次连接超时启动重连
                reConnCount = 1;
        }
    }

    /**
     * 收到服务器返回消息的回调
     */
    void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            //Debug.Log("ar----------------"+ ar);
            // Retrieve the state object from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;

            // Read data from the remote device.
            int bytesRead = socket.EndReceive(ar);
            if (bytesRead > 0)
            {
                // There might be more data, so store the data received so far.
                state.ms.Seek(0, SeekOrigin.End);
                state.ms.Write(state.buffer, 0, bytesRead);
                state.ms.Seek(0, SeekOrigin.Begin);
                // Handle data.
                TryHandleData(state);
                //  Get the rest of the data.
                socket.BeginReceive(state.buffer, 0, Config.BUFFER_SIZE, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            else
            {
                Debug.Log("null data");
                // Signal that all bytes have been received.
                receiveDone.Set();
            }
        }
        catch (ObjectDisposedException)
        {
            // 主动调用了Close()
            isConnected = false;
        }
        catch (Exception e)
        {
            isConnected = false;
            // 如果socket已经断了，报这个异常
            //  System.Net.Sockets.SocketException: Connection timed out
            Debug.Log(e.ToString());
            startReConnect();
        }
    }

    /**
     * 注册ReceiveCallback
     */
    void Receive()
    {
        try
        {
            // Create the state object.
            StateObject state = new StateObject();
            receiveDone.Reset();
            // Begin receiving the data from the remote device.
            socket.BeginReceive(state.buffer, 0, Config.BUFFER_SIZE, 0,
                new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception e)
        {
            isConnected = false;
            Debug.Log(e.ToString());
            startReConnect();
        }
    }

    /**
     * 处理服务端返回来的数据
     * 
     * 服务器回来的数据格式:
     * ----------2-----------2--------------data---------------------
     * -------字节长度------消息类型---------消息体(model)--------------
     */
    void TryHandleData(StateObject state)
    {
        long length = state.ms.Length;
        BinaryReader reader = new BinaryReader(state.ms);

        while (true)
        {
            if (state.header == 0)
            {
                if (length >= Config.HEADER_SIZE)
                {
                    byte[] _size = reader.ReadBytes(Config.HEADER_SIZE);
                    length -= Config.HEADER_SIZE;
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(_size);
                    }
                    state.header = BitConverter.ToUInt16(_size, 0);
                }
                else
                {
                    break;
                }
            }

            if (state.header > 0)
            {
                if (length >= state.header)
                {
                    byte[] data = reader.ReadBytes(state.header);
                    length -= state.header;
                    DecryptStream(data);
                    // Reset header.
                    state.header = 0;
                }
                else
                {
                    break;
                }
            }
        }

        if (state.ms.Length != length)
        {
            state.ms = new MemoryStream();
            if (length > 0)
            {
                byte[] data = reader.ReadBytes((int)(length));
                state.ms.Write(data, 0, data.Length);
            }
        }
    }

    /**
     * 发送数据之后的回调,看数据是否发送正常
     */
    void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Complete sending the data to the remote device.
            int bytesSent = socket.EndSend(ar);
            seqId++;
            //Debug.Log("Sent " + bytesSent + " bytes to server.");
            // Signal that all bytes have been sent.
            sendDone.Set();
        }
        catch (Exception e)
        {
            isConnected = false;
            sendDone.Set();
            Debug.Log(e.ToString());
            startReConnect();
        }
    }

    /**
     * 发送数据包
     * 
     * 发送的数据格式:
     * ----------2---------------4--------------------2--------------data---------------------
     * -------字节长度----------序列id----------------消息类型---------消息体(model)--------------
     */
    public void Send(NetProto.Api.ENetMsgId msgId, IMessage model)
    {
		byte[] data = SerializeManager.Serialize(model); // 序列化对象

        Int16 id = (Int16)msgId;
        UInt16 payloadSize = 6; // sizeof(seqid) + sizeof(msgid)

        //payloadSize += (UInt16)data.Length;
        payloadSize += (UInt16)data.Length;

        // payload
        byte[] payload = new byte[payloadSize];

        // seqid
        Byte[] _seqid = BitConverter.GetBytes(seqId);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(_seqid);
        _seqid.CopyTo(payload, 0);

        // opcode
        Byte[] _opcode = BitConverter.GetBytes(id);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(_opcode);
        _opcode.CopyTo(payload, 4);

        // data
        if (data != null)
        {
            data.CopyTo(payload, 6);
        }

        // try encrypt
        byte[] encrypted = EncryptStream(payload);

        // =>pack
        byte[] buffer = new byte[2 + payloadSize]; // sizeof(header) + payloadSize

        // =>header
        Byte[] _header = BitConverter.GetBytes(payloadSize);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(_header);
        _header.CopyTo(buffer, 0);

        // =>payload
        encrypted.CopyTo(buffer, 2);

        sendDone.Reset();

        // 发出一条消息后重置心跳时间
        heartbeatCount = 0;

        try
        {
            //Debug.Log("msgId---------------send : " + msgId);
            socket.BeginSend(buffer, 0, buffer.Length, 0, new AsyncCallback(SendCallback), socket);
            sendDone.WaitOne();
        }
        catch (Exception e)
        {
            isConnected = false;
            // 如果socket已经断了，报这个异常
            // System.Net.Sockets.SocketException: The socket is not connected
            Debug.Log(e.ToString());
            startReConnect();

        }
    }

    // 注册收到消息后的数据处理
    public void RegisterHandler(NetProto.Api.ENetMsgId opc, Dispatcher.MsgHandler handler)
    {
        dispatcher.RegisterHandler(opc, handler);
    }

    // 注册数据处理完后的一次性动作
    public bool RegisterAction(NetProto.Api.ENetMsgId id, Action<object> act)
    {
        return dispatcher.RegisterAction(id, act);
    }

    // 设置某个action不销毁
    public void setActionForever(NetProto.Api.ENetMsgId id)
    {
        dispatcher.setActionForever(id);
    }

    // 关闭网络连接
    public void Close()
    {
        if (socket != null)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            try
            {
                socket.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        isConnected = false;
        seqId = 1;
        encryptor = null;
        decryptor = null;
    }

    //处理socket 断开后的重连
    UInt16 reConnCount = 0;
    protected void ReConnect()
    {
        Close();
        if (reConnCount > 0 && reConnCount <= Config.RECONN_COUNT)
        {
            reConnCount++;
            Connect(Config.address, Config.port);
            ReConnectHandle reConnectHandle = new ReConnectHandle();
            NetCore.Instance.registHandle(reConnectHandle);
            reConnectHandle.ReConnect(reConnectSuccessAction);
        }
        else
        {
            Debug.LogError("多次连接失败！请检查你的网络是否正常");
            //todo show MessageBos and Exit
            if (reConnectFailAction != null)
            {
                reConnectFailAction();
            }
        }
    }

    //检查是需要重新Socket连接
    protected void CheckReConnect()
    {
        if (reConnCount > 0 && reConnCount <= Config.RECONN_COUNT)
        {
            ReConnect();
        }
    }

    //Socked异常，进入Socket重连接状态
    public void startReConnect()
    {
        Close();
        //Debug.Log("startReConnect()");
        reConnCount = 1;
    }


}
