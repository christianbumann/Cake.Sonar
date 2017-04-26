﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Sonar
{
    public class SonarRunner : Tool<SonarSettings> 
    {
        public SonarRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IGlobber globber) : base(fileSystem, environment, processRunner, globber)
        {
        }

        public SonarRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools) : base(fileSystem, environment, processRunner, tools)
        {
        }

        protected override string GetToolName()
        {
            return "SonarQube";
        }

        protected override IEnumerable<string> GetToolExecutableNames()
        {
            return new[] {"SonarQube.Scanner.MSBuild.exe"};
        }

        public void Run(SonarSettings settings)
        {
            Run(settings, settings.GetArguments(), new ProcessSettings { RedirectStandardOutput = settings.Silent }, PostAction);
        }

        protected void PostAction(IProcess process)
        {
            var lastLine = process.GetStandardOutput().LastOrDefault();
            if (!string.IsNullOrEmpty(lastLine) && lastLine.Contains("Exit code: 1"))
            {
                var exitCode = process.GetExitCode();
                throw new Exception($"Sonar failure (exit code {exitCode}) . Check the logs.");
            }

        }
    }
}