using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        const int MAX_BACKLOG = 1000;
        const int MAX_MESSAGE_SIZE = 1024;
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: [DONE] call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: [DONE] initialize this.serverSocket
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(iep);
        }

        public void StartServer()
        {
            // TODO:[DONE] Listen to connections, with large backlog.
            serverSocket.Listen(MAX_BACKLOG);
            // TODO: [DONE] Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: [DONE] accept connections and start thread for each accepted connection.
                Socket clientSocket = serverSocket.Accept();
                Console.WriteLine("Client Accepted, Ip: " + clientSocket.RemoteEndPoint);

                Thread newThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newThread.Start(clientSocket);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: [DONE] Create client socket 
            Socket clientSocket = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSocket.ReceiveTimeout = 0;
            // TODO: [DONE] receive requests in while true until remote client closes the socket.
            byte[] data = new byte[MAX_MESSAGE_SIZE];  
            while (true)
            {
                try
                {
                    // TODO: [DONE] Receive request
                    int recievedMessageLength = clientSocket.Receive(data);
                    // TODO: [DONE] break the while loop if receivedLen==0
                    if (recievedMessageLength == 0)
                        break;
                    // TODO: [DONE] Create a Request object using received request string
                    Request request = new Request(Encoding.ASCII.GetString(data));
                    // TODO: [DONE] Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    // TODO: [DONE] Send Response back to client
                    byte[] responseBytes = Encoding.ASCII.GetBytes(response.ResponseString);
                    clientSocket.Send(responseBytes, 0, response.ResponseString.Length, SocketFlags.None);
                }
                catch (Exception ex)
                {
                    // TODO: [DONE] log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO:[DONE] close client socket
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
            string content = string.Empty;
            string redirected = string.Empty;
            string physicalPath = string.Empty;
            StatusCode statusCode = StatusCode.OK;
            try
            {
                //TODO: check for bad request
                if (!request.ParseRequest())
                {
                    //(ayman)TODO: add logic for handling bad request 
                    statusCode = StatusCode.BadRequest;
                    content = File.ReadAllText(Path.Combine(Configuration.RootPath, Configuration.BadRequestDefaultPageName));
                    return new Response(statusCode, "text/html", content, redirected);
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                physicalPath = Path.Combine(Configuration.RootPath, request.relativeURI);
                //TODO: check file exists
                if(!File.Exists(physicalPath))
                {
                    if (redirected != string.Empty)
                    {
                        statusCode = StatusCode.InternalServerError;
                        physicalPath = Path.Combine(Configuration.RootPath, Configuration.InternalErrorDefaultPageName);
                    }
                    else
                    {
                        statusCode = StatusCode.NotFound;
                        physicalPath = Path.Combine(Configuration.RootPath, Configuration.NotFoundDefaultPageName);
                    }
                }
                //TODO: check for redirect
                
                redirected = GetRedirectionPagePathIFExist(request.relativeURI);
                if(redirected != string.Empty)
                {
                    statusCode = StatusCode.Redirect;
                    physicalPath = Path.Combine(Configuration.RootPath, Configuration.RedirectionDefaultPageName);
                }
                //TODO: read the physical file
                content = LoadDefaultPage(physicalPath);
                // Create OK response
                

            }
            catch (Exception ex)
            {
                // TODO: [DONE] log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                statusCode = StatusCode.InternalServerError;
                physicalPath = Path.Combine(Configuration.RootPath, Configuration.InternalErrorDefaultPageName);

            }
            return new Response(statusCode,"text/html",content,redirected);
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // TODO: using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if(Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                return Configuration.RedirectionRules[relativePath];
            }
            else { 
            return string.Empty;
                 }
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            //string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            string content = "";
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            try
            {
                content = File.ReadAllText(defaultPageName);
                return content;
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
                return string.Empty;
            }
            // else read file and return its contentsr.
            //content = Encoding.ASCII.GetString();
            
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
            }
            catch (Exception ex)
            {
                // TODO: [DONE] log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
