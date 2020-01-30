using System;
using System.IO;
using System.Threading;
using System.Net;


namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //ftp server variables
            String serverAddress = "192.168.0.33";
            String path = "Testing";
            String filename = "accountbalances.txt";
            String username = "administrator";
            String password = "Password1";

            //update balance url
            //String address = "";


            //get the accountbalances.txt file from ftp server
            Console.WriteLine("Downloading....");
            FtpWebRequest request =
                (FtpWebRequest) WebRequest.Create("ftp://" + serverAddress + "/" + path + "/" + filename);
            request.Credentials = new NetworkCredential(username, password);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            using (Stream ftpStream = request.GetResponse().GetResponseStream())

            using (Stream fileStream = File.Create(@"" + filename))
            {
                byte[] buffer = new byte[10240];
                int read;
                while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, read);
                    Console.WriteLine("Downloaded {0} bytes", fileStream.Position);
                }
            }


            //delete the accountbalances.txt file from server
            FtpWebRequest deleteRequest =
                (FtpWebRequest) WebRequest.Create("ftp://" + serverAddress + "/" + path + "/" + filename);

            deleteRequest.Credentials = new NetworkCredential(username, password);

            deleteRequest.Method = WebRequestMethods.Ftp.DeleteFile;

            using (FtpWebResponse deleteResponse = (FtpWebResponse) deleteRequest.GetResponse())
            {
                Console.WriteLine(deleteResponse.StatusDescription);
            }


            //read contents of accountbalances.txt file and send make http het request for account update for each line
            string[] lines = File.ReadAllLines("accountbalances.txt");
            foreach (string line in lines)
            {
                String[] strlist = line.Split(',');

                String accountName = strlist[1];
                String ledgerBalance = strlist[2];
                String availableBalance = strlist[3];

                Console.WriteLine("Account Name: " + accountName +
                                  ", Ledger Balance: " + ledgerBalance +
                                  ", Available Balance: " + availableBalance);

                ///send http get request
                //laravel route: . . . /{ accountName }/{ ledgerBalance }/{ availableBalance }
                string url = "https://stackoverflow.com/questions/943852/how-to-send-an-https-get-request-in-c-sharp";
                //string url = address + "/" + accountName + "/" + ledgerBalance + "/" + availableBalance;
                HttpWebRequest getRequest = (HttpWebRequest) WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse) getRequest.GetResponse();
                Stream resStream = response.GetResponseStream();
                Console.WriteLine(response.StatusDescription);

            }
        }//main
    }//class
}//namespace