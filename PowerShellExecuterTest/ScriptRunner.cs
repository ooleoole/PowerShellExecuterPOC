using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading;

namespace PowerShellExecuterTest
{
    public sealed class ScriptRunner
    {
        public void ExecuteScript(string script, params PsParameter[] parameters)
        {
            using (var powerShellInstance = PowerShell.Create())
            {
                powerShellInstance.AddScript(script);
                foreach (var parameter in parameters)
                    powerShellInstance.AddParameter(parameter.ParameterName, parameter.ParameterData);

                var output = powerShellInstance.Invoke();
                if (powerShellInstance.Streams.Error.Count > 0)
                {
                    //TODO:Do something
                }
                OutputValue(output);
            }
        }

        public void ExecuteScriptAsync(string script, params PsParameter[] parameters)
        {
            using (var powerShellInstance = PowerShell.Create())
            {
                powerShellInstance.AddScript(script);
                if (parameters != null)
                    foreach (var parameter in parameters)
                        powerShellInstance.AddParameter(parameter.ParameterName, parameter.ParameterData);

                var output = new PSDataCollection<PSObject>();

                output.DataAdded += Output_DataAdded;
                powerShellInstance.Streams.Error.DataAdded += Error_DataAdded;

                var result = powerShellInstance.BeginInvoke<PSObject, PSObject>(null, output);

                while (!result.IsCompleted)
                {
                    Console.WriteLine("Waiting for pipeline to finish...");
                    Thread.Sleep(1000);


                }

                Console.WriteLine("Execution has stopped. The pipeline state: " + powerShellInstance.InvocationStateInfo.State);
                OutputValue(output);
            }
        }

        private void Error_DataAdded(object sender, DataAddedEventArgs e)
        {
            Console.WriteLine("An error was written to the Error stream!");
        }

        private void Output_DataAdded(object sender, DataAddedEventArgs e)
        {
            Console.WriteLine($"Object:{sender.GetType()} added something to output");
        }

        private static void OutputValue(IEnumerable<PSObject> output)
        {
            foreach (var psObject in output)
            {
                if (psObject.BaseObject.GetType().ToString()!="System.ServiceProcess.ServiceController")
                {
                    Console.WriteLine(psObject.BaseObject.GetType().FullName);
                    Console.WriteLine(psObject.BaseObject + "\n");
                }
               
            }
        }
    }

    public class PsParameter
    {
        public string ParameterName { get; set; }
        public string ParameterData { get; set; }

    }
}
