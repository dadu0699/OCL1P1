﻿using OCL1P1.model;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace OCL1P1.util
{
    class DFAReport
    {
        private string route;
        private StringBuilder graph;

        public DFAReport()
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

        public void ReportDFA(string name, List<Transition> transitions)
        {
            graph = new StringBuilder();
            string rdot = route + "\\" + name.Replace(" ", "") + ".dot";
            string rpng = route + "\\" + name.Replace(" ", "") + ".png";

            graph.Append("digraph G {");
            graph.Append("\n\tgraph [rankdir = LR, label=\"" + name + "\", labelloc=t, fontsize=30];");
            graph.Append("\n\tnode [shape = circle, height = 0.5, fixedsize = true, fontsize = 14];");

            foreach (Transition item in transitions.OrderBy(x => x.From.StateName))
            {
                if (item.From != null)
                {
                    if (item.From.IsEnd)
                    {
                        graph.Append("\n\t" + item.From.StateName + " [shape = doublecircle];");
                    }
                    graph.Append("\n\t" + item.From.StateName);
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
                    graph.AppendLine("[label=\"" + item.Token.Value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"];");
                }
                else
                {
                    graph.AppendLine("[label=\"&epsilon;\"];");
                }

                if (item.To.IsEnd)
                {
                    graph.Append("\t" + item.To.StateName + " [shape = doublecircle];");
                }
            }

            graph.Append("\n\t\"\"[shape = none];");
            graph.Append("\n\t\"\"");
            graph.Append("->" + "S0");

            graph.Append("\n}");
            generateDotPNG(rdot, rpng);
        }
    }
}
