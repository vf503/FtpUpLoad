using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FtpUpLoad
{
    class Program
    {
        static void Main(string[] args)
        {
            string strFtpPath = "ftp://203.207.118.110:21";   //ftp地址  
            string strUserName = "chenchen";    //用户名  
            string strPassword = "zjsp";  //密码 
            string strFilesPath = FtpUpLoad.Properties.Settings.Default.LocalPath;
            //
            string[] FileList = GetLatestFiles(@strFilesPath, 1);
            //
            foreach (string FilePath in FileList)
            {
                Boolean flag = false;
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(FilePath);
                flag = UploadFile(fileInfo, strFtpPath, strUserName, strPassword);
                if (flag == true)
                {
                    Console.WriteLine("Success");
                }
                else
                {
                    Console.WriteLine("Fail");
                }
            }
        }
        //
        public static string[] GetLatestFiles(string Path, int count)
        {
            var query = (from f in Directory.GetFiles(Path)
                         let fi = new FileInfo(f)
                         orderby fi.CreationTime descending
                         select fi.FullName).Take(count);
            return query.ToArray();
        }
        //
        /// <summary>  
            /// 上传文件  
            /// </summary>  
            /// <param name="fileinfo">需要上传的文件</param>  
            /// <param name="targetDir">目标路径</param>  
            /// <param name="hostname">ftp地址</param>  
            /// <param name="username">ftp用户名</param>  
            /// <param name="password">ftp密码</param>  
            /// <returns></returns>  
        public static Boolean UploadFile(System.IO.FileInfo fileinfo, string hostname, string username, string password)
        {
            string strExtension = System.IO.Path.GetExtension(fileinfo.FullName);
            string strFileName = "";

            strFileName = fileinfo.Name;    //获取文件的文件名  
            string URI = hostname + "/" + strFileName;

            //获取ftp对象  
            System.Net.FtpWebRequest ftp = GetRequest(URI, username, password);

            //设置ftp方法为上传  
            ftp.Method = System.Net.WebRequestMethods.Ftp.UploadFile;

            //制定文件传输的数据类型  
            ftp.UseBinary = true;
            ftp.UsePassive = true;


            //文件大小  
            ftp.ContentLength = fileinfo.Length;
            //缓冲大小设置为2kb  
            const int BufferSize = 2048;

            byte[] content = new byte[BufferSize - 1 + 1];
            int dataRead;

            //打开一个文件流（System.IO.FileStream)去读上传的文件  
            using (System.IO.FileStream fs = fileinfo.OpenRead())
            {
                try
                {
                    //把上传的文件写入流  
                    using (System.IO.Stream rs = ftp.GetRequestStream())
                    {
                        do
                        {
                            //每次读文件流的2KB  
                            dataRead = fs.Read(content, 0, BufferSize);
                            rs.Write(content, 0, dataRead);
                        } while (!(dataRead < BufferSize));
                        rs.Close();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    ftp = null;
                    ftp = GetRequest(URI, username, password);
                    ftp.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;//删除  
                    ftp.GetResponse();
                    return false;
                }
                finally
                {
                    fs.Close();
                }
            }
        }
        //
        /// <summary>  
            /// 得到ftp对象  
            /// </summary>  
            /// <param name="URI">ftp地址</param>  
            /// <param name="username">ftp用户名</param>  
            /// <param name="password">ftp密码</param>  
            /// <returns>返回ftp对象</returns>  
        private static System.Net.FtpWebRequest GetRequest(string URI, string username, string password)
        {
            //根据服务器信息FtpWebRequest创建类的对象  
            FtpWebRequest result = (FtpWebRequest)FtpWebRequest.Create(URI);
            //提供身份验证信息  
            result.Credentials = new System.Net.NetworkCredential(username, password);
            //result.Credentials = new System.Net.NetworkCredential();  
            //设置请求完成之后是否保持到FTP服务器的控制连接，默认值为true  
            result.KeepAlive = false;
            return result;
        }
    }
}
