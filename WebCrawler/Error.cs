
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WebCrawler
{


    public class error
    {
        public string AllJSON;
        public string ExceptionType;

        public string Message;
        public string StackTrace;
        public string Source;
        public string TargetSite;
        public string HelpLink;

        public System.Collections.IDictionary Data;
        public error InnerException;

        public string Url;
        public string Authority;
        public string Host;
        public int? Port;

        public bool IsHosted;
        public string Method;
        public System.Collections.Specialized.NameValueCollection Params;
        public System.Collections.Specialized.NameValueCollection Headers;
        public System.Collections.Specialized.NameValueCollection QueryString;
        public System.Collections.Specialized.NameValueCollection Form;

        public bool IsDb;
        public string Sql;
        public Int32 ErrorCode;

        public Int32 HttpStatusCode;
        public string HttpStatus;


        public error(Exception ex)
            : this(ex, null)
        { }

        public error(Exception ex, string SQL)
        {
            this.Sql = SQL;

            this.AllJSON = ex.ToString(); // TODO: To JSON
            this.ExceptionType = ex.GetType().ToString();
            this.Message = ex.Message;
            this.StackTrace = ex.StackTrace;
            this.Source = ex.Source;

            //this.TargetSite = ex.TargetSite;
            this.HelpLink = ex.HelpLink;
            this.Data = ex.Data;

            if (ex.InnerException != null)
                this.InnerException = new error(ex.InnerException);

            if (object.ReferenceEquals(ex.GetType(), typeof(System.Data.Common.DbException)))
            {
                this.IsDb = true;
                System.Data.Common.DbException dbex = (System.Data.Common.DbException)ex;
                this.ErrorCode = dbex.ErrorCode;
            } // End if(ex.GetType() == typeof(System.Data.Common.DbException))

            if (object.ReferenceEquals(ex.GetType(), typeof(System.Net.WebException)))
            {
                System.Net.WebException webex = (System.Net.WebException)ex;
                this.HttpStatusCode = (int)webex.Status;
                this.HttpStatus = webex.Status.ToString();
            } // End if(ex.GetType() == typeof(System.Net.WebException))

            this.IsHosted = System.Web.Hosting.HostingEnvironment.IsHosted;

            if (this.IsHosted)
            {
                if (System.Web.HttpContext.Current != null
                    && System.Web.HttpContext.Current.Request != null
                    && System.Web.HttpContext.Current.Request.Url != null
                )
                {
                    this.Url = System.Web.HttpContext.Current.Request.Url.OriginalString;
                    this.Authority = System.Web.HttpContext.Current.Request.Url.Authority;
                    this.Host = System.Web.HttpContext.Current.Request.Url.Host;
                    this.Port = System.Web.HttpContext.Current.Request.Url.Port;

                    this.Method = System.Web.HttpContext.Current.Request.HttpMethod;
                    this.Params = System.Web.HttpContext.Current.Request.Params;
                    this.Headers = System.Web.HttpContext.Current.Request.Headers;
                    this.QueryString = System.Web.HttpContext.Current.Request.QueryString;
                    this.Form = System.Web.HttpContext.Current.Request.Form;
                }

            } // End if(this.IsHosted)

            // Uri ur = new Uri("http://");
            // ur.Authority;
            //if(ur.IsAbsoluteUri
        }


        public void SaveToDb()
        {
            System.Guid uid = System.Guid.NewGuid();
            SaveToDb(uid, 1);
        }


        public void SaveToDb(Guid? parent, int Level)
        {
            if (parent.HasValue)
            {

            }

            // InsertKeyMode

            // Insert this re
            if (this.InnerException != null)
                this.InnerException.SaveToDb(parent, Level + 1);
        }

    } // End Class 

}
