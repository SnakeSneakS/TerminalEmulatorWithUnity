using System;
using System.Security.Authentication;

public partial class Login
{
    /*
    public static string LOCAL_HOST;

    public static void ConnectSSH()
    {

    }

    public class SSHRequestor
    {
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string[] Commands { get; set; }

        public SSHRequestor(string host, string userName, string password)
        {
            Host = host;
            UserName = userName;
            Password = password;
        }

        public void Run()
        {
            using (var client = new SshClient(Host, UserName, Password))
            {
                client.Connect();
                if (!client.IsConnected) throw new AuthenticationException();
                Array.ForEach(Commands, x => Console.WriteLine(client.CreateCommand(x).Execute()));
                client.Disconnect();
            }
        }
    }
    */
}
