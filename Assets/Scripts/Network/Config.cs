namespace NetProto
{
    public class Config
    {
        // socket配置
		public const string address = "XX.XX.XX.XXX";
        public const int port = 8898;

        public const int heartBeatInteval = 5;

        public const int CONNECTION_TIMEOUT = 3000;
		public const int RECONN_COUNT = 3;
        // Size of receive buffer.
        public const int BUFFER_SIZE = 1024;
        public const int HEADER_SIZE = 2;
        // 加密
        public const string SALT = "DH";
        public const int DH1BASE = 3;
        public const int DH1PRIME = 0x7FFFFFC3;
    }
}
