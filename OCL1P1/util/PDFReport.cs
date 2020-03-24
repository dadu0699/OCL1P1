using iTextSharp.text;
using iTextSharp.text.pdf;
using OCL1P1.model;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace OCL1P1.util
{
    class PDFReport
    {
        private string route;

        public PDFReport()
        {
            route = Directory.GetCurrentDirectory();
        }

        public void GeneratePDF(List<Error> listError)
        {
            Document doc = new Document(PageSize.LETTER);
            string pdfRoute = route + "\\" + "LexicalErrors.pdf";
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(pdfRoute, FileMode.Create));

            doc.AddTitle("Lexical Errors");
            doc.AddCreator("Didier Dominguez - 201801266");
            doc.Open();

            Font _standardFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);

            doc.Add(new Paragraph("Lexical Errors"));
            doc.Add(Chunk.NEWLINE);

            PdfPTable pdfTable = new PdfPTable(3);
            pdfTable.WidthPercentage = 100;

            PdfPCell clValue = new PdfPCell(new Phrase("Valor", _standardFont));
            clValue.BorderWidth = 0;
            clValue.BorderWidthBottom = 0.75f;

            PdfPCell clRow = new PdfPCell(new Phrase("Fila", _standardFont));
            clRow.BorderWidth = 0;
            clRow.BorderWidthBottom = 0.75f;

            PdfPCell clColumn = new PdfPCell(new Phrase("Columna", _standardFont));
            clColumn.BorderWidth = 0;
            clColumn.BorderWidthBottom = 0.75f;

            // Añadimos las celdas a la tabla
            pdfTable.AddCell(clValue);
            pdfTable.AddCell(clRow);
            pdfTable.AddCell(clColumn);

            foreach (Error item in listError)
            {
                clValue = new PdfPCell(new Phrase(item.Character, _standardFont));
                clValue.BorderWidth = 0;

                clRow = new PdfPCell(new Phrase(item.Row.ToString(), _standardFont));
                clRow.BorderWidth = 0;

                clColumn = new PdfPCell(new Phrase(item.Column.ToString(), _standardFont));
                clColumn.BorderWidth = 0;

                pdfTable.AddCell(clValue);
                pdfTable.AddCell(clRow);
                pdfTable.AddCell(clColumn);
            }

            doc.Add(pdfTable);

            doc.Close();
            writer.Close();
            OpenReport("LexicalErrors.pdf");
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
