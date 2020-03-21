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
            graph = new StringBuilder();
            string rdot = route + "\\" + name.Replace(" ", "") + ".dot";
            string rpng = route + "\\" + name.Replace(" ", "") + ".png";

            graph.Append("digraph G {");
            graph.Append("\n\tgraph [rankdir = LR, label=\"AFN: " + name + "\", labelloc=t, fontsize=30];");
            graph.Append("\n\tnode [shape = circle, height = 0.5, fixedsize = true, fontsize = 14];");

            Transition last = transitions.Last();
            foreach (Transition item in transitions)
            {
                if (item.From != null)
                {
                    graph.Append("\t" + item.From.StateName);
                }
                else
                {
                    graph.Append("\n\t\"\"[shape = none];");
                    graph.Append("\n\t\"\"");
                }

                if (item.To != null)
                {
                    graph.Append("->" + item.To.StateName);
                }

                if (item.Token != null)
                {
                    graph.AppendLine("[label=\"" + item.Token.Value.Replace("\"", "").Replace("\\", "\\\\") + "\"];");
                }
                else
                {
                    graph.AppendLine("[label=\"&epsilon;\"];");
                }

                if (item.Equals(last))
                {
                    graph.Append("\t" + item.To.StateName + " [shape = doublecircle];");
                }
            }

            graph.Append("\n}");
            generateDotPNG(rdot, rpng);
        }
    }
}
