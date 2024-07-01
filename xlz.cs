using System;
using System.Windows.Forms;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Diagnostics;

namespace AGlossaryExtractor
{
    #region XLZ
    internal class Xlz
    {
        public static XNamespace logoportNS = @"urn:logoport:xliffeditor:xliff-extras:1.0";
        public XDocument contentDocument { get; set; }
        public XDocument skeletonDocument { get; set; }
        public string inputFilePath { get; set; }
        public IEnumerable<XElement> AllTransUnits { get; set; }
        public IEnumerable<XElement> TranslatedTransUnits { get; set; }
        public IEnumerable<XElement> TranslatableTransUnits { get; set; }
        public IEnumerable<XElement> NonTranslatableTransUnits { get; set; }
        public IEnumerable<XElement> MachineTranslationTransUnits { get; set; }
        public IEnumerable<XElement> TMTranslationTransUnits { get; set; }

        private string contentXlfName = "content.xlf";

        public Xlz(string inputFilePath)
        {
            this.inputFilePath = inputFilePath;
            LoadLionbridgeXlz();
            AllTransUnits = GetAllTransUnits();
            TranslatableTransUnits = GetTranslatableTransUnits();
            TranslatedTransUnits = GetTranslatedTransUnits();
            NonTranslatableTransUnits = GetNonTranslatableTransUnits();
            MachineTranslationTransUnits = GetMtTransUnits();
            TMTranslationTransUnits = GetTMTransUnits();


        }

        private IEnumerable<XElement> GetAllTransUnits()
        {
            var allUnits = from t in contentDocument.Descendants()
                           where t.Name.LocalName == "trans-unit"
                           select t;

            return allUnits;
        }

        private IEnumerable<XElement> GetTMTransUnits()
        {
            var tmUnits = from t in contentDocument.Descendants()
                          where t.Name.LocalName == "trans-unit" && t.DescendantsAnyNS("target") != null
                          && t.DescendantsAnyNS("target").First().AttributeAnyNS("state-qualifier") != null
                          && t.DescendantsAnyNS("target").First().AttributeAnyNS("state-qualifier").Value == "leveraged-tm"
                          select t;

            return tmUnits;
        }

        private IEnumerable<XElement> GetMtTransUnits()
        {
            var mtUnits = from t in contentDocument.Descendants()
                          where t.Name.LocalName == "trans-unit" && t.DescendantsAnyNS("target") != null
                          && t.DescendantsAnyNS("target").First().AttributeAnyNS("state-qualifier") != null
                          && t.DescendantsAnyNS("target").First().AttributeAnyNS("state-qualifier").Value == "leveraged-mt"
                          select t;

            return mtUnits;
        }


        private IEnumerable<XElement> GetTranslatableTransUnits()
        {
            var translatableTransUnits = from t in contentDocument.Descendants()
                                         where t.Name.LocalName == "trans-unit"
                                         && t.AttributeAnyNS("translate") != null && t.AttributeAnyNS("translate").Value == "yes"
                                         select t;

            return translatableTransUnits;
        }


        private IEnumerable<XElement> GetNonTranslatableTransUnits()
        {
            var nontranslatableTransUnits = from t in contentDocument.Descendants()
                                            where t.Name.LocalName == "trans-unit"
                                            && t.AttributeAnyNS("translate") != null && t.AttributeAnyNS("translate").Value == "no"
                                            select t;

            return nontranslatableTransUnits;
        }

        private IEnumerable<XElement> GetTranslatedTransUnits()
        {
            var translatedTransUnits = from t in contentDocument.Descendants()
                                       where t.Name.LocalName == "trans-unit"
                                       && t.AttributeAnyNS("translate") != null
                                       && t.DescendantsAnyNS("target") != null
                                       select t;

            return translatedTransUnits;
        }




        private void LoadLionbridgeXlz()
        {
            try
            {
                using (ZipArchive xlzArchive = ZipFile.OpenRead(inputFilePath))
                {
                    var contentXlf = xlzArchive.Entries
                        .Where(x => x.Name == "content.xlf")
                        .DefaultIfEmpty(xlzArchive.Entries.First(x => x.Name.EndsWith(".xlf")))
                        .First();

                    contentXlfName = contentXlf.FullName;

                    var skeleton = xlzArchive.Entries
                        .FirstOrDefault(x => x.Name.EndsWith(".skl"));

                    // Content
                    using (Stream stream = contentXlf.Open())
                    {
                        StreamReader reader = new StreamReader(stream);

                        string content = reader.ReadToEnd();

                        //Step 1 - Load source xml into memory stream - it will be used later for creating a reader with correct settings
                        System.Text.Encoding encode = System.Text.Encoding.UTF8;
                        using (MemoryStream ms = new MemoryStream(encode.GetBytes(content)))
                        {
                            //Step 2 - set initial setings
                            XmlReaderSettings settings = new XmlReaderSettings();
                            settings.DtdProcessing = DtdProcessing.Parse;
                            settings.XmlResolver = null;
                            settings.IgnoreWhitespace = false;
                            XmlReader readerXml = XmlReader.Create(ms, settings);

                            //Step 3 - Load source document with validation off                                  
                            XDocument contentXml = XDocument.Load(readerXml, LoadOptions.PreserveWhitespace);

                            contentDocument = contentXml;
                        }
                    }

                    // Skeleton
                    if (skeleton != null)
                    {
                        using (Stream stream = skeleton.Open())
                        {
                            StreamReader reader = new StreamReader(stream);
                            string skeletonContent = reader.ReadToEnd();

                            //Step 1 - Load source xml into memory stream - it will be used later for creating a reader with correct settings
                            System.Text.Encoding encode = System.Text.Encoding.UTF8;

                            using (MemoryStream ms = new MemoryStream(encode.GetBytes(skeletonContent)))
                            {
                                //Step 2 - set initial setings
                                XmlReaderSettings settings = new XmlReaderSettings();
                                settings.DtdProcessing = DtdProcessing.Parse;
                                settings.XmlResolver = null;
                                settings.IgnoreWhitespace = false;

                                XmlReader readerXml = XmlReader.Create(ms, settings);

                                //Step 3 - Load source document with validation off                                  
                                XDocument skeletonXml = XDocument.Load(readerXml, LoadOptions.PreserveWhitespace);

                                skeletonDocument = skeletonXml;
                            }
                        }
                    }
                }
            }

            catch (InvalidDataException)
            {
                //Step 2 - set initial setings
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Parse;
                settings.XmlResolver = null;
                settings.IgnoreWhitespace = false;
                using (var readerXml = XmlReader.Create(inputFilePath, settings))
                {
                    //Step 3 - Load source document with validation off     
                    XDocument contentXml = XDocument.Load(readerXml, LoadOptions.PreserveWhitespace);
                    contentDocument = contentXml;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Please check your file: {0} - this is not a valid archive - possibility to process file outside of TMS may be required:ex {1}", Path.GetFileName(inputFilePath), ex.ToString()));
            }

        }
        public void Save(string saveAsPath = null)
        {
            try
            {
                var pathToSave = saveAsPath == null ? inputFilePath : saveAsPath;
                using (FileStream zipToOpen = new FileStream(pathToSave, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        var content = archive.Entries.Where(x => x.Name == "content.xlf").First();
                        var skeleton = archive.Entries.Where(x => x.Name == "skeleton.skl").First();

                        content.Delete();
                        skeleton.Delete();

                        Encoding utf8Encoding = new UTF8Encoding(false);
                        XmlWriterSettings settings = new XmlWriterSettings();

                        ZipArchiveEntry newEntry = archive.CreateEntry("content.xlf");

                        using (XmlWriter writer = XmlWriter.Create(newEntry.Open(), settings))
                        {
                            if (contentDocument.DocumentType != null)
                            {
                                contentDocument.DocumentType.InternalSubset = null;
                            }

                            contentDocument.Save(writer);

                        }

                        newEntry = archive.CreateEntry("skeleton.skl");

                        using (XmlWriter writer = XmlWriter.Create(newEntry.Open(), settings))
                        {

                            skeletonDocument.Save(writer);

                        }
                    }
                }
            }
            catch (InvalidDataException)
            {
                contentDocument.Save(saveAsPath);
            }
            catch (Exception ex)
            {
                throw new Exception("Something unexpected happend - Please check your file   - possibility to process file outside of TMS may be required  " + ex.ToString());
            }
        }

        public void Save2(string saveAsPath = null)
        {
            try
            {
                var pathToSave = saveAsPath == null ? inputFilePath : saveAsPath;
                using (FileStream zipToOpen = new FileStream(pathToSave, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        var content = archive.Entries.Where(x => x.Name == "content.xlf").First();

                        content.Delete();

                        Encoding utf8Encoding = new UTF8Encoding(false);
                        XmlWriterSettings settings = new XmlWriterSettings();

                        ZipArchiveEntry newEntry = archive.CreateEntry("content.xlf");

                        using (XmlWriter writer = XmlWriter.Create(newEntry.Open(), settings))
                        {
                            if (contentDocument.DocumentType != null)
                            {
                                contentDocument.DocumentType.InternalSubset = null;
                            }

                            contentDocument.Save(writer);

                        }
                    }
                }
            }
            catch (InvalidDataException)
            {
                contentDocument.Save(saveAsPath);
            }
            catch (Exception ex)
            {
                throw new Exception("Something unexpected happend - Please check your file   - possibility to process file outside of TMS may be required  " + ex.ToString());
            }
        }
    }
    #endregion


    public static class Extension
    {
        #region Extension
        public static IEnumerable<XElement> DescendantsAnyNS<T>(this T source, string localName)
 where T : XContainer
        {
            var result = source.Descendants().Where(e => e.Name.LocalName == localName);

            return result.Count() == 0 ? null : result;
        }

        public static string TransUnitID<T>(this T source)
where T : XElement
        {

            string result = source.AttributeAnyNS("id") == null ? null : source.AttributeAnyNS("id").Value;
            return result;

        }

        public static string GetSource<T>(this T source, bool includeInternal = false)
where T : XElement
        {
            if (source.DescendantsAnyNS("source") != null)
            {
                var sourceElement = source.DescendantsAnyNS("source").First();

                IEnumerable<string> text;
                if (includeInternal)
                    text = sourceElement.DescendantNodes().OfType<XText>().Select(x => x.Value);
                else
                    text = sourceElement.DescendantNodes().OfType<XText>().Where(x => x.Parent == sourceElement).Select(x => x.Value);

                return String.Join("", text);
            }
            return "";
        }

        public static string GetTarget<T>(this T source, bool includeInternal = false)
where T : XElement
        {
            if (source.DescendantsAnyNS("target") != null)
            {
                var sourceElement = source.DescendantsAnyNS("target").First();

                IEnumerable<string> text;
                if (includeInternal)
                    text = sourceElement.DescendantNodes().OfType<XText>().Select(x => x.Value);
                else
                    text = sourceElement.DescendantNodes().OfType<XText>().Where(x => x.Parent == sourceElement).Select(x => x.Value);

                return String.Join("", text);
            }
            return "";
        }

        public static XAttribute AttributeAnyNS<T>(this T source, string localName)
    where T : XElement
        {
            return source.Attributes().SingleOrDefault(e => e.Name.LocalName == localName);
        }

        #endregion
    }

}