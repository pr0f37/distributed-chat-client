using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;

namespace ChatClient
{
    class SystemInformator
    {
        public string getMac()
        {
            string macAddress = string.Empty;
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.Description.Contains("Ethernet"))
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties(); //  .GetIPInterfaceProperties();
                    PhysicalAddress address = adapter.GetPhysicalAddress();
                    byte[] bytes = address.GetAddressBytes();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        macAddress += bytes[i].ToString("X2");
                        if (i != bytes.Length - 1)
                        {
                            macAddress += "-";
                        }
                    }
                }
            }
            try
            {
                if (macAddress == string.Empty)
                    throw new Exception("MAC retrieval failed.") ; 
                return macAddress;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("\nMAC retrieval failed. Please type your ID by yourself:");
                macAddress = Console.ReadLine();
            }
            return macAddress;
        }
    }

   
}
