using System;
using System.Management.Automation;

namespace PowerShellExecuterTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (var powerShellInstance = PowerShell.Create())
            {
                var runner = new ScriptRunner();
                runner.ExecuteScript("param($param1) $d = get-date; $s = 'test string value'; " +
                                     "$d; $s; $param1; get-service", new PsParameter { ParameterName = "param1", ParameterData = "parameter 1 value metod" });

                runner.ExecuteScriptAsync("$s1 = 'test1'; $s2 = 'test2'; $s1; write-error 'some error';start-sleep -s 7; $s2");
               

                Console.ReadKey();
            }

        }
    }
}
