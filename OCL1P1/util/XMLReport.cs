using OCL1P1.model;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace OCL1P1.util
{
    class XMLReport
    {
        private FileStream fileStream;
        private StreamWriter streamWriter;

        public XMLReport()
        {
        }

        public void ReportToken(List<Token> listTokens)
        {
            string filename = "Tokens.xml";
            fileStream = new FileStream(filename, FileMode.Create);
            streamWriter = new StreamWriter(fileStream, Encoding.UTF8);

            streamWriter.WriteLine("<ListaTokens>");

            foreach (Token item in listTokens)
            {
                streamWriter.WriteLine("\t<Token>");
                streamWriter.WriteLine("\t\t<Nombre>" + item.TypeToken + "</Nombre>");
                streamWriter.WriteLine("\t\t<Valor>" + item.Value.Replace('\n', ' ').Replace('\t', ' ') + "</Valor>");
                streamWriter.WriteLine("\t\t<Fila>" + item.Row + "</Fila>");
                streamWriter.WriteLine("\t\t<Columna>" + item.Column + "</Columna>");
                streamWriter.WriteLine("\t</Token>");
            }

            streamWriter.WriteLine("</ListaTokens>");
            streamWriter.Close();
            fileStream.Close();
            OpenReport(filename);
        }

        public void ReportLexicalErrors(List<Error> listError)
        {
            string filename = "LexicalErrors.xml";
            fileStream = new FileStream(filename, FileMode.Create);
            streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            streamWriter.WriteLine("<ListaErrores>");

            foreach (Error item in listError)
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
            OpenReport(filename);
        }

        public void ReportSyntacticErrors(List<Error> listError)
        {
            string filename = "SyntacticErrors.xml";
            fileStream = new FileStream(filename, FileMode.Create);
            streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            streamWriter.WriteLine("<ListaErroresSintacticos>");

            foreach (Error item in listError)
            {
                streamWriter.WriteLine("\t<Error>");
                streamWriter.WriteLine("\t\t<Valor>" + item.Character + "</Valor>");
                streamWriter.WriteLine("\t\t<Descripcion>" + item.Description + "</Descripcion>");
                streamWriter.WriteLine("\t\t<Fila>" + item.Row + "</Fila>");
                streamWriter.WriteLine("\t\t<Columna>" + item.Column + "</Columna>");
                streamWriter.WriteLine("\t</Error>");
            }

            streamWriter.WriteLine("</ListaErroresSintacticos>");
            streamWriter.Close();
            fileStream.Close();
            OpenReport(filename);
        }

        public void ReportSymbolTable(List<Symbol> listSymbols)
        {
            string filename = "SymbolTable.xml";
            fileStream = new FileStream(filename, FileMode.Create);
            streamWriter = new StreamWriter(fileStream, Encoding.UTF8);

            streamWriter.WriteLine("<TablaDeSimbolos>");

            foreach (Symbol item in listSymbols)
            {
                streamWriter.WriteLine("\t<Simbolo>");
                streamWriter.WriteLine("\t\t<Tipo>" + item.Type + "</Tipo>");
                streamWriter.WriteLine("\t\t<Nombre>" + item.Name + "</Nombre>");
                streamWriter.Write("\t\t<Valor>");
                foreach (Token token in item.Value)
                {
                    streamWriter.Write(token.Value + " ");
                }
                streamWriter.WriteLine("</Valor>");
                streamWriter.WriteLine("\t</Simbolo>");
            }

            streamWriter.WriteLine("</TablaDeSimbolos>");
            streamWriter.Close();
            fileStream.Close();
            OpenReport(filename);
        }

        private void OpenReport(string filename)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "\\" + filename))
            {
                Process.Start(Directory.GetCurrentDirectory() + "\\" + filename);
            }
        }
    }
}
