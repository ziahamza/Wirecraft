using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Hosting;

namespace Wirecraft.Web.HttpModules
{
    public class ExceptionInterceptor : IHttpModule 
   { 
       static int exceptionCount = 0; 
       static string sourceName = null; 
       static object initLock = new object(); 
       static bool initialized = false;
       public void Init(HttpApplication app) 
       {
           if (!initialized) {
               lock (initLock)
               {
                   if (!initialized)
                   {
                       string webenginePath = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(),
                                  "webengine.dll");
                       FileVersionInfo ver = FileVersionInfo.GetVersionInfo(webenginePath);
                       sourceName = string.Format(CultureInfo.InvariantCulture, "ASP.NET {0}.{1}.{2}.0",
                                                   ver.FileMajorPart, ver.FileMinorPart, ver.FileBuildPart);
                       if (!EventLog.SourceExists(sourceName))
                       { 
                           throw new Exception(String.Format(CultureInfo.InvariantCulture, 
                               "Event Log not available, WE ARE DOOMED!!",
                               sourceName)); 
                       }
                       AppDomain.CurrentDomain.UnhandledException +=  
                             new UnhandledExceptionEventHandler(OnUnhandledException);
                       initialized = true; 
                   } 
               } 
           } 
       }
       public void Dispose() { } 
       public void OnUnhandledException(object o, UnhandledExceptionEventArgs e) 
       {
           if (Interlocked.Exchange(ref exceptionCount, 1) != 0) return;
           string message = "Exception Occured!!";

           Exception currentException = null;
           message += "/r/n message: " + currentException.Message;
           message += "/r/n trace:" + currentException.StackTrace;

           System.IO.File.AppendAllText(HostingEnvironment.ApplicationPhysicalPath + "/App_Data/unhandled_exception.txt", message);
           EventLog Log = new EventLog();
           Log.Source = sourceName; 
           Log.WriteEntry(message.ToString(), EventLogEntryType.Error); 
       } 
   } 
}
