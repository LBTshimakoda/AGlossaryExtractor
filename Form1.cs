using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.VisualBasic.FileIO;

namespace AGlossaryExtractor
{
    public partial class Form1 : Form
    {
        public static List<string> langs = new List<string>()
        { "en-gb", "en-us", "ar-xm", "cs-cz", "de-de", "el-gr", "es-es", "et-ee", "fi-fi", "fr-fr", "hu-hu", "it-it",
                "ja-jp", "ko-kr", "lv-lv", "nl-nl", "pl-pl", "pt-br", "pt-pt", "ru-ru", "sv-se", "zh-cn",
                "bg-bg", "da-dk", "ga", "hr-hr", "is-is", "kk-kz", "lt-lt", "mk-mk", "mt-mt", "nb-no", 
                "ro-ro", "sk-sk", "sl-si", "sq-al", "sr-rs", "tr-tr", "uk-ua", "bs-ba"
        };
        public static List<string> slangs = new List<string>()
        { "en-gb", "en-us", "ar-xm", "cs-cz", "de-de", "el-gr", "es-es", "et-ee", "fi-fi", "fr-fr", "hu-hu", "it-it",
                "ja-jp", "ko-kr", "lv-lv", "nl-nl", "pl-pl", "pt-br", "pt-pt", "ru-ru", "sv-se", "zh-cn",
                "bg-bg", "da-dk", "ga", "hr-hr", "is-is", "kk-kz", "lt-lt", "mk-mk", "mt-mt", "nb-no",
                "ro-ro", "sk-sk", "sl-si", "sq-al", "sr-rs", "tr-tr", "uk-ua", "bs-ba"
        };
        private BindingSource bindingSource;
        public Form1()
        {
            InitializeComponent();
            this.textBox1.AllowDrop = true;
            this.textBox1.DragOver += new DragEventHandler(textBox1_DragOver);
            this.textBox1.DragDrop += new DragEventHandler(textBox1_DragDrop);
            this.textBox1.DragEnter += new DragEventHandler(textBox1_DragEnter);
            this.textBox2.AllowDrop = true;
            this.textBox2.DragOver += new DragEventHandler(textBox2_DragOver);
            this.textBox2.DragDrop += new DragEventHandler(textBox2_DragDrop);
            this.textBox2.DragEnter += new DragEventHandler(textBox2_DragEnter);
            this.textBox3.AllowDrop = true;
            this.textBox3.DragOver += new DragEventHandler(textBox3_DragOver);
            this.textBox3.DragDrop += new DragEventHandler(textBox3_DragDrop);
            this.textBox3.DragEnter += new DragEventHandler(textBox3_DragEnter);
            tabPage1.Text = @"TXT";
            tabPage2.Text = @"XLZ";
            this.bindingSource = new BindingSource();
            comboBox2.DataSource = slangs;
            comboBox1.DataSource = langs;
            comboBox2.SelectedIndex = 0;
            comboBox1.SelectedIndex = 2;
            this.Text = "TSV Glossary Extractor (July 10th, 2024)";
        }
        void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                //if (Directory.Exists(files[0]))
                //{
                  //  this.textBox1.Text = files[0];
                //}

                if (files[0].EndsWith(".txt"))
                {
                    this.textBox1.Text = files[0];
                    var fname = (new FileInfo(files[0])).Name;
                    var termsNote = fname.Substring(0, 6);
                    if (Regex.IsMatch(fname, "_"))
                        termsNote = fname.Substring(0, fname.IndexOf("_"));
                    this.textBox4.Text = termsNote;
                }
            }
        }
        void textBox1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        void textBox2_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                //if (Directory.Exists(files[0]))
                //{
                  //  this.textBox2.Text = files[0];
                //}

                if (files[0].EndsWith(".xlz"))
                {
                    this.textBox2.Text = files[0];
                }
            }
        }
        void textBox2_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        void textBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        void textBox3_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                //if (Directory.Exists(files[0]))
                //{
                  //  this.textBox3.Text = files[0];
                //}

                if (files[0].EndsWith(".txt"))
                {
                    this.textBox3.Text = files[0];
                    var fname = (new FileInfo(files[0])).Name;
                    var termsNote = fname.Substring(0, 6);
                    if (Regex.IsMatch(fname, "_"))
                        termsNote = fname.Substring(0, fname.IndexOf("_"));
                    this.textBox5.Text = termsNote;
                }
            }
        }
        void textBox3_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        void textBox3_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        public static List<GlossaryTerm> glossaryTerms = new List<GlossaryTerm>();
        public static List<GlossaryTerm> glossaryTermsMedDRA = new List<GlossaryTerm>();
        public static List<GlossaryTerm> glossaryTermsEDQM = new List<GlossaryTerm>();
        private async void button1_Click(object sender, EventArgs e)
        {
            var inputFile = textBox1.Text;
            var inputFile1 = textBox3.Text;
            if (File.Exists(inputFile))
            {
                var text = File.ReadAllText(inputFile);
                glossaryTermsMedDRA = await getTermsFromTSV(text);
                if (glossaryTermsMedDRA != null)
                {
                    richTextBox1.Text += "MedDRA terms loaded: " + glossaryTermsMedDRA.Count + "\n";
                    var fname = (new FileInfo(inputFile)).Name;
                    var termsNote = fname.Substring(0,6);
                    if (Regex.IsMatch(fname, "_"))
                        termsNote = fname.Substring(0, fname.IndexOf("_"));
                    textBox4.Text = termsNote;
                }
            }
            if (File.Exists(inputFile1))
            {
                var text1 = File.ReadAllText(inputFile1);
                glossaryTermsEDQM = await getTermsFromTSV(text1);
                if (glossaryTermsEDQM != null)
                {
                    richTextBox1.Text += "EDQM terms loaded: " + glossaryTermsEDQM.Count + "\n";
                    var fname1 = (new FileInfo(inputFile1)).Name;
                    var termsNote1 = fname1.Substring(0, 6);
                    if (Regex.IsMatch(fname1, "_"))
                        termsNote1 = fname1.Substring(0, fname1.IndexOf("_"));
                    textBox5.Text = termsNote1;
                }
            }
            glossaryTerms = glossaryTermsMedDRA.Union(glossaryTermsEDQM).ToList();
            if(glossaryTerms==null || glossaryTerms.Count==0)
            {
                richTextBox1.Text += "No terms loaded. Check text files.\n";
            }
            richTextBox1.Text += "Combined terms count: " + glossaryTerms.Count + "\n";
            // Convert the list to a DataTable
            DataTable dataTable = new DataTable();

            // Get the properties of the class using reflection
            var properties = typeof(GlossaryTerm).GetProperties();

            // Add columns to the DataTable based on the class properties
            foreach (var property in properties)
            {
                dataTable.Columns.Add(property.Name);
            }

            // Add rows to the DataTable
            foreach (var term in glossaryTerms)
            {
                var values = properties.Select(p => p.GetValue(term, null)).ToArray();
                dataTable.Rows.Add(values);
            }

            bindingSource.DataSource = dataTable;
            dataGridView1.DataSource = bindingSource;
        }
        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            string filterText = SearchTextBox.Text;
            ApplyFilter(filterText);
        }

        private void ApplyFilter(string filterText)
        {
            if (string.IsNullOrEmpty(filterText))
            {
                bindingSource.RemoveFilter();
            }
            else
            {
                // Escape single quotes in the filter text
                filterText = filterText.Replace("'", "''");

                // Get the properties of the Person class using reflection
                var properties = typeof(GlossaryTerm).GetProperties();

                // Create a filter expression dynamically based on the properties
                var filterExpression = string.Format($"en_gb LIKE '%{filterText}%' OR en_us LIKE '%{filterText}%'");//Join(" OR ", properties.Select(p => $"{p.Name} LIKE '%{filterText}%'"));

                bindingSource.Filter = filterExpression;
            }
        }
        public async Task<List<GlossaryTerm>> getTermsFromTSV(string tsvContent)
        {
            string[] lines = tsvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int lineCount = lines.Length;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = lineCount;
            progressBar1.Value = 0;

            int currentLine = 0;

            Dictionary<int, string> langIndex = new Dictionary<int, string>();
            var terms = new List<GlossaryTerm>();
            int j = 0;
            using (StringReader stringReader = new StringReader(tsvContent))
            {
                using (TextFieldParser parser = new TextFieldParser(stringReader))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters("\t");
                    while (!parser.EndOfData)
                    {
                        //Process row
                        j++;
                        currentLine++;
                        progressBar1.Value = currentLine;
                        label9.Text = $"{(currentLine * 100) / lineCount}%";
                        Application.DoEvents();
                        int i = 1; // column number
                                   //string s2 = "";
                        string[] fields = parser.ReadFields();
                        var term = new GlossaryTerm();

                        if (j == 1) //
                        {
                            foreach (string s in fields)
                            {
                                if (s == "Level") term.Level = s;
                                if (s == "en-gb") term.en_gb = s;
                                if (s == "ar-xm") term.ar_xm = s;
                                if (s == "cs-cz") term.cs_cz = s;
                                if (s == "de-de") term.de_de = s;
                                if (s == "el-gr") term.el_gr = s;
                                if (s == "es-es") term.es_es = s;
                                if (s == "et-ee") term.et_ee = s;
                                if (s == "fi-fi") term.fi_fi = s;
                                if (s == "fr-fr") term.fr_fr = s;
                                if (s == "hu-hu") term.hu_hu = s;
                                if (s == "it-it") term.it_it = s;
                                if (s == "ja-jp") term.ja_jp = s;
                                if (s == "ko-kr") term.ko_kr = s;
                                if (s == "lv-lv") term.lv_lv = s;
                                if (s == "nl-nl") term.nl_nl = s;
                                if (s == "pl-pl") term.pl_pl = s;
                                if (s == "pt-br") term.pt_br = s;
                                if (s == "pt-pt") term.pt_pt = s;
                                if (s == "ru-ru") term.ru_ru = s;
                                if (s == "sv-se") term.sv_se = s;
                                if (s == "zh-cn") term.zh_cn = s;

                                if (s == "en-us") term.en_us = s;
                                if (s == "bg-bg") term.bg_bg = s;
                                if (s == "da-dk") term.da_dk = s;
                                if (s == "ga") term.ga = s;
                                if (s == "hr-hr") term.hr_hr = s;
                                if (s == "is-is") term.is_is = s;
                                if (s == "kk-kz") term.kk_kz = s;
                                if (s == "lt-lt") term.lt_lt = s;
                                if (s == "mk-mk") term.mk_mk = s;
                                if (s == "mt-mt") term.mt_mt = s;
                                if (s == "nb-no") term.nb_no = s;
                                if (s == "ro-ro") term.ro_ro = s;
                                if (s == "sk-sk") term.sk_sk = s;
                                if (s == "sl-si") term.sl_si = s;
                                if (s == "sq-al") term.sq_al = s;
                                if (s == "sr-rs") term.sr_rs = s;
                                if (s == "tr-tr") term.tr_tr = s;
                                if (s == "uk-ua") term.uk_ua = s;
                                if (s == "bs-ba") term.bs_ba = s;

                                langIndex.Add(i, s);
                                i++;
                            }
                        }
                        if (j > 1)
                            foreach (string s in fields)
                            {
                                if (langIndex[i] == "Level") term.Level = s;
                                if (langIndex[i] == "en-gb") term.en_gb = s;
                                if (langIndex[i] == "ar-xm") term.ar_xm = s;
                                if (langIndex[i] == "cs-cz") term.cs_cz = s;
                                if (langIndex[i] == "de-de") term.de_de = s;
                                if (langIndex[i] == "el-gr") term.el_gr = s;
                                if (langIndex[i] == "es-es") term.es_es = s;
                                if (langIndex[i] == "et-ee") term.et_ee = s;
                                if (langIndex[i] == "fi-fi") term.fi_fi = s;
                                if (langIndex[i] == "fr-fr") term.fr_fr = s;
                                if (langIndex[i] == "hu-hu") term.hu_hu = s;
                                if (langIndex[i] == "it-it") term.it_it = s;
                                if (langIndex[i] == "ja-jp") term.ja_jp = s;
                                if (langIndex[i] == "ko-kr") term.ko_kr = s;
                                if (langIndex[i] == "lv-lv") term.lv_lv = s;
                                if (langIndex[i] == "nl-nl") term.nl_nl = s;
                                if (langIndex[i] == "pl-pl") term.pl_pl = s;
                                if (langIndex[i] == "pt-br") term.pt_br = s;
                                if (langIndex[i] == "pt-pt") term.pt_pt = s;
                                if (langIndex[i] == "ru-ru") term.ru_ru = s;
                                if (langIndex[i] == "sv-se") term.sv_se = s;
                                if (langIndex[i] == "zh-cn") term.zh_cn = s;

                                if (langIndex[i] == "en-us") term.en_us = s;
                                if (langIndex[i] == "bg-bg") term.bg_bg = s;
                                if (langIndex[i] == "da-dk") term.da_dk = s;
                                if (langIndex[i] == "ga") term.ga = s;
                                if (langIndex[i] == "hr-hr") term.hr_hr = s;
                                if (langIndex[i] == "is-is") term.is_is = s;
                                if (langIndex[i] == "kk-kz") term.kk_kz = s;
                                if (langIndex[i] == "lt-lt") term.lt_lt = s;
                                if (langIndex[i] == "mk-mk") term.mk_mk = s;
                                if (langIndex[i] == "mt-mt") term.mt_mt = s;
                                if (langIndex[i] == "nb-no") term.nb_no = s;
                                if (langIndex[i] == "ro-ro") term.ro_ro = s;
                                if (langIndex[i] == "sk-sk") term.sk_sk = s;
                                if (langIndex[i] == "sl-si") term.sl_si = s;
                                if (langIndex[i] == "sq-al") term.sq_al = s;
                                if (langIndex[i] == "sr-rs") term.sr_rs = s;
                                if (langIndex[i] == "tr-tr") term.tr_tr = s;
                                if (langIndex[i] == "uk-ua") term.uk_ua = s;
                                if (langIndex[i] == "bs-ba") term.bs_ba = s;

                                i++;
                            }
                        if (term.Level == null || term.Level == "") term.Level = "";
                        //if (term.en_gb == null || term.en_gb == "") term.en_gb = term.en_us;
                        terms.Add(term);
                    }
                }
            }
            //label9.Text = "Glossary loaded";
            return terms;
        }

        public static List<GlossaryTermScript> getTermsFromTSV2Langs(string tsvContent, string sLang, string tLang)
        {
            Dictionary<int, string> langIndex = new Dictionary<int, string>();
            var terms = new List<GlossaryTermScript>();
            int j = 0;
            using (StringReader stringReader = new StringReader(tsvContent))
            {
                using (TextFieldParser parser = new TextFieldParser(stringReader))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters("\t");
                    while (!parser.EndOfData)
                    {
                        //Process row
                        j++;
                        int i = 1; // column number
                                   //string s2 = "";
                        string[] fields = parser.ReadFields();
                        var term = new GlossaryTermScript();
                        if (j == 1)
                        {
                            foreach (string s in fields)
                            {
                                langIndex.Add(i, s);
                                i++;
                            }
                        }
                        if (j > 1)
                            foreach (string s in fields)
                            {
                                if (langIndex[i] == "Level") term.Level = s;
                                if (langIndex[i] == sLang) term.sLang = s;
                                if (sLang == "en-us" && langIndex[i] == "en-gb") term.sLang = s;
                                if (sLang == "en-gb" && langIndex[i] == "en-us") term.sLang = s;
                                if (langIndex[i] == tLang) term.tLang = s;
                                i++;
                            }
                        if (term.Level == null || term.Level == "") term.Level = "";
                        //if (term.en_gb == null || term.en_gb == "") term.en_gb = term.en_us;
                        terms.Add(term);
                    }
                }
            }
            return terms;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //var inputFile = textBox2.Text; 
            //if (inputFile == null || !File.Exists(inputFile)) return;
            //if (glossaryTerms == null || glossaryTerms.Count == 0)
            //{
            //    richTextBox2.Text += "Load glossary TXT first.\n";
            //    return;
            //}
            //int newCounter = 0;
            //var slang = comboBox2.Text;
            //var lang = comboBox1.Text;
            ////richTextBox2.Text += lang + "\n";
            //var terms = extractLangGlossaryTerms(glossaryTermsMedDRA,slang, lang);
            //richTextBox2.Text = ""; //terms.Count + "\n";

            //var xlz = new Xlz(inputFile);
            //var tus = xlz.TranslatableTransUnits;
            //GlossaryExtractor extractor = new GlossaryExtractor(terms);
            //foreach (var tu in tus)
            //{
            //    var source = tu.GetSource(false);
            //    Dictionary<string, List<string>> foundTerms = extractor.ExtractTermsFromText(source);
            //    if (foundTerms.Count > 0)
            //    {
            //        richTextBox2.Text += "\nSource id: " + tu.TransUnitID() + " ";
            //        richTextBox2.Text += "(" + foundTerms.Count + "): ";
            //        foreach (var term in foundTerms)
            //        {
            //            richTextBox2.Text += "| " + term.Key + " |";
            //        }
            //        newCounter += foundTerms.Count;
            //    }
            //}
            //richTextBox2.Text += "\nFound terms in file: " + newCounter + "\n";
            //richTextBox2.Text += "End " + DateTime.Now + "\n";
        }
        public List<GlossaryTermScript> extractLangGlossaryTerms(List<GlossaryTerm> terms, string sLang, string tLang)
        {
            List<GlossaryTermScript> glossaryTerms1 = new List<GlossaryTermScript>();

            var properties = typeof(GlossaryTerm).GetProperties();
            List<string> propertyNames = new List<string>();

            foreach (var property in properties)
            {
                propertyNames.Add(property.Name);
            }
            if (!propertyNames.Contains(sLang.Replace("-", "_")))
            {
                richTextBox2.Text += "No terms available for this source language " + sLang + "\n";
                return null;
            }
            if (!propertyNames.Contains(tLang.Replace("-", "_")))
            {
                richTextBox2.Text += "No terms available for this target language " + tLang + "\n";
                return null;
            }

            // Get the properties of the class using reflection
            foreach (var term in terms)
            {
                var glossaryTerm1 = new GlossaryTermScript();
                Type type = term.GetType();
                foreach (var propertyName in propertyNames)
                {
                    PropertyInfo propertyInfo = type.GetProperty(propertyName);
                    if (propertyInfo != null && propertyName == "Level")
                        glossaryTerm1.Level = term.Level;
                    if (propertyInfo != null && propertyName == sLang.Replace("-", "_"))
                    {
                        var value = propertyInfo.GetValue(term, null);
                        if (value != null)
                            glossaryTerm1.sLang = value.ToString();
                        else
                            glossaryTerm1.sLang = null;
                    }
                    if (propertyInfo != null && propertyName == tLang.Replace("-", "_"))
                    {
                        var value = propertyInfo.GetValue(term, null);
                        if (value != null)
                            glossaryTerm1.tLang = value.ToString();
                        else
                            glossaryTerm1.tLang = null;
                    }
                }
                if(glossaryTerm1.tLang!=null && glossaryTerm1.tLang != "-")
                    glossaryTerms1.Add(glossaryTerm1);
            }
            return glossaryTerms1;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            var inputFile = textBox2.Text;
            if (inputFile == null || !File.Exists(inputFile)) return;
            if (glossaryTerms == null || glossaryTerms.Count == 0)
            {
                richTextBox2.Text += "Load glossary TXT first.\n";
                return;
            }
            var slang = comboBox2.Text;
            var lang = comboBox1.Text;
            var uniqueSortedTerms = getExtractedTermsForXlz(inputFile, slang, lang);
            if (uniqueSortedTerms != null)
            {
                //richTextBox2.Text += "\n" + uniqueSortedTerms.Count + "\n";
                SaveSimpleGlossaryTermsToTsv(uniqueSortedTerms, inputFile + "_" + slang + "_" + lang + ".txt", slang, lang);
                richTextBox2.Text += "\n" + String.Format("Extracted Terms saved to glossary: <file://{0}> ", inputFile + "_" + slang + "_" + lang + ".txt") + "\n";
            }
        }
        public List<GlossaryTermScript> getExtractedTermsForXlz(string inputFile, string slang, string tlang)
        {
            if (glossaryTerms == null || glossaryTerms.Count == 0)
            {
                //richTextBox2.Text += "Load glossary TXT first.\n";
                return null;
            }
            var sLang1 = slang;
            if (slang == "en-us") sLang1 = "en-gb"; // only en-gb in MedDRA for English
            var termsMedDRA = extractLangGlossaryTerms(glossaryTermsMedDRA, sLang1, tlang);
            GlossaryExtractor extractorMedDRA = new GlossaryExtractor(termsMedDRA);
            var sLang2 = slang;
            if (slang == "en-gb") sLang2 = "en-us"; // only en-us in EDQM for English
            var termsEDQM = extractLangGlossaryTerms(glossaryTermsEDQM, sLang2, tlang);
            GlossaryExtractor extractorEDQM = new GlossaryExtractor(termsEDQM);
            if (termsMedDRA == null && termsEDQM == null)
                return null;
            int newCounter = 0;
            var extractedTerms = new List<GlossaryTermScript>();
            var checks = new List<string>();
            var xlz = new Xlz(inputFile);
            var tus = xlz.TranslatableTransUnits;
            foreach (var tu in tus)
            {
                var source = tu.GetSource(false);
                Dictionary<string, List<string>> foundTermsMedDRA = extractorMedDRA.ExtractTermsFromText(source);
                Dictionary<string, List<string>> foundTermsEDQM = extractorEDQM.ExtractTermsFromText(source);
                if (foundTermsMedDRA.Count > 0)
                {
                    var note = "";
                    foreach (var term in foundTermsMedDRA)
                    {
                        note += term.Key + " = " + term.Value[0] + "\n";
                        var extractedTerm = new GlossaryTermScript { Level = term.Value[1], sLang = term.Key, tLang = term.Value[0] };
                        if (!checks.Contains(term.Key))
                        {
                            checks.Add(term.Key);
                            extractedTerms.Add(extractedTerm);
                        }
                    }
                    //tu.Add(new XElement("note", new XAttribute("annotates", "Glossary"), new XAttribute("from", "MedDRA"), note));
                    //newCounter += foundTermsMedDRA.Count;
                }
                if (foundTermsEDQM.Count > 0)
                {
                    var note = "";
                    foreach (var term in foundTermsEDQM)
                    {
                        note += term.Key + " = " + term.Value[0] + "\n";
                        var extractedTerm = new GlossaryTermScript { Level = term.Value[1], sLang = term.Key, tLang = term.Value[0] };
                        if (!checks.Contains(term.Key))
                        {
                            checks.Add(term.Key);
                            extractedTerms.Add(extractedTerm);
                        }
                    }
                    //tu.Add(new XElement("note", new XAttribute("annotates", "Glossary"), new XAttribute("from", "EDQM"), note));
                    //newCounter += foundTermsMedDRA.Count;
                }
            }
            var uniqueSortedTerms = extractedTerms.OrderBy(term => term.sLang).ToList();
            return uniqueSortedTerms;
        }
        private void richTextBox2_OnLinkClicked(object sender, LinkClickedEventArgs e)
        {
            var filePath = new Uri(e.LinkText).AbsolutePath;
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Path.GetFileName(filePath);
            psi.WorkingDirectory = Path.GetDirectoryName(filePath);
            psi.Arguments = "";
            Process.Start("\"" + Path.Combine(psi.WorkingDirectory, psi.FileName).Replace("%20", " ") + "\"");
        }

        static void SaveSimpleGlossaryTermsToTsv(List<GlossaryTermScript> glossaryTerms, string filePath, string sLang, string tLang)
        {
            // Use StreamWriter with UTF-8 encoding and BOM
            using (StreamWriter writer = new StreamWriter(filePath, false, new UTF8Encoding(true)))
            {
                // Write the header
                //writer.WriteLine("Term\tDefinition");
                writer.WriteLine("Comment\tMT_Ready\tMT_DoNotTranslate\tMT_CaseSensitive\t" + sLang + "\t" + tLang + "\tPOS");

                // Write each glossary term
                foreach (var term in glossaryTerms)
                {
                        writer.WriteLine(term.Level + "\tTRUE\tFALSE\tFALSE\t" + term.sLang + "\t" + term.tLang + "\tnoun");
                }
            }
        }

        public bool SaveLangGlossaryTermsToTsv(List<GlossaryTerm> glossaryTerms, string filePath, string sLang, string tLang)
        {
            var properties = typeof(GlossaryTerm).GetProperties();
            List<string> propertyNames = new List<string>();
            // Add columns to the DataTable based on the class properties
            foreach (var property in properties)
            {
                propertyNames.Add(property.Name);
                //richTextBox2.Text += property.Name + "\n";
            }
            if (!propertyNames.Contains(sLang.Replace("-", "_")))
            {
                richTextBox2.Text += "No terms available for this source language " + sLang + "\n";
                return false;
            }
            if (!propertyNames.Contains(tLang.Replace("-", "_")))
            {
                richTextBox2.Text += "No terms available for this target language " + tLang + "\n";
                return false;
            }
            // Use StreamWriter with UTF-8 encoding and BOM
            using (StreamWriter writer = new StreamWriter(filePath, false, new UTF8Encoding(true)))
            {
                // Write the header
                //writer.WriteLine("Term\tDefinition");

                foreach (var term in glossaryTerms)
                {
                    var glossaryTerm1 = new GlossaryTermScript();
                    Type type = term.GetType();
                    var sLanguage = "";
                    var tLanguage = "";
                    foreach (var propertyName in propertyNames)
                    {
                        PropertyInfo propertyInfo = type.GetProperty(propertyName);
                        if (propertyInfo != null && propertyName == sLang.Replace("-", "_"))
                        {
                            var value = propertyInfo.GetValue(term, null);
                            if(value!=null) sLanguage = value.ToString();
                        }
                        if (propertyInfo != null && propertyName == tLang.Replace("-", "_"))
                        {
                            var value = propertyInfo.GetValue(term, null);
                            if (value != null) tLanguage = value.ToString();
                        }
                    }
                    writer.WriteLine(term.Level + "\t" + sLanguage + "\t" + tLanguage);
                }
                // Write each glossary term
                //foreach (var term in glossaryTerms)
                //{
                //    if (tLang == "ar-xm") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.ar_xm}");
                //    if (tLang == "cs-cz") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.cs_cz}");
                //    if (tLang == "de-de") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.de_de}");
                //    if (tLang == "el-gr") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.el_gr}");
                //    if (tLang == "es-es") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.es_es}");
                //    if (tLang == "et-ee") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.et_ee}");
                //    if (tLang == "fi-fi") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.fi_fi}");
                //    if (tLang == "fr-fr") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.fr_fr}");
                //    if (tLang == "hu-hu") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.hu_hu}");
                //    if (tLang == "it-it") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.it_it}");
                //    if (tLang == "ja-jp") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.ja_jp}");
                //    if (tLang == "ko-kr") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.ko_kr}");
                //    if (tLang == "lv-lv") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.lv_lv}");
                //    if (tLang == "nl-nl") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.nl_nl}");
                //    if (tLang == "pl-pl") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.pl_pl}");
                //    if (tLang == "pt-br") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.pt_br}");
                //    if (tLang == "pt-pt") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.pt_pt}");
                //    if (tLang == "ru-ru") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.ru_ru}");
                //    if (tLang == "sv-se") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.sv_se}");
                //    if (tLang == "zh-cn") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.zh_cn}");

                //    if (tLang == "en-us") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.en_us}");
                //    if (tLang == "bg-bg") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.bg_bg}");
                //    if (tLang == "da-dk") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.da_dk}");
                //    if (tLang == "qa") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.qa}");
                //    if (tLang == "hr-hr") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.hr_hr}");
                //    if (tLang == "is-is") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.is_is}");
                //    if (tLang == "kk-kz") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.kk_kz}");
                //    if (tLang == "lt-lt") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.lt_lt}");
                //    if (tLang == "mk-mk") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.mk_mk}");
                //    if (tLang == "mt-mt") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.mt_mt}");
                //    if (tLang == "nb-no") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.nb_no}");
                //    if (tLang == "ro-ro") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.ro_ro}");
                //    if (tLang == "sk-sk") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.sk_sk}");
                //    if (tLang == "sl-si") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.sl_si}");
                //    if (tLang == "sq-al") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.sq_al}");
                //    if (tLang == "sr-rs") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.sr_rs}");
                //    if (tLang == "tr-tr") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.tr_tr}");
                //    if (tLang == "uk-ua") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.uk_ua}");
                //    if (tLang == "bs-ba") writer.WriteLine($"{term.Level}\t{term.en_gb}\t{term.bs_ba}");
                //}
            }
            return true;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            var inputFile = textBox2.Text;
            if (inputFile == null || !File.Exists(inputFile)) return;
            if (glossaryTerms == null || glossaryTerms.Count == 0)
            {
                richTextBox2.Text += "Load glossary TXT first.\n";
                return;
            }
            var slang = comboBox2.Text;
            var lang = comboBox1.Text;
            var MedDRAnote = "MedDRA";
            var EDQMnote = "EDQM";
            if (textBox4.Text != "") MedDRAnote = textBox4.Text;
            if (textBox5.Text != "") EDQMnote = textBox5.Text;
            //richTextBox2.Text = lang;
            var sLang1 = slang;
            if (slang == "en-us") sLang1 = "en-gb"; // only en-gb in MedDRA for English
            var termsMedDRA = extractLangGlossaryTerms(glossaryTermsMedDRA, sLang1, lang);
            GlossaryExtractor extractorMedDRA = new GlossaryExtractor(termsMedDRA);
            var sLang2 = slang;
            if (slang == "en-gb") sLang2 = "en-us"; // only en-us in EDQM for English
            var termsEDQM = extractLangGlossaryTerms(glossaryTermsEDQM, sLang2, lang);
            GlossaryExtractor extractorEDQM = new GlossaryExtractor(termsEDQM);
            var fio = new FileInfo(inputFile);
            var extension = fio.Extension;
            var fname = fio.Name.Substring(0, fio.Name.LastIndexOf(".")); 
            var dir = fio.DirectoryName;
            var outputFile = Path.Combine(dir, fname + "_withNotes" + extension);
            if(File.Exists(outputFile)) File.Delete(outputFile);
            File.Copy(inputFile, outputFile);
            int newCounter = 0;
            var extractedTerms = new List<GlossaryTermScript>();
            var checks = new List<string>();
            var xlz = new Xlz(outputFile);
            var tus = xlz.TranslatableTransUnits;
            foreach (var tu in tus)
            {
                var source = tu.GetSource(false);
                Dictionary<string, List<string>> foundTermsMedDRA = extractorMedDRA.ExtractTermsFromText(source);
                Dictionary<string, List<string>> foundTermsEDQM = extractorEDQM.ExtractTermsFromText(source);
                if (foundTermsMedDRA.Count > 0)
                {
                    var note = "";
                    foreach (var term in foundTermsMedDRA)
                    {
                        note += term.Key + " = " + term.Value[0] + "\n";
                        var extractedTerm = new GlossaryTermScript { Level = term.Value[1], sLang = term.Key, tLang = term.Value[0] };
                        if (!checks.Contains(term.Key))
                        {
                            checks.Add(term.Key);
                            extractedTerms.Add(extractedTerm);
                        }
                    }
                    tu.Add(new XElement("note", new XAttribute("annotates", "Glossary"), new XAttribute("from", MedDRAnote), note));
                    newCounter += foundTermsMedDRA.Count;
                }
                if (foundTermsEDQM.Count > 0)
                {
                    var note = "";
                    foreach (var term in foundTermsEDQM)
                    {
                        note += term.Key + " = " + term.Value[0] + "\n";
                        var extractedTerm = new GlossaryTermScript { Level = term.Value[1], sLang = term.Key, tLang = term.Value[0] };
                        if (!checks.Contains(term.Key))
                        {
                            checks.Add(term.Key);
                            extractedTerms.Add(extractedTerm);
                        }
                    }
                    tu.Add(new XElement("note", new XAttribute("annotates", "Glossary"), new XAttribute("from", EDQMnote), note));
                    newCounter += foundTermsEDQM.Count;
                }
            }
            xlz.Save2();
            richTextBox2.Text += String.Format("Extracted Terms added to xlz file: <file://{0}> ", outputFile) + "\n";
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var scriptBaseFile = Path.Combine(path, @"script_base.xml");
            if(!File.Exists(scriptBaseFile))
            {
                richTextBox2.Text += scriptBaseFile + "\n";
                richTextBox2.Text += @"Script base file (script_base.xml) should be available in the root of the tool." + "\n";
                return;
            }
            path += @"\scripts";
            if(!Directory.Exists(path)) Directory.CreateDirectory(path); 
            //var lang = comboBox1.Text;
            var inputFile = textBox1.Text;
            var inputFile1 = textBox3.Text;

            if (!File.Exists(inputFile) || !File.Exists(inputFile1))
            {
                richTextBox2.Text += "Load the latest MedDRA and EDQM glossaries on TXT tab of the tool first.\n";
                return; 
            }
            var slang = comboBox2.Text;
            //var langs1 = new List<string>() { "de-de" };
            progressBar2.Minimum = 0;
            progressBar2.Maximum = langs.Count;
            progressBar2.Value = 0;
            int currentLang = 0;
            foreach (var lang in langs)
            {

                var sourceFilePath = inputFile + "_" + slang + "_" + lang + ".txt";
                if(!SaveLangGlossaryTermsToTsv(glossaryTermsMedDRA, sourceFilePath, slang, lang))
                    return;
                var fio = new FileInfo(sourceFilePath);
                var fname = fio.Name;
                var zipFilePath = sourceFilePath + ".zip";
                var zipEntryName = fname;
                CreateZipFromTextFile(sourceFilePath, zipFilePath, zipEntryName);
                var bytes = File.ReadAllBytes(zipFilePath);
                string base64StringNewMedDRA = Convert.ToBase64String(bytes, 0, bytes.Length);

                var sourceFilePath1 = inputFile1 + "_copy.txt";
                File.Copy(inputFile1, sourceFilePath1);
                var fio1 = new FileInfo(sourceFilePath1);
                var fname1 = fio1.Name;
                var zipFilePath1 = sourceFilePath + ".zip";
                var zipEntryName1 = fname;
                CreateZipFromTextFile(sourceFilePath1, zipFilePath1, zipEntryName1);
                var bytes1 = File.ReadAllBytes(zipFilePath1);
                string base64StringNewEDQM = Convert.ToBase64String(bytes1, 0, bytes1.Length);

                File.Delete(sourceFilePath);
                File.Delete(zipFilePath);
                File.Delete(sourceFilePath1);
                File.Delete(zipFilePath1);

                var sContent = File.ReadAllText(scriptBaseFile);

                var pattern = new Regex(@"(if\(lang \!= "").*?(""\))");
                sContent = pattern.Replace(sContent, "$1" + lang + "$2");
                pattern = new Regex(@"(base64StringEncodedMedDRA = @"").*?("";)");
                sContent = pattern.Replace(sContent, "$1" + base64StringNewMedDRA + "$2");
                pattern = new Regex(@"(base64StringEncodedEDQM = @"").*?("";)");
                sContent = pattern.Replace(sContent, "$1" + base64StringNewEDQM + "$2");

                DateTime currentDate = DateTime.Now;
                File.WriteAllText(path + @"\xlz_exports_" + slang + "_" + lang + "_" + currentDate.ToString("yyyy-MM-dd") + ".xml", sContent);
                //richTextBox2.Text += String.Format("Script file created: <file://{0}> ", path + @"\xlz_reports_" + slang + "_" + lang + "_" + currentDate.ToString("yyyy-MM-dd") + ".xml") + "\n";
                richTextBox2.Text += ".";

                pattern = new Regex(@"//xlz\.Save2\(\);");
                sContent = pattern.Replace(sContent, "xlz.Save2();");
                pattern = new Regex(@"(\n\s*?)(SaveSimpleGlossaryTermsToTsv)");
                sContent = pattern.Replace(sContent, "$1" + @"//" + "$2");
                File.WriteAllText(path + @"\xlz_notes_" + slang + "_" + lang + "_" + currentDate.ToString("yyyy-MM-dd") + ".xml", sContent);
                //File.WriteAllText(zipFilePath + "_base64.txt", base64StringNew);
                //richTextBox2.Text += String.Format("Script file created: <file://{0}> ", path + @"\xlz_notes_" + slang + "_" + lang + "_" + currentDate.ToString("yyyy-MM-dd") + ".xml") + "\n";
                richTextBox2.Text += ".";
                currentLang++;
                progressBar2.Value = currentLang;
                label10.Text = $"{(currentLang * 100) / langs.Count}%";
                Application.DoEvents();
            }
            richTextBox2.Text += "\nAll generated scripts saved in" + String.Format(": <file://{0}> ", path) + "\n";
        }
        static void CreateZipFromTextFile(string sourceFilePath, string zipFilePath, string zipEntryName)
        {
            // Ensure the source file exists
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException("The source file was not found.", sourceFilePath);
            }

            // Delete the zip file if it already exists
            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            // Create the zip file and add the text file to it
            using (FileStream zipToOpen = new FileStream(zipFilePath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    ZipArchiveEntry readmeEntry = archive.CreateEntry(zipEntryName);

                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        // Read the text file and write its content to the zip entry
                        string content = File.ReadAllText(sourceFilePath);
                        writer.Write(content);
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var source = textBox2.Text;
            if (source == "")
                return;
            if (glossaryTerms == null || glossaryTerms.Count == 0)
            {
                richTextBox2.Text += "Load glossary TXT first.\n";
                return;
            }
            var slang = comboBox2.Text;
            var lang = comboBox1.Text;
            var sLang1 = slang;
            if (slang == "en-us") sLang1 = "en-gb"; // only en-gb in MedDRA for English
            var termsMedDRA = extractLangGlossaryTerms(glossaryTermsMedDRA, sLang1, lang);
            GlossaryExtractor extractorMedDRA = new GlossaryExtractor(termsMedDRA);
            var sLang2 = slang;
            if (slang == "en-gb") sLang2 = "en-us"; // only en-us in EDQM for English
            var termsEDQM = extractLangGlossaryTerms(glossaryTermsEDQM, sLang2, lang);
            GlossaryExtractor extractorEDQM = new GlossaryExtractor(termsEDQM);
            Dictionary<string, List<string>> foundTermsMedDRA = extractorMedDRA.ExtractTermsFromText(source);
            Dictionary<string, List<string>> foundTermsEDQM = extractorEDQM.ExtractTermsFromText(source);
            if (foundTermsMedDRA.Count > 0)
            {
                foreach (var term in foundTermsMedDRA)
                {
                    richTextBox2.Text += "MedDRA " + term.Key + " = " + term.Value[0] + "\n";
                }
            }
            if (foundTermsEDQM.Count > 0)
            {
                foreach (var term in foundTermsEDQM)
                {
                    richTextBox2.Text += "EDQM " + term.Key + " = " + term.Value[0] + "\n";
                }
            }
        }
    }
    public class GlossaryTerm
    {
        public string Level { get; set; }
        public string en_gb { get; set; }
        public string ar_xm { get; set; }
        public string cs_cz { get; set; }
        public string de_de { get; set; }
        public string el_gr { get; set; }
        public string es_es { get; set; }
        public string et_ee { get; set; }
        public string fi_fi { get; set; }
        public string fr_fr { get; set; }
        public string hu_hu { get; set; }
        public string it_it { get; set; }
        public string ja_jp { get; set; }
        public string ko_kr { get; set; }
        public string lv_lv { get; set; }
        public string nl_nl { get; set; }
        public string pl_pl { get; set; }
        public string pt_br { get; set; }
        public string pt_pt { get; set; }
        public string ru_ru { get; set; }
        public string sv_se { get; set; }
        public string zh_cn { get; set; }

        public string en_us { get; set; }
        public string bg_bg { get; set; }
        public string da_dk { get; set; }
        public string ga { get; set; }
        public string hr_hr { get; set; }
        public string is_is { get; set; }
        public string kk_kz { get; set; }
        public string lt_lt { get; set; }
        public string mk_mk { get; set; }
        public string mt_mt { get; set; }
        public string nb_no { get; set; }
        public string ro_ro { get; set; }
        public string sk_sk { get; set; }
        public string sl_si { get; set; }
        public string sq_al { get; set; }
        public string sr_rs { get; set; }
        public string tr_tr { get; set; }
        public string uk_ua { get; set; }
        public string bs_ba { get; set; }
    }
    public class GlossaryTermScript
    {
        public string Level { get; set; }
        public string sLang { get; set; }
        public string tLang { get; set; }
    }
    public class TrieNode
    {
        public Dictionary<string, TrieNode> Children { get; set; }
        public bool IsEndOfTerm { get; set; }
        public string Term { get; set; }
        public string OrigTerm { get; set; }
        public string Translation { get; set; }
        public string Level { get; set; }

        public TrieNode()
        {
            Children = new Dictionary<string, TrieNode>();
            IsEndOfTerm = false;
            Term = null;
            OrigTerm = null;
            Translation = null;
            Level = null;
        }
    }
    public class Trie
    {
        private TrieNode root;

        public Trie()
        {
            root = new TrieNode();
        }

        public void Insert(string term, string origterm, string translation, string level)
        {
            var node = root;
            var words = term.Split(' ');

            foreach (string word in words)
            {
                if (!node.Children.ContainsKey(word))
                {
                    node.Children[word] = new TrieNode();
                }
                node = node.Children[word];
            }
            node.IsEndOfTerm = true;
            node.Term = term;
            node.OrigTerm = origterm;
            node.Translation = translation;
            node.Level = level;
        }

        // Search for a term in the Trie
        public bool Search(List<string> words)
        {
            TrieNode node = root;
            foreach (var word in words)
            {
                if (!node.Children.ContainsKey(word))
                {
                    return false;
                }
                node = node.Children[word];
            }
            return node.IsEndOfTerm;
        }
    
        public TrieNode GetRoot()
        {
            return root;
        }
    }
    public class GlossaryExtractor
    {
        private Trie trie;
        private List<GlossaryTermScript> glossaryTerms;

        public GlossaryExtractor(List<GlossaryTermScript> terms)
        {
            glossaryTerms = terms;
            trie = new Trie();
            foreach (var term in terms)
            {
                if(term.sLang!=null)
                    trie.Insert(term.sLang.ToLower(), term.sLang, term.tLang, term.Level);
            }
        }
        public Dictionary<string, List<string>> ExtractTermsFromText(string text)
        {
            Dictionary<string, List<string>> foundTerms = new Dictionary<string, List<string>>();
            string[] paragraphs = text.Split(new[] { "\r\n\r\n", "\n\n", ".", ",", "?", "!" }, StringSplitOptions.None);

            foreach (string paragraph in paragraphs)
            {
                var paragraph1 = AddPluralsAndEdForms(paragraph);
                SearchTermsInParagraph(paragraph1, foundTerms);
            }
            return RemoveSingleTermsIfLongerVersionsExist(foundTerms, text);
        }
        private static bool IsTermSeparatelyInText(string term, string text)
        {
            // Use regex to match whole words to avoid partial matches.
            // This regex pattern ensures that the term is matched as a whole word.
            //string pattern = $@"\b{Regex.Escape(term)}\b";
            //var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return text.ToLower().Contains(term.ToLower()); //regex.IsMatch(text)
        }
        public static Dictionary<string, List<string>> RemoveSingleTermsIfLongerVersionsExist(Dictionary<string, List<string>> terms, string text)
        {
            // Sort terms by length descending so we handle longer terms first
            var sortedTerms = terms.Keys.OrderByDescending(k => k.Length).ToList();

            var filteredTerms = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var term in sortedTerms)
            {
                // Check if term should be added (i.e., it's not contained within an already added longer term)
                bool isSubTerm = false;
                foreach (var filteredTerm in filteredTerms.Keys)
                {
                    if (filteredTerm.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0 && !IsTermSeparatelyInText(term, text))
                    {
                            isSubTerm = true;
                            break;
                    }
                }
                text = Regex.Replace(text, term, " ", RegexOptions.IgnoreCase);

                if (!isSubTerm)
                {
                    filteredTerms[term] = terms[term];
                }
            }

            return filteredTerms;
        }
        public static Dictionary<string, List<string>> RemoveTermsContainingOtherTerms(Dictionary<string, List<string>> terms)
        {
            // Make a copy of the dictionary to avoid modifying the dictionary while iterating
            var filteredTerms = new Dictionary<string, List<string>>(terms);

            foreach (var term in terms.Keys)
            {
                foreach (var otherTerm in terms.Keys)
                {
                    // If the term contains another term and they are not the same term
                    if (!term.Equals(otherTerm, StringComparison.OrdinalIgnoreCase) && term.IndexOf(otherTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                    //if (term != otherTerm && term.Contains(otherTerm))
                    {
                        filteredTerms.Remove(term);
                        break; // No need to check further once the term is removed
                    }
                }
            }

            return filteredTerms;
        }
        public static string AddPluralsAndEdForms(string text)
        {
            var words = text.Split(new[] { ' ', '\t', '\n', '\r', '/' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> newwords = new List<string>();
            foreach (var word in words)
            {
                newwords.Add(word);
            }
            newwords.Add(".");
            foreach (var word in words)
            {
                if (word.EndsWith("s")) newwords.Add(word.Substring(0, word.Length - 1));
                else newwords.Add(word);
            }
            newwords.Add(".");
            //foreach (var word in words)
            //{
            //    if (word.EndsWith("ed")) newwords.Add(word.Substring(0, word.Length - 1));
            //    else newwords.Add(word);
            //}
            //newwords.Add(".");
            //foreach (var word in words)
            //{
            //    if (word.EndsWith("ed")) newwords.Add(word.Substring(0, word.Length - 2));
            //    else newwords.Add(word);
            //}
            return string.Join(" ", newwords.ToList());
        }
        private void SearchTermsInParagraph(string paragraph, Dictionary<string, List<string>> foundTerms)
        {
            // Split the paragraph into words
            var words = paragraph.Split(new[] { ' ', '\t', '\n', '\r', '/' }, StringSplitOptions.RemoveEmptyEntries);
            //words = AddPluralsAndEdForms1(words);
            TrieNode root = trie.GetRoot();
            for (int i = 0; i < words.Length; i++)
            {
                var potentialTerm = new List<string>();
                TrieNode node = root;
                int j = i;
                while (j < words.Length)
                {
                    string cleanedWord = words[j].ToLower();
                    cleanedWord = cleanedWord.TrimEnd('.', ',', ';', ':', '!', '?', '(', ')', '[', ']', '{', '}', '\"', '\'', '/', '\\', '<', '>', '”');
                    cleanedWord = cleanedWord.TrimStart('.', ',', ';', ':', '!', '?', '(', ')', '[', ']', '{', '}', '\"', '\'', '/', '\\', '<', '>', '“');
                    if (node.Children.ContainsKey(cleanedWord))
                    {
                        node = node.Children[cleanedWord];
                        potentialTerm.Add(cleanedWord);
                        if (node.IsEndOfTerm)
                        {
                            if (trie.Search(potentialTerm))
                            {
                                if (!foundTerms.ContainsKey(node.OrigTerm))
                                {
                                    foundTerms[node.OrigTerm] = new List<string>();
                                }
                                foundTerms[node.OrigTerm].Add(node.Translation);
                                foundTerms[node.OrigTerm].Add(node.Level);
                            }
                        }
                    }
                    else 
                    {
                        break;
                    }
                    j++;
                }
            }
        }
    }
}
