﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace HttpUtil.Provider
{
    public abstract class DefaultBodyProvider : BodyProvider
    {
      

        public virtual void OnProgressChange(long bytesSent, long? totalBytes)
        {
            //Do nothing
        }

        public virtual void OnCompleted(long totalBytes)
        {
            //Do nothing
        }

        public abstract string GetContentType();

        public abstract System.IO.Stream GetBody();

        public abstract byte[] getBodyParameter();
    }
}
