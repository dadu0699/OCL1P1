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

        public void ReportNFA(string name, Transition transition)
        {
            this.graph = new StringBuilder();
            string rdot = route + "\\" + name.Replace(" ", "") + ".dot";
            string rpng = route + "\\" + name.Replace(" ", "") + ".png";

            this.graph.Append("digraph G {");
            this.graph.Append("\n\tgraph [rankdir = LR]");
            this.graph.Append("\n\tnode [shape = circle, height = 0.5, fixedsize = true, fontsize = 14];");

            /*foreach (var item in transition.State.EpsilonTransitions)
            {
                this.graph.Append("\n\t" + transition.State.StateName + "->" + item.StateName 
                    + "[label=\"&epsilon\"];");
            }

            foreach (var item in transition.State.Transitions)
            {
                this.graph.Append("\n\t" + transition.State.StateName + "->" + item.State.StateName);
                if (item.Token != null)
                {
                    this.graph.Append("[label=\"" + item.Token.Value + "\"];");
                }
                else
                {
                    this.graph.Append("[label=\"&epsilon;\"];");
                }
            }*/

            this.graph.Append("\n}");
            generateDotPNG(rdot, rpng);
        }
    }
}
