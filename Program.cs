using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;

namespace NQLauncherCon
{
    class Program
    {
        static void Main(string[] args)
        {
			// The location where the NQMod dir is located
			string NQLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MP_MODSPACK");

            // Get the Location where Civ V is installed from registry, for various reasons
    		string CivKeyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\steam app 8930";
	    	string CivValueName = "InstallLocation";
		    string CivLocation = (String)Registry.GetValue(CivKeyName, CivValueName, "");
            string NQTarget = Path.Combine(CivLocation, "Assets\\DLC\\MP_MODSPACK");
		
		    // GMR's config file, for various reasons
    		string GMRConfigFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GMR\\settings.xml");
	    	XmlDocument GMRConf = new XmlDocument();
		    GMRConf.Load(GMRConfigFile);
    		// The DirectX version as configured in GMR
            XmlNodeList DXVers = GMRConf.SelectNodes("Settings/CivDirectXVersionInt");
			
            string launcherCmd = Path.Combine(CivLocation, "CivilizationV.exe");
            string launcherArgs = "";
            string checkProcess = "";
            if (DXVers[0].InnerText  == "0") {
                launcherArgs = "\\dx9";
                checkProcess = "CivilizationV";
            }
            else if (DXVers[0].InnerText == "1") {
                launcherArgs = "\\dx11";
                checkProcess = "CivilizationV_DX11";
            }
            else if (DXVers[0].InnerText == "2") {
                launcherArgs = "\\win8";
                checkProcess = "CivilizationV_Tablet";
            }

            JunctionPoint.Create(@NQTarget, @NQLocation, true);

            var proc = Process.Start(launcherCmd, launcherArgs);
            proc.WaitForExit();
            Thread.Sleep(1000);
            Process[] pname = Process.GetProcessesByName(checkProcess);
            while (pname.Length == 0) {
                Thread.Sleep(1000);
                pname = Process.GetProcessesByName(checkProcess);
            }

            pname[0].WaitForExit();
            JunctionPoint.Delete(NQTarget);
        }
    }
}
