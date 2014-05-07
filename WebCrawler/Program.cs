
using System;
using System.Collections.Generic;
using System.Windows.Forms;


//using HtmlAgilityPack;


namespace WebCrawler
{


    static class Program
    {



		public static void CombineUrl()
		{
			Uri root = new Uri(@"http://www.ds.ch/lol/sprint/mix/index.aspx", UriKind.Absolute);

			string newurl = "../foo/abc/../images/arrow.gif";
			// newurl = "https://wwws.google.com/images";
			Uri relative = new Uri(newurl, UriKind.RelativeOrAbsolute);

			Uri comb = new Uri(root, relative);
			Console.WriteLine (comb);
		} // End Sub CombineUrl


		public static string NormalizeURL(string context, string url)
		{
			Uri urContext = new Uri(context, UriKind.Absolute);
			Uri urURL = new Uri(url, UriKind.RelativeOrAbsolute);


			// Console.WriteLine(urContext.Scheme);
			// Console.WriteLine(System.Uri.SchemeDelimiter);

			// Console.WriteLine(urContext.Authority);
			// Console.WriteLine(urContext.Host); // Unlike the Authority property, this property value does not include the port number.
			// Console.WriteLine(urContext.Port);

			// Console.WriteLine(urContext.AbsolutePath);
			// Console.WriteLine(urContext.LocalPath);

			// System.Net.ServicePointManager.

			// if (System.Text.RegularExpressions.Regex.IsMatch (url, "^https?://", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
			if(url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) 
				|| url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
				|| url.StartsWith(urContext.Scheme + "://", StringComparison.OrdinalIgnoreCase)
				)
				return url;

			if(url.StartsWith("//", StringComparison.OrdinalIgnoreCase))
			{
				url = urContext.Scheme + url;

				return url;
			} // End if(url.StartsWith("//", StringComparison.OrdinalIgnoreCase))

			// string BaseURL = urContext.Authority + urContext.AbsolutePath;
			// Console.WriteLine (BaseURL);

			Uri comb = new Uri(urContext, urURL);
			return comb.OriginalString;
		} // End Function NormalizeURL


		public static string[] GetSiteUrls(string URL)
		{
			//List<string> ls = new List<string> ();
			// Is incorrect, path must be case-sensitive, url mustn't be.
			HashSet<string> hs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			GetSiteUrls (URL, hs);

			string[] stringArray = new string[hs.Count];
			hs.CopyTo (stringArray);
			hs.Clear ();
			hs = null;

			return stringArray;
		} // End Function GetSiteUrls



        public static bool IsLargeContentType(string URL)
        {
            Uri ur = new Uri(URL);
            string fn = System.IO.Path.GetFileName(ur.AbsolutePath);
			string ext = System.IO.Path.GetExtension(fn);

            //Console.WriteLine(ur.AbsolutePath);
            //Console.WriteLine(fn);
            //Console.WriteLine(ext);

            if (string.IsNullOrEmpty(ext))
                return false;

            if (StringComparer.OrdinalIgnoreCase.Equals(ext, ".aspx"))
                return false;

            if (StringComparer.OrdinalIgnoreCase.Equals(ext, ".html"))
                return false;

            if (StringComparer.OrdinalIgnoreCase.Equals(ext, ".htm"))
                return false;

            if (StringComparer.OrdinalIgnoreCase.Equals(ext, ".jpg"))
                return true;

            if (StringComparer.OrdinalIgnoreCase.Equals(ext, ".png"))
                return true;

            if (StringComparer.OrdinalIgnoreCase.Equals(ext, ".gif"))
                return true;

            if (StringComparer.OrdinalIgnoreCase.Equals(ext, ".webp"))
                return true;

            if (StringComparer.OrdinalIgnoreCase.Equals(ext, ".jpeg"))
                return true;

            if (StringComparer.OrdinalIgnoreCase.Equals(ext, ".bmp"))
                return true;

            if (StringComparer.OrdinalIgnoreCase.Equals(ext, ".tif"))
                return true;

            if (StringComparer.OrdinalIgnoreCase.Equals(ext, ".tiff"))
                return true;

            if (StringComparer.OrdinalIgnoreCase.Equals(ext, ".pdf"))
                return true;

            return false;
        }


		public static void GetSiteUrls(string URL, HashSet<string> hs)
		{
			// System.Collections.Specialized.StringCollection sc = new System.Collections.Specialized.StringCollection();

			try
			{
				hs.Add(URL);

				HtmlAgilityPack.HtmlWeb hw = new HtmlAgilityPack.HtmlWeb();

				hw.UserAgent = "Lord Vishnu/Transcendental (Vaikuntha;Supreme Personality of Godness)";
				hw.UsingCache = false;
				hw.UseCookies = true;

				//HtmlAgilityPack.HtmlDocument doc = hw.Load("http://www.daniel-steiger1.ch/NonExistant");
				HtmlAgilityPack.HtmlDocument doc = hw.Load(URL);
				// doc.Save("out.html");
				// Console.WriteLine(hw.StatusCode);

				#if false

				// $linksOrStyles = $xml->xpath('//*[@rel="stylesheet" or @media="all" or @media="screen"]');     
				// HtmlNodeCollection ncStyleSheets = doc.DocumentNode
				//     .SelectNodes("//link[@rel=\"stylesheet\" or @media=\"all\" or @media=\"screen\"]");
					.SelectNodes("//link[@rel=\"stylesheet\"]");

				if (ncStyleSheets != null)
				{

					foreach (HtmlAgilityPack.HtmlNode link in ncStyleSheets)
					{
						//Console.WriteLine(link.Name);
						Console.WriteLine(link.Attributes["href"].Value);
					} // Next link

				} // End if (ncStyleSheets != null)

				#endif


				bool bOnlySameOrigin = true;

				HtmlAgilityPack.HtmlNodeCollection ncLinks = doc.DocumentNode.SelectNodes("//a[@href]");

				if (ncLinks != null)
				{
					//foreach (HtmlNode link in doc.DocumentElement.SelectNodes("//a[@href]"))
					foreach (HtmlAgilityPack.HtmlNode link in ncLinks)
					{
						//Console.WriteLine(link.Name);
						Console.WriteLine(link.InnerText);
						string href = link.Attributes["href"].Value;

						href = NormalizeURL(URL, href);
						Uri BaseUrl = new Uri(URL);
						Uri urHref = new Uri(href);

						bool bSameOrigin = false;
						if( StringComparer.OrdinalIgnoreCase.Equals(BaseUrl.Authority, urHref.Authority) )
							bSameOrigin = true;

						if(!bOnlySameOrigin || bSameOrigin)
						{
                            if (!IsLargeContentType(href))
                            {
                                if (!hs.Contains(href))
                                    GetSiteUrls(href, hs);
                            } // End if (!IsImageUrl(href))

							//ls.Add(href);
						} // End if(!bOnlySameOrigin || bSameOrigin)

					} // Next link

				} // End if (ncLinks != null)


				HtmlAgilityPack.HtmlNodeCollection ncIframes = doc.DocumentNode.SelectNodes("//iframe[@src]");

				if (ncIframes != null)
				{
					//foreach (HtmlNode link in doc.DocumentElement.SelectNodes("//a[@href]"))
					foreach (HtmlAgilityPack.HtmlNode iframe in ncIframes)
					{
						// Console.WriteLine(iframe.Name);
						// Console.WriteLine(iframe.Id);
						string src = iframe.Attributes["src"].Value;

						src = NormalizeURL(URL, src);
						Uri BaseUrl = new Uri(URL);
						Uri urSrc = new Uri(src);

						bool bSameOrigin = false;
						if( StringComparer.OrdinalIgnoreCase.Equals(BaseUrl.Authority, urSrc.Authority) )
							bSameOrigin = true;

						if(!bOnlySameOrigin || bSameOrigin)
						{
                            //ls.Add(href);

                            if (!IsLargeContentType(src))
                            {
                                if (!hs.Contains(src))
                                    GetSiteUrls(src, hs);
                            } // End if (!IsImageUrl(src))

						} // End if(!bOnlySameOrigin || bSameOrigin)

					} // Next link

				} // End if (ncIframes != null)


			}
			catch(System.Net.WebException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);

				switch(ex.Status)
				{
					case System.Net.WebExceptionStatus.NameResolutionFailure:
						break;
					case System.Net.WebExceptionStatus.ConnectFailure:
						break;
					case System.Net.WebExceptionStatus.ConnectionClosed:
						break;
					case System.Net.WebExceptionStatus.TrustFailure:
						break;
					case System.Net.WebExceptionStatus.SecureChannelFailure:
						break;
					case System.Net.WebExceptionStatus.Timeout:
						break;
					case System.Net.WebExceptionStatus.RequestCanceled:
						break;
					case System.Net.WebExceptionStatus.SendFailure:
						break;
					case System.Net.WebExceptionStatus.ReceiveFailure:
						break;
					case System.Net.WebExceptionStatus.KeepAliveFailure:
						break;
					case System.Net.WebExceptionStatus.PipelineFailure:
						break;
					case System.Net.WebExceptionStatus.ServerProtocolViolation:
						break;
					case System.Net.WebExceptionStatus.MessageLengthLimitExceeded:
					case System.Net.WebExceptionStatus.UnknownError:
					default:
						throw;
						//break;
				} // End switch(ex.Status)

			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}

		} // End Function GetSiteUrls



		// System.Runtime.InteropServices.Marshal.SizeOf(System.IntPtr);
		//const int padb = 20 - 2 * sizeof(System.IntPtr)- sizeof(int);
		const int padb = 20 - 2 * 4- sizeof(int); // If we cannot pad exactly, 
		// let the structure be larger than 64 bytes


		[System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
		struct sysinfo_t
		{
			public System.UIntPtr  uptime;             // Seconds since boot

			[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst=3)]
			public System.UIntPtr [] loads;  // 1, 5, and 15 minute load averages


			public System.UIntPtr totalram;  // Total usable main memory size
			public System.UIntPtr  freeram;   // Available memory size
			public System.UIntPtr  sharedram; // Amount of shared memory
			public System.UIntPtr  bufferram; // Memory used by buffers


			// [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.
			public System.UIntPtr  totalswap; // Total swap space size

			public System.UIntPtr  freeswap;  // swap space still available
			public ushort procs;    // Number of current processes

			public System.UIntPtr totalhigh;  // Total high memory size 
			public System.UIntPtr freehigh;  // Available high memory size 
			public uint mem_unit;   // Memory unit size in bytes 

			// Compile-Time - Runtime - TypeSize info restriction: Better too much than too few
			[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = padb)]
			public char[] _f; // Pads structure to 64 bytes
		} // End Struct sysinfo_t


		public class landscape
		{
			private const string LibcIsAFreakingTextFile = @"/lib/x86_64-linux-gnu/libc.so.6";

			private sysinfo_t info;
			private DateTime m_dtSystemStart;

			[System.Runtime.InteropServices.DllImport(LibcIsAFreakingTextFile)]
			private static extern int sysinfo(ref sysinfo_t info);  


			public landscape()
			{
				this.info = new sysinfo_t();
				sysinfo(ref this.info);
				this.m_dtSystemStart = System.DateTime.Now.AddSeconds( - this.UpTime.TotalSeconds );
			} // End Constructor 


			public TimeSpan UpTime
			{
				get
				{
					UInt64 upt = info.uptime.ToUInt64();

					int hours = (int) (upt/3600);
					int mins = (int) (upt%3600/60);
					int secs = (int) (upt%60);

					TimeSpan ts = new TimeSpan(hours, mins, secs);
					return ts;
				}

			} // End Property UpTime


			public DateTime SystemStartTime
			{
				get
				{
					return this.m_dtSystemStart;
				}
			} // End Property SystemStartTime


			public ushort NumberOfProcesses
			{
				get
				{
					return info.procs;
				}
			} // End Property NumberOfProcesses


			public ulong OneMinuteLoadAverage
			{
				get
				{
					return info.loads[0].ToUInt64();
				}
			} // End Property OneMinuteLoadAverage


			public ulong FiveMinutesLoadAverage
			{
				get
				{
					return info.loads[1].ToUInt64();
				}
			} // End Property FiveMinutesLoadAverage


			public ulong FifteenMinutesLoadAverage
			{
				get{

					return info.loads[2].ToUInt64();
				}
			} // End Property FifteenMinutesLoadAverage


			public ulong SharedRAM
			{
				get
				{
					return info.sharedram.ToUInt64();
				}
			}


			public ulong BufferRAM
			{
				get
				{
					return info.bufferram.ToUInt64();
				}
			}


			public ulong TotalSwap
			{
				get
				{
					return info.totalswap.ToUInt64();
				}
			}


			public ulong FreeSwap
			{
				get
				{
					return info.freeswap.ToUInt64();
				}
			}


			public ulong FreeSwapQuota
			{
				get
				{
					if(this.TotalSwap == 0)
						return 0; 

					return this.FreeSwap * 100 / this.TotalSwap;
				}
			}


			public ulong TotalRAM
			{
				get
				{
					return info.totalram.ToUInt64();
				}
			}


			public ulong FreeRAM
			{
				get
				{
					return info.freeram.ToUInt64();
				}
			}


			public ulong FreeRamQuota
			{
				get
				{
					if(this.TotalRAM == 0)
						return 0; 

					return this.FreeRAM * 100 / this.TotalRAM;
				}
			}


			public ulong TotalHighMemory
			{
				get
				{
					return info.totalhigh.ToUInt64();
				}
			}


			public ulong FreeHighMemory
			{
				get
				{
					return info.freehigh.ToUInt64();
				}
			}


			public ulong FreeHighMemoryQuota
			{
				get
				{
					if(this.TotalHighMemory == 0)
						return 0; 

					return this.FreeHighMemory * 100 / this.TotalHighMemory;
				}

			}


			// Memory unit size in bytes 
			public uint MemoryUnitSize
			{
				get{
					return info.mem_unit;
				}

			}


		} // End Class landscape

		public static void GetAllText()
		{
			string URL = @"http://www.daniel-steiger.ch/profile";
			HtmlAgilityPack.HtmlWeb hw = new HtmlAgilityPack.HtmlWeb();

			hw.UserAgent = "Lord Vishnu/Transcendental (Vaikuntha;Supreme Personality of Godness)";
			hw.UsingCache = false;
			hw.UseCookies = true;

			//HtmlAgilityPack.HtmlDocument doc = hw.Load("http://www.daniel-steiger1.ch/NonExistant");
			HtmlAgilityPack.HtmlDocument doc = hw.Load(URL);

			/*
			// Remove scripts
			var nodes = doc.DocumentNode.SelectNodes("//script|//style");
			foreach(var mynode in nodes)
			{
				mynode.ParentNode.RemoveChild(mynode);
			}

			string outHtml = doc.DocumentNode.OuterHtml;
			*/

			foreach(HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("//text()"))
			{
				string it = node.InnerText;
				if(string.IsNullOrEmpty(it))
					continue;

				it = it.Replace("&nbsp;", " ");

				it = it.Trim(new char[]{' ', '\t', '\r', '\n'});

				if(string.IsNullOrEmpty(it))
					continue;

				if(StringComparer.OrdinalIgnoreCase.Equals(node.ParentNode.Name, "script"))
					continue;


				if(StringComparer.OrdinalIgnoreCase.Equals(node.ParentNode.Name, "style"))
					continue;

				Console.WriteLine(node.ParentNode.Name);

				Console.WriteLine(it);
			}


			// doc.Save("out.html");
		}


        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
			landscape sysinfo = new landscape();

			GetAllText();



			Console.WriteLine(sysinfo.NumberOfProcesses);
			Console.WriteLine(sysinfo.UpTime);
			Console.WriteLine(sysinfo.TotalRAM);
			Console.WriteLine(sysinfo.FreeRAM);
			Console.WriteLine(sysinfo.BufferRAM);
			Console.WriteLine(sysinfo.TotalHighMemory);
			Console.WriteLine(sysinfo.FreeHighMemory);
			Console.WriteLine(sysinfo.FreeHighMemoryQuota);
			Console.WriteLine(sysinfo.TotalSwap);
			Console.WriteLine(sysinfo.FreeSwap);
			Console.WriteLine(sysinfo.FreeSwapQuota);
			Console.WriteLine(sysinfo.FreeRamQuota);
			Console.WriteLine(sysinfo.SharedRAM);
			Console.WriteLine(sysinfo.SystemStartTime);
			Console.WriteLine(sysinfo.OneMinuteLoadAverage);
			Console.WriteLine(sysinfo.FiveMinutesLoadAverage);
			Console.WriteLine(sysinfo.FifteenMinutesLoadAverage);
			Console.WriteLine(sysinfo.MemoryUnitSize);

			double lol = (double) sysinfo.TotalRAM / 1024 / 1024 / 1204;
			Console.WriteLine(lol);



            bool bShowForm = false;

            if(bShowForm)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
			} // End if(bShowForm)


			// http://arachnode.net/
			// https://github.com/sjdirect

			string[] urls = GetSiteUrls("http://www.daniel-steiger.ch");
			if(urls.Length > 0)
			{
				Console.WriteLine("The following URLs found:");
				Console.WriteLine(string.Join(Environment.NewLine, urls));
			}
			else
				Console.WriteLine("No URLs found.");




            //const string url = "http://google.com";

            //HtmlAgilityPack.HtmlWeb page = new HtmlAgilityPack.HtmlWeb();
            //HtmlAgilityPack.HtmlDocument document = page.Load(url);
            //page.Get(url, "/");
            //document.Save("out.html");

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(" --- Press any key to continue --- ");
            Console.ReadKey();
		} // End Sub Main


	} // End Class Program


} // End Namespace WebCrawler 
