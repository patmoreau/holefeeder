using System;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class StatusDto
    {
        public string Version { get; set; }

        public string AssemblyFileVersion { get; set; }

        public string AssemblyInformationalVersion { get; set; }
        
        public DateTime ServerDateTime { get; set; }
    }
}
