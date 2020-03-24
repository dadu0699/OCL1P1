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
    class SubsetReport
    {
        private string route;
        private StringBuilder graph;

        public SubsetReport()
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

        public void ReportSubset(string name, string[,] statesMatrix)
        {
            graph = new StringBuilder();
            string rdot = route + "\\" + name.Replace(" ", "") + ".dot";
            string rpng = route + "\\" + name.Replace(" ", "") + ".png";

            graph.Append("digraph G {");
            graph.Append("\n\tgraph [rankdir = LR, label=\"" + name + "\", " +
                "labelloc=t, fontsize=30, pad=0.5, nodesep=0.5, ranksep=2];");
            graph.Append("\n\tnode [shape=none];");
            graph.Append("\n\ttable [label=<");
            graph.Append("\n\t\t<table border=\"0\" cellborder=\"1\" cellspacing=\"0\">");


            for (int i = 0; i < statesMatrix.GetLength(0); i++)
            {
                graph.Append("\n\t\t\t<tr>");
                for (int j = 0; j < statesMatrix.GetLength(1); j++)
                {
                    if (statesMatrix[i, j] != null && statesMatrix[i, j].Contains('#') && i > 0)
                    {
                        graph.Append("\n\t\t\t\t<td bgcolor=\"burlywood1\" > " + statesMatrix[i, j].Replace("#", "") + "</td>");
                    }
                    else if (statesMatrix[i, j] != null)
                    {
                        graph.Append("\n\t\t\t\t<td>" + statesMatrix[i, j].Replace("<", "&lt;").Replace(">", "&gt;") + "</td>");
                    }
                    else
                    {
                        graph.Append("\n\t\t\t\t<td>" + statesMatrix[i, j] + "</td>");
                    }
                }
                graph.Append("\n\t\t\t</tr>");
            }

            graph.Append("\n\t\t</table>");

            graph.Append("\n\t>];\n}");
            generateDotPNG(rdot, rpng);
        }
    }
}
