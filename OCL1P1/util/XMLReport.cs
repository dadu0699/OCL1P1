using OCL1P1.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCL1P1.util
{
    class XMLReport
    {
        private FileStream fileStream;
        private StreamWriter streamWriter;

        public XMLReport()
        {
        }

        public void generateReport(string filename, List<Token> listTokens)
        {
            /* fileStream = null;
            streamWriter = null; */
            fileStream = new FileStream(filename, FileMode.Create);
            streamWriter = new StreamWriter(fileStream, Encoding.UTF8);

            streamWriter.WriteLine("<ListaTokens>");

            foreach (var item in listTokens)
            {
                streamWriter.WriteLine("\t<Token>");
                streamWriter.WriteLine("\t\t<Nombre>" + item.TypeToken + "</Nombre>");
                streamWriter.WriteLine("\t\t<Valor>" + item.Value + "</Valor>");
                streamWriter.WriteLine("\t\t<Fila>" + item.Row + "</Fila>");
                streamWriter.WriteLine("\t\t<Columna>" + item.Column + "</Columna>");
                streamWriter.WriteLine("<\t/Token>");
            }

            streamWriter.WriteLine("</ListaTokens>");
            streamWriter.Close();
            fileStream.Close();
        }

        public void generateReport(string filename, List<Error> listError)
        {
            // streamWriter = null;
            fileStream = new FileStream(filename, FileMode.Create);
            streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            streamWriter.WriteLine("<ListaErrores>");

            foreach (var item in listError)
            {
                streamWriter.WriteLine("\t<Error>");
                streamWriter.WriteLine("\t\t<Valor>" + item.Character + "</Valor>");
                streamWriter.WriteLine("\t\t<Fila>" + item.Row + "</Fila>");
                streamWriter.WriteLine("\t\t<Columna>" + item.Column + "</Columna>");
                streamWriter.WriteLine("\t</Error>");
            }

            streamWriter.WriteLine("</ListaErrores>");
            streamWriter.Close();
            fileStream.Close();
        }
    }
}
