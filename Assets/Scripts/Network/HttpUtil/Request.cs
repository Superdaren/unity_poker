namespace HttpUtil
{
    using HttpUtil.Provider;
    using System;
    using System.IO;
    using System.Net;
    using HttpUtil.Streams;
    using System.Text;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;

    public class Request
    {
        protected string url;
        protected HttpVerb method = HttpVerb.Get;
        protected bool sync = false;
        protected HeaderProvider headers;
        protected AuthenticationProvider auth;
        protected BodyProvider body;
        protected byte[] bodyData;

        protected ActionProvider action;

        public Request()
        {
            
        }


        public String Url
        {
            set
            {
                this.url = value;
            }
            get
            {
                return this.url;
            }
        }

        public HttpVerb Method
        {
            set
            {
                this.method = value;
            }

            get
            {
                return this.method;
            }
        }

        public bool Sync
		{
			set
			{
                this.sync = value;
			}
			get
			{
				return this.sync;
			}
		}

        public HeaderProvider Headers
        {
            set
            {
                this.headers = value;
            }
            get
            {
                return this.headers;
            }
        }

        public AuthenticationProvider Auth
        {
            set
            {
                this.auth = value;
            }
            get
            {
                return this.auth;
            }
        }

        public ActionProvider Action
        {
            set
            {
                this.action = value;
            }
            get
            {
                return this.action;
            }
        }

        public BodyProvider Body
        {
            set
            {
                this.body = value;
            }
            get
            {
                return this.body;
            }
        }

        public byte[] BodyData
		{
			set
			{
				this.bodyData = value;
			}
			get
			{
				return this.bodyData;
			}
		}

        public void Go()
        {
            MakeRequest();
        }


        protected virtual HttpWebRequest GetWebRequest(string url)
        {
            return (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
        }

        // 授权请求
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受     
        }

        protected void MakeRequest()
        {
            // 授权请求
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url is empty");
            }

            try
            {
                /*
                 * Create new Request
                 */
                HttpWebRequest request = this.GetWebRequest(url);
                request.CookieContainer = Cookies.Container;
                request.Method = method.ToString().ToUpper();
				if (headers != null)
                {
                    request.Headers = GetHeadersFromProvider(headers.GetHeaders());
                }

                if (body != null) 
                {
					byte[] data = body.getBodyParameter();
					request.ContentType = body.GetContentType();
					using (Stream stream = request.GetRequestStream())
					{
						stream.Write(data, 0, data.Length);
					}
                }
                if(sync) {
                    ExecuteSyncRequest(request);
                } else {
                    ExecuteRequest(request);
                }

            }
            catch (WebException webEx)
            {
                action.Fail(webEx);
            }
        }

        private static WebHeaderCollection GetHeadersFromProvider(Header[] headers)
        {
            WebHeaderCollection whc = new WebHeaderCollection();

            foreach (Header h in headers)
            {
                whc[h.Name] = h.Value;
            }
            
            return whc;
        }

		protected virtual void ExecuteRequest(HttpWebRequest request)
		{
			request.BeginGetResponse(ProcessCallback(action.Success, action.Fail), request);
		}

        protected virtual void ExecuteRequestWithoutBody(HttpWebRequest request)
        {
            request.BeginGetResponse(ProcessCallback(action.Success, action.Fail), request);
        }

        protected virtual void ExecuteRequestWithBody(HttpWebRequest request)
        {
			request.BeginGetRequestStream(new AsyncCallback((IAsyncResult callbackResult) =>
			{
			    HttpWebRequest tmprequest = (HttpWebRequest)callbackResult.AsyncState;

			    ProgressCallbackHelper copy = body.GetBody().CopyToProgress(tmprequest.EndGetRequestStream(callbackResult),null);
			    copy.ProgressChanged += (bytesSent, totalBytes) => { body.OnProgressChange(bytesSent, totalBytes); };
			    copy.Completed += (totalBytes) => { body.OnCompleted(totalBytes); };
			    copy.Go();

			    // Start the asynchronous operation to get the response
			    tmprequest.BeginGetResponse(ProcessCallback(action.Success, action.Fail), tmprequest);
			}), request);
		}


        protected AsyncCallback ProcessCallback(Action<WebHeaderCollection, Stream> success, Action<WebException> fail)
        {
            return new AsyncCallback((callbackResult) =>
            {
                HttpWebRequest webRequest = (HttpWebRequest)callbackResult.AsyncState;

                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)webRequest.EndGetResponse(callbackResult))
                    {
                       // if (response.ContentLength > 0) { response.Headers.Add("Content-Length", response.ContentLength.ToString()); }
                        if (success!=null ) {
                            Stream stream = response.GetResponseStream();
							// 在unity主线程中执行
							Loom.QueueOnMainThread(() =>
							{
								success(response.Headers, stream);
							});
                        }
                    }
                }
                catch (WebException webEx)
                {
					Loom.QueueOnMainThread(() =>
					{
						fail(webEx);
					});
                }
            });
        }

        /**
         *  同步的请求接口
         */
		protected virtual void ExecuteSyncRequest(HttpWebRequest request)
		{
            try 
            {
				HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                if (action.Success != null)
                    action.Success(response.Headers, response.GetResponseStream());
                    
			} catch(WebException ex)
            {
                if(action.Fail != null)
                    action.Fail(ex);
            }
		}
    }

   
}
