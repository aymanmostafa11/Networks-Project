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
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
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
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            string[] seperatingString = { "\r\n" };
            //TODO: parse the receivedRequest using the \r\n delimeter   
            requestLines = requestString.Split(seperatingString, System.StringSplitOptions.None);
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3 || requestLines.Length > 4)
                return false;
            // Parse Request line
            ParseRequestLine();
            // Validate blank line exists
            ValidateBlankLine();
            // Load header lines into HeaderLines dictionary
            
            return true;
        }

        private bool ParseRequestLine()
        {
            int method = 0, URI = 1, httpVersion = 2;
            string requestLine = requestLines[0];
            string[] requestLineTokens = requestLine.Split(' ');

            if (requestLineTokens[method] != RequestMethod.GET.ToString())
                return false;

            if (!ValidateIsURI(requestLineTokens[URI]))
                return false;


            return false;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            throw new NotImplementedException();
        }

        private bool ValidateBlankLine()
        {
            throw new NotImplementedException();
        }

    }
}
