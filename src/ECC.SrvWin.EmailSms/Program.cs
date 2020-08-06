﻿using System;
using ECC.Servicos;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace ECC.SrvWin.EmailSms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>



        static void Main(string[] args)
        {
            var exePath = Assembly.GetExecutingAssembly().Location;
            var selfInstaller = new InstallWindowsService();

            if (args.Any())
            {
                if (args[0].Length > 1
                    && (args[0][0] == '-' || args[0][0] == '/'))
                {
                    switch (args[0].Substring(1).ToLower())
                    {
                        default:
                            break;
                        case "install":
                        case "i":
                            selfInstaller.InstallMe(exePath, args[1].Substring(1));
                            break;
                        case "uninstall":
                        case "u":
                            selfInstaller.UninstallMe(exePath);
                            break;
                    }
                }
            }

            ServiceBase.Run(new ServiceBase[] { new EmailSmsService() });

        }
    }
}
