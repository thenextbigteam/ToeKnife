﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using Sledge.Settings;
using Sledge.Settings.GameDetection;

namespace Sledge.Sandbox
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CheckGL3();
            Application.Exit();
            return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm());

            var wd = new WonDetector();
            wd.Detect();

            //return;

            QuickStartBootstrap.MapFile = @"D:\Github\sledge\_Resources\RMF\entities.rmf";
            SettingsManager.Read();
            QuickStartBootstrap.Game = SettingsManager.Games.Single(x => x.ID == 1);
            QuickStartBootstrap.Start();
        }

        private struct Usage
        {
            public FileInfo File { get; set; }
            public string Method { get; set; }
            public List<Tuple<string, string>> VersionCategories { get; set; }

            public Usage(FileInfo file, string method) : this()
            {
                File = file;
                Method = method;
                VersionCategories = new List<Tuple<string, string>>();
            }

            public void AddVersionCategory(string version, string category)
            {
                VersionCategories.Add(Tuple.Create(version, category));
            }
        }

        private static void CheckGL3()
        {
            Thread.Sleep(1000);


            Console.Out.Flush();
            Console.WriteLine();
            Console.WriteLine();

            var att = typeof (AutoGeneratedAttribute);
            var type = typeof (OpenTK.Graphics.OpenGL.GL);
            var methods = new List<Tuple<string, string, string>>();
            foreach (var method in type.GetMethods())
            {
                var data = method.GetCustomAttributes(att, false);
                if (data.Length > 0)
                {
                    var ag = (AutoGeneratedAttribute) data[0];
                    //Console.WriteLine(method.Name + " = " + ag.Version + " (" + ag.Category + ")");
                    methods.Add(Tuple.Create(method.Name, ag.Version, ag.Category));
                }
            }
            var loc = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var dir = loc.Directory;
            while (dir != null && dir.GetFiles("Sledge.sln").Length == 0) dir = dir.Parent;
            if (dir == null) return;

            var files = dir.EnumerateFiles("*.cs", SearchOption.AllDirectories);
            var regex = new Regex(@"GL\.([^(]*)\(", RegexOptions.Compiled);

            var usages = new List<Usage>();

            foreach (var f in files)
            {
                try
                {
                    var text = File.ReadAllText(f.FullName);
                    foreach (Match match in regex.Matches(text))
                    {
                        var method = match.Groups[1].Value;
                        var usage = new Usage(f, method);
                        foreach (var mtd in methods.Where(x => x.Item1 == method))
                        {
                            usage.AddVersionCategory(mtd.Item2, mtd.Item3);
                        }
                        usages.Add(usage);
                    }
                }
                catch
                {
                    Console.WriteLine("Failed to read: " + f.FullName);
                }
            }

            var ok = new string[] { "Enable", "Disable", "Uniform1", "Uniform4" };
            foreach (var kv in usages)
            {
                if (!ok.Contains(kv.Method) && kv.VersionCategories.Any(x => float.Parse(x.Item1) > 2.0))
                {
                    Console.Write(kv.File.FullName + " - " + kv.Method + ":");
                    foreach (var vc in kv.VersionCategories)
                    {
                        Console.Write(" -> " + vc.Item1 + " (" + vc.Item2 + ")");
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.Out.Flush();

            Thread.Sleep(1000);
        }
    }
}