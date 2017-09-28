using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpUtil.Streams
{
    public struct ProgressChangedEventArgs 
    {
        public long BytesCopied { get; private set; }
        public long? TotalBytes { get; private set; }
        
    }
}
