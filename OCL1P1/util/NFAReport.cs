using OCL1P1.model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCL1P1.util
{
    class NFAReport
    {
        private string route;
        private StringBuilder graph;

        public NFAReport()
        {
            route = Directory.GetCurrentDirectory();
        }

        private void generateDotPNG(string rdot, string rpng)
        {
            File.WriteAllText(rdot, graph.ToString());
            string dotCommand = "dot.exe -Tpng " + rdot + " -o " + rpng;
            // Console.WriteLine(dotCommand);

            var command = string.Format(dotCommand);
            var startProcess = new ProcessStartInfo("cmd", "/C" + command);
            var process = new Process();

            process.StartInfo = startProcess;
            process.Start();
            process.WaitForExit();
        }

        public void ReportNFA(string name, List<Transition> transitions)
        {
            this.graph = new StringBuilder();
            string rdot = route + "\\" + name.Replace(" ", "") + ".dot";
            string rpng = route + "\\" + name.Replace(" ", "") + ".png";

            this.graph.Append("digraph G {");
            this.graph.Append("\n\tgraph [rankdir = LR];");
            this.graph.Append("\n\tnode [shape = circle, height = 0.5, fixedsize = true, fontsize = 14];");

            foreach (Transition item in transitions)
            {
                if (item.From != null)
                {
                    this.graph.Append("\t" + item.From.StateName);
                }
                else
                {
                    this.graph.Append("\t\"\"[shape = none]");
                    this.graph.Append("\n\t\"\"");
                }

                if (item.To != null)
                {
                    this.graph.Append("->" + item.To.StateName);
                }
                else
                {

                    this.graph.Append("-> \"\"");
                }

                if (item.Token != null)
                {
                    this.graph.AppendLine("[label=\"" + item.Token.Value.Replace("\"", "") + "\"];");
                }
                else
                {
                    this.graph.AppendLine("[label=\"&epsilon;\"];");
                }
            }

            this.graph.Append("\n}");
            generateDotPNG(rdot, rpng);
        }
    }
}
