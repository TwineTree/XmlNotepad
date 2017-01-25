namespace DiffTest
{
    using XmlNotepad;
    using Microsoft.XmlDiffPatch;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml;
    using System.Xml.Linq;
    using System.Text;

    class Program
    {
        static void Main(string[] args)
        {
            //DiffXml();
            DiffXmlTest();

            Console.ReadLine();
        }

        private static void DiffXmlTest()
        {
            XmlDocument original = new XmlDocument();
            XmlDocument doc = new XmlDocument();
            var filename = @"C:\Users\risha\Desktop\test0.xml";
            var changed = @"C:\Users\risha\Desktop\test1.xml";

            using (XmlReader reader = XmlReader.Create(@"C:\Users\risha\Desktop\test0.xml"))
            {
                original.Load(reader);
            }

            XmlReaderSettings settings = GetReaderSettings();
            using (XmlReader reader = XmlReader.Create(@"C:\Users\risha\Desktop\test1.xml", settings))
            {
                doc.Load(reader);
            }

            //string startupPath = Application.StartupPath;
            ////output diff file.
            //string diffFile = Path.Combine(Path.GetTempPath(),
            //    Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".xml");
            //this.tempFiles.AddFile(diffFile, false);

            Stream diffFile = new MemoryStream();

            bool isEqual = false;
            XmlTextWriter diffWriter = new XmlTextWriter(diffFile, Encoding.UTF8);
            diffWriter.Formatting = Formatting.Indented;
            //using (diffWriter)
            //{
            XmlDiff diff = new XmlDiff();
            isEqual = diff.Compare(original, doc, diffWriter);
            diff.Options = XmlDiffOptions.None;
            //}

            if (isEqual)
            {
                //This means the files were identical for given options.
                //MessageBox.Show(this, SR.FilesAreIdenticalPrompt, SR.FilesAreIdenticalCaption,
                //    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //string tempFile = Path.Combine(Path.GetTempPath(),
            //    Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".htm");
            //tempFiles.AddFile(tempFile, false);

            var tempFile = new MemoryStream();
            var tempFileRight = new MemoryStream();

            diffFile.Position = 0;
            using (XmlReader diffGram = XmlReader.Create(diffFile, settings))
            {
                XmlDiffView diffView = new XmlDiffView();
                diffView.Load(new XmlTextReader(filename), diffGram);
                using (TextWriter htmlWriter = new StreamWriter(
                    tempFile, Encoding.UTF8))
                {
                    var htmlWriterRight = new StreamWriter(tempFileRight, 
                        Encoding.UTF8);

                    //SideBySideXmlNotepadHeader(filename, changed, htmlWriter);
                    diffView.GetHtml(htmlWriter, htmlWriterRight);
                    //htmlWriter.WriteLine("</body></html>");
                    htmlWriter.Flush();
                    htmlWriterRight.Flush();
                    tempFile.Position = 0;
                    tempFileRight.Position = 0;

                    var sr = new StreamReader(tempFile);
                    var html = sr.ReadToEnd();
                    var srRight = new StreamReader(tempFileRight);
                    var htmlRight = srRight.ReadToEnd();

                    string noHTML = Regex.Replace(html, @"&nbsp;", " ");
                    html = HttpUtility.HtmlDecode(noHTML);
                    string noHTMLRight = Regex.Replace(htmlRight, @"&nbsp;", " ");
                    htmlRight = HttpUtility.HtmlDecode(noHTMLRight);

                    var diffHtml = string.Format(
                            @"<html><head>{0}</head><body><table><tr><td>{1}</td><td>{2}</td></tr></table></body></html>",
                            @"<script src='svg.js' ></script>",
                            html,
                            htmlRight);

                    File.WriteAllText(@"C:\wazo_studio\diff_test\files\result.html", diffHtml);
                    Console.WriteLine(html);
                }
            }
        }

        private static XmlReaderSettings GetReaderSettings()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ProhibitDtd = false;
            settings.CheckCharacters = true;
            settings.IgnoreWhitespace = false;
            settings.ConformanceLevel = ConformanceLevel.Document;
            return settings;
        }

        private static void DiffXml()
        {
            var textFiles = ReadTextFromFiles();
            var xml1 = textFiles[0];
            var xml2 = textFiles[1];

            var node1 = XElement.Parse(xml1).CreateReader();
            var node2 = XElement.Parse(xml2).CreateReader();

            var result = new XDocument();
            var writer = result.CreateWriter();
            var diff = new XmlDiff();

            diff.Compare(node1, node2, writer);
            writer.Flush();
            writer.Close();

            MemoryStream originalStream = new MemoryStream();
            StreamWriter originalStreanWriter = new StreamWriter(originalStream);
            originalStreanWriter.Write(xml1);
            originalStreanWriter.Flush();
            originalStream.Position = 0;

            MemoryStream diffStream = new MemoryStream();
            StreamWriter diffStreanWriter = new StreamWriter(originalStream);
            diffStreanWriter.Write(result.ToString());
            diffStreanWriter.Flush();
            diffStream.Position = 0;
            originalStream.Position = 0;

            var originalNode = XElement.Parse(xml1).CreateReader();
            var diffNode = XElement.Parse(result.ToString()).CreateReader();

            XmlNamespaceManager manager = new XmlNamespaceManager(originalNode.NameTable);
            manager.AddNamespace("xlink", "http://www.w3.org/1999/xlink");
            manager.AddNamespace("svg", "http://www.w3.org/2000/svg");

            XmlTextReader orig = new XmlTextReader(originalStream, manager.NameTable);
            XmlTextReader diffGram = new XmlTextReader(diffStream);

            Console.WriteLine(result.ToString());

            XmlDiffView dv = new XmlDiffView();
            dv.Load(orig,
                diffNode);

            MemoryStream htmlStream = new MemoryStream();
            StreamWriter tw = new StreamWriter(htmlStream);
            //dv.GetHtml(tw);
            tw.Flush();

            htmlStream.Position = 0;
            StreamReader reader = new StreamReader(htmlStream);
            string readerString = reader.ReadToEnd();
            string noHTML = Regex.Replace(readerString, @"&nbsp;", " ").Trim();

            string html = HttpUtility.HtmlDecode(noHTML);

            xml1 = string.Format(
                @"<html><head>{0}</head><body>{1}</body></html>",
                @"<script src='svg.js' ></script>",
                xml1);

            html = string.Format(
                @"<html><head>{0}</head><body>{1}</body></html>",
                @"<script src='svg.js' ></script>",
                html);

            File.WriteAllText(@"C:\wazo_studio\diff_test\files\result-old.html", xml1);
            File.WriteAllText(@"C:\wazo_studio\diff_test\files\result.html", readerString);
            File.WriteAllText(@"C:\Users\risha\Documents\Visual Studio 2015\Projects\Diff\Diff\data\result.xml", html);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml1);
        }

        private void SideBySideXmlNotepadHeader(
            string sourceXmlFile,
            string changedXmlFile,
            TextWriter resultHtml)
        {

            // this initializes the html
            resultHtml.WriteLine("<html><head>");
            resultHtml.WriteLine("<style TYPE='text/css'>");
            resultHtml.WriteLine(GetEmbeddedString("XmlNotepad.Resources.XmlReportStyles.css"));
            resultHtml.WriteLine("</style>");
            resultHtml.WriteLine("</head>");
            resultHtml.WriteLine(GetEmbeddedString("XmlNotepad.Resources.XmlDiffHeader.html"));

            //resultHtml.WriteLine(string.Format(SR.XmlDiffBody,
            //        System.IO.Path.GetDirectoryName(sourceXmlFile),
            //        System.IO.Path.GetFileName(sourceXmlFile),
            //        System.IO.Path.GetDirectoryName(changedXmlFile),
            //        System.IO.Path.GetFileName(changedXmlFile)
            //));

        }

        string GetEmbeddedString(string name)
        {
            using (Stream stream = typeof(XmlNotepad.FormMain).Assembly.GetManifestResourceStream(name))
            {
                StreamReader sr = new StreamReader(stream);
                return sr.ReadToEnd();
            }
        }

        static List<string> ReadTextFromFiles()
        {
            var oldText = File.ReadAllText(@"C:\wazo_studio\diff_test\files\0.txt");
            var newText = File.ReadAllText(@"C:\wazo_studio\diff_test\files\1.txt");

            return new List<string>() { oldText, newText };
        }
    }
}
