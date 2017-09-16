using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.IO;

namespace ConsoleApplication1
{
    class http_server
    {
        private HttpListener listner;
        private bool flag = true;
        private const int bufferSize = 1024 * 512;

        public void server_start()
        {
            if (listner == null)
            {
                listner = new HttpListener();
            }
            if (listner.IsListening)
            {
                throw new Exception("Server is alive.");
            }
            listner.Prefixes.Add(network.Default.Scheme + "://" + network.Default.IP + ":" + network.Default.Port + "/");
            listner.Start();
            listner.BeginGetContext(new AsyncCallback(ListenerCallback), listner);
            //client_work(listner);
        }

        public string server_status()
        {
            if (listner == null)
            {
                return("Server shutdown.");
            }
            if (listner.IsListening)
            {
                return ("Server listening.");
            }
            return ("Server shutdown.");
        }

        public void server_stop()
        {
            flag = false;
            
            listner.Stop();
        }

        public void server_close()
        {
            flag = false;
            listner.Close();            
        }

        public void client_work(HttpListener temp)
        {
            while (flag)
            {
                IAsyncResult result = temp.BeginGetContext(new AsyncCallback(ListenerCallback), temp);
                result.AsyncWaitHandle.WaitOne();
            }
                    
        }

        public void ListenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);
            listner.BeginGetContext(new AsyncCallback(ListenerCallback), listner);

            Task srver_resp = new Task(() => http_server.server_response(context));
            srver_resp.Start();
        }

        public static void server_response(HttpListenerContext temp)
        {
            HttpListenerRequest request = temp.Request;
            HttpListenerResponse response = temp.Response;

            var url = tuneUrl(request.RawUrl);
            var fullPath = string.IsNullOrEmpty(url) ? network.Default.rootPath : Path.Combine(network.Default.rootPath, url);
            if (Directory.Exists(fullPath))
                returnDirContents(response, fullPath);
            else if (File.Exists(fullPath))
                returnFile(response, fullPath);
            else
                return404(response);

            //string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            //response.ContentLength64 = buffer.Length;
            //System.IO.Stream output = response.OutputStream;
            //output.Write(buffer, 0, buffer.Length);
            //output.Close();
        }

        private static void returnDirContents(HttpListenerResponse response, string dirPath)
        {

            response.ContentType = "text/html";
            response.ContentEncoding = Encoding.UTF8;
            using (var sw = new StreamWriter(response.OutputStream))
            {
                sw.WriteLine("<html>");
                sw.WriteLine("<head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head>");
                sw.WriteLine("<body><ul>");

                var dirs = Directory.GetDirectories(dirPath);
                foreach (var d in dirs)
                {
                    var link = d.Replace(network.Default.rootPath, "").Replace('\\', '/');
                    sw.WriteLine("<li>&lt;DIR&gt; <a href=\"" + link + "\">" + Path.GetFileName(d) + "</a></li>");
                }

                var files = Directory.GetFiles(dirPath);
                foreach (var f in files)
                {
                    var link = f.Replace(network.Default.rootPath, "").Replace('\\', '/');
                    sw.WriteLine("<li><a href=\"" + link + "\">" + Path.GetFileName(f) + "</a></li>");
                }

                sw.WriteLine("</ul></body></html>");
            }
            response.OutputStream.Close();
        }

        private static void returnFile(HttpListenerResponse response, string filePath)
        {
            var mime = new MimeSharp.Mime();
            response.ContentType = mime.Lookup(Path.GetExtension(filePath));
            var buffer = new byte[bufferSize];
            using (var fs = File.OpenRead(filePath))
            {
                response.ContentLength64 = fs.Length;
                int read;
                while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                    response.OutputStream.Write(buffer, 0, read);
            }

            response.OutputStream.Close();
        }

        private static void return404(HttpListenerResponse response)
        {
            response.StatusCode = 404;
            response.Close();
        }

        private static string tuneUrl(string url)
        {
            url = url.Replace('/', '\\');
            url = WebUtility.UrlDecode(url);
            url = url.Substring(1);
            return url;
        }
    }
}
