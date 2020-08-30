using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VigilantKJV.Helpers
{
    public class FtpHandler
    {
        const string ftpPathDefault = @"ftp://ftp.newnetdemo.com/Kevin/";
        const string ftpUDefault = @"UPLOADER";
        const string ftpPwDefault = @"UPLOADER";

        public static async Task SendDbToFTP(string fileshortname = "MyKjvDb", string ftpPath = ftpPathDefault, string ftpU = ftpUDefault, string ftpPw = ftpPwDefault, byte[] file = null)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(ftpU, ftpPw);
                    //client.UploadFile(Path.Combine(ftpPath, fileshortname), WebRequestMethods.Ftp.UploadFile, userDbPath);
                    await Task.Run(() => client.UploadDataAsync(new Uri(Path.Combine(ftpPath, fileshortname)), file));
                }
                //FtpWebRequest request = (FtpWebRequest)WebRequest.Create();
                //request.Method = WebRequestMethods.Ftp.UploadFile;
                //request.Credentials = new NetworkCredential(ftpU, ftpPw);
                //request.UseBinary = true;

                //if (file == null)
                //    file = userDatabase();
                ////request.KeepAlive = false;
                ////request.UsePassive = false;
                //var requestStream = request.GetRequestStream();
                //await requestStream.WriteAsync(file, 0, file.Length);
                //requestStream.Close();
                //requestStream.Dispose();
                //var response = request.GetResponse();

                //response.Close();
                //response.Dispose();
                UserDialogs.Instance.Alert("Db Uploaded To FTP!", "Info", "Groovy");
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"Error uploading:\n{ex}", "Info", "Ugh");
            }
        }
        //public static async Task SendDbToFTP(string fileshortname = "MyKjvDb", string ftpPath = ftpPathDefault, string ftpU = ftpUDefault, string ftpPw = ftpPwDefault, byte[] file = null)
        //{
        //    try
        //    {
        //        UserDB userDB = new UserDB();

        //        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Path.Combine(ftpPath, fileshortname));
        //        request.Method = WebRequestMethods.Ftp.UploadFile;
        //        request.Credentials = new NetworkCredential(ftpU, ftpPw);
        //        request.UseBinary = true;

        //        if (file == null)
        //            file = userDatabase();
        //        //request.KeepAlive = false;
        //        //request.UsePassive = false;
        //        var requestStream = request.GetRequestStream();
        //        await requestStream.WriteAsync(file, 0, file.Length);
        //        requestStream.Close();
        //        requestStream.Dispose();
        //        //var response = request.GetResponse();

        //        //response.Close();
        //        //response.Dispose();
        //        UserDialogs.Instance.Alert("Db Uploaded To FTP!", "Info", "Groovy");
        //    }
        //    catch (Exception ex)
        //    {
        //        UserDialogs.Instance.Alert($"Error uploading:\n{ex}", "Info", "Ugh");
        //    }
        //}
    }
}
