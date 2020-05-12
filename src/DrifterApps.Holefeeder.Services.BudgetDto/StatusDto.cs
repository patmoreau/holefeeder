using System;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class StatusDto
    {
        public string Version { get; }

        public string AssemblyFileVersion { get; }

        public string AssemblyInformationalVersion { get; }
        
        public DateTime ServerDateTime { get; }

        public StatusDto(string version, string assemblyFileVersion, string assemblyInformationalVersion)
        {
            Version = version;
            AssemblyFileVersion = assemblyFileVersion;
            AssemblyInformationalVersion = assemblyInformationalVersion;
            ServerDateTime = DateTime.Now;
        }
    }
}
