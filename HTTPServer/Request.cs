using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD,
        BAD_REQUEST
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09,
        INVALID_HTTP
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
            this.headerLines = new Dictionary<string, string>();
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            string[] seperatingString = { "\r\n" };
            //TODO:[DONE] parse the receivedRequest using the \r\n delimeter   
            requestLines = requestString.Split(seperatingString, System.StringSplitOptions.None);
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3)
                return false;
            // Parse Request line
            if (!ParseRequestLine()) 
                return false;
            // Validate blank line exists
            if (!ValidateBlankLine())
                return false;

            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines())
                return false;

            return true;
        }

        private bool ParseRequestLine()
        {
            
            int method = 0, URI = 1, httpVersion = 2;
            string requestLine = requestLines[0];
            string[] requestLineTokens = requestLine.Split(' ');


            this.method = parseRequestMethod(requestLineTokens[method]);
            if (this.method == RequestMethod.BAD_REQUEST)
                return false;

            if (!ValidateIsURI(requestLineTokens[URI]))
                return false;
            relativeURI = requestLineTokens[URI];

            this.httpVersion = parseRequestHttpVersion(requestLineTokens[httpVersion]);
            if (this.httpVersion == HTTPVersion.INVALID_HTTP)
                return false;

            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            int headerIndexStart = 1, headerIndexEnd = requestLines.Length - 2;
            int attribute = 0, value = 1;
            string[] seperatingString = { ": " };

            for (int i = headerIndexStart; i < headerIndexEnd; i++)
            {
                
                if (requestLines[i] == "")
                    break;
                string[] headerLine = requestLines[i].Split(seperatingString, StringSplitOptions.None);
                headerLines.Add(headerLine[attribute], headerLine[value]);
            }

            // HTTP 1.1 doesn't allow empty header sections
            if (headerLines.Count < 1 && this.httpVersion == HTTPVersion.HTTP11)
                return false;

            return true;
        }

        private bool ValidateBlankLine()
        {
            //checking for "" instead of \r\n because we split them by \r\n so it wouldn't appear
            return requestLines[requestLines.Length - 2] == "";
        }

        private RequestMethod parseRequestMethod(string text)
        {
            switch(text)
            {
                case "GET":
                    return RequestMethod.GET;
                case "POST":
                    return RequestMethod.POST;
                case "HEAD":
                    return RequestMethod.HEAD;
                default:
                    return RequestMethod.BAD_REQUEST;
                    

            }
        }

        private HTTPVersion parseRequestHttpVersion(string text)
        {
            string[] http = text.Split('/');
            if (http[0] != "HTTP")
                return HTTPVersion.INVALID_HTTP;

            switch (text)
            {
                case "HTTP/1.0":
                    return HTTPVersion.HTTP10;
                case "HTTP/1.1":
                    return HTTPVersion.HTTP11;
                default:
                    return HTTPVersion.HTTP09;
            }
        }


    }
}
