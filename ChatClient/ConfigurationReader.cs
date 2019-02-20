using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections;

namespace ChatClient
{
    /**
     * Reads config file, sets and keeps options. 
     */
    class ConfigurationReader
    {
        /**
         * Constructor
         */
        public ConfigurationReader(string fileName)
        {
            _fileConfig = fileName;
            //_ipAddress = new byte[] {127, 0, 0, 1};
        }

        public void ParseFile()
        {
            XElement fileContent = XElement.Load(_fileConfig);
            _port = (int)fileContent.Element("portNumber");
            _ipAddress = (string)fileContent.Element("ipAddress");
        }

        public void ParseFile(string fileName) 
        {
            _fileConfig = fileName;
            ParseFile();
        }

        public string  IpAddress
        {
            get { return _ipAddress;}
        }

        public int Port
        {
            get { return _port; }
        }

        private int _port;
        private string _ipAddress;
        private string _fileConfig;
        private string _fileLogger;
    }
}
