using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using static System.Environment;


namespace mRemoteNG.App.Info
{
	public static class GeneralAppInfo
	{
		public static readonly string UrlHome = "http://www.mremoteng.org/";
		public static readonly string UrlDonate = "http://donate.mremoteng.org/";
		public static readonly string UrlForum = "http://forum.mremoteng.org/";
		public static readonly string UrlBugs = "http://bugs.mremoteng.org/";
	    public static readonly string ApplicationVersion = Application.ProductVersion;
        public static readonly string ProductName = Application.ProductName;
        public static readonly string Copyright = ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute), false)).Copyright;
        public static readonly string HomePath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
		public static string ReportingFilePath = "";
		public static readonly string PuttyPath = HomePath + "\\PuTTYNG.exe";
        public static string UserAgent
		{
			get
			{
			    var details = new List<string>();
			    details.Add("compatible");
			    details.Add(OSVersion.Platform == PlatformID.Win32NT ? $"Windows NT {OSVersion.Version.Major}.{OSVersion.Version.Minor}": OSVersion.VersionString);
			    if (Tools.EnvironmentInfo.IsWow64)
				{
					details.Add("WOW64");
				}
				details.Add(Thread.CurrentThread.CurrentUICulture.Name);
				details.Add($".NET CLR {Environment.Version}");
				var detailsString = string.Join("; ", details.ToArray());
						
				return $"Mozilla/5.0 ({detailsString}) {ProductName}/{ApplicationVersion}";
			}
		}

	    public static Version GetApplicationVersion()
	    {
            Version v;
            System.Version.TryParse(ApplicationVersion, out v);
	        return v;
	    }
	}
}