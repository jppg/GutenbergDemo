using System;
using System.Net;
using System.IO;
using Toxy;
using Toxy.Parsers;

namespace GutenbergDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            WebClient wc = new WebClient();
            var tempFileName = Path.GetTempFileName();
            var fileUrl = "http://path_to_the_file";
            
            wc.DownloadFile(fileUrl, tempFileName);

            //Get the filetype contained on metadata
            var mimeType = wc.ResponseHeaders["content-type"];

            Console.WriteLine(mimeType);
            
            //Get the original filename
            var fileName = "";
            if (!string.IsNullOrEmpty(wc.ResponseHeaders["Content-Disposition"]))
            {
                fileName = wc.ResponseHeaders["Content-Disposition"].Substring(wc.ResponseHeaders["Content-Disposition"].IndexOf("filename=") + 9).Replace("\"", "");
            }

            Console.WriteLine(fileName);
            
            //Get all the metadata tags and their values
            foreach(var hd in wc.ResponseHeaders)
            {
                Console.WriteLine(hd.ToString() + " =>" + wc.ResponseHeaders[hd.ToString()]);
            }

            string body = string.Empty;
            if(fileName.ToLower().EndsWith(".pdf"))
            {
                var pdf = new PDFTextParser(new Toxy.ParserContext(tempFileName));
                body = pdf.Parse();
            }
            else if(fileName.ToLower().EndsWith(".docx"))
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                var docx = new Word2007TextParser(new Toxy.ParserContext(tempFileName));
                body = docx.Parse();
            }
            else if(fileName.ToLower().EndsWith(".rtf"))
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                var rtf = new RTFTextParser(new Toxy.ParserContext(tempFileName));                        
                body = rtf.Parse();
            }
            else if(fileName.ToLower().EndsWith(".doc"))
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                var doc = new Word2003TextParser(new Toxy.ParserContext(tempFileName));
                
                body = doc.Parse();
            }

            Console.WriteLine(body);

        }
    }
}

