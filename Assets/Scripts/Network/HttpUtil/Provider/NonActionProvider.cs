using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HttpUtil.Provider
{
    public class NonActionProvider : ActionProvider
    {
        public Action<WebHeaderCollection, Stream> Success
        {
            get { return (a, b) => { }; }
        }

        public Action<WebException> Fail
        {
            get { return (a) => { throw a; }; }
        }
    }
}
