﻿// IronMeta Copyright © Gordon Tisher 2015

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IronMeta.Matcher;
using IronMeta.Generator;
using System.Diagnostics;
using System.IO;

namespace IronMeta
{
    class Program
    {
        static int Main(string[] args)
        {
            const string message = "IronMeta --force {0} --namespace {1} --input {2} --output {3}: {4}";

            try
            {
                var options = Options.Parse(args);

                var inputInfo = new FileInfo(options.InputFile);
                var outputInfo = new FileInfo(options.OutputFile);

                if (outputInfo.Exists && outputInfo.LastWriteTimeUtc > inputInfo.LastWriteTimeUtc && !options.Force)
                {
                    Console.WriteLine(string.Format(message, options.Force, 
                        options.Namespace, inputInfo.FullName, outputInfo.FullName, 
                        "input is older than output; not generating"));
                    return 0;
                }
                else
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var match = CSharpShell.Process(inputInfo.FullName, outputInfo.FullName, options.Namespace, true);
                    stopwatch.Stop();

                    if (match.Success)
                    {
                        Console.WriteLine(string.Format(message, options.Force, 
                            options.Namespace, inputInfo.FullName, outputInfo.FullName, stopwatch.Elapsed));
                        return 0;
                    }
                    else
                    {
                        Console.Error.WriteLine("IronMeta Syntax Error: " + match.MatchState.LastError);
                        return 1;
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return 2;
            }
        }
    }
}
