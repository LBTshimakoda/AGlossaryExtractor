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
using System.Runtime.CompilerServices;
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
                "ro-ro", "sk-sk", "sl-si", "sq-al", "sr-rs", "tr-tr", "uk-ua", "bs-ba",
                "ar-eg", "es-mx", "fa-ir", "fr-ca", "he-il", "hi-in", "id-id", "ms-my", "srl-rs", "th-th", "zh-tw"
        };
        public static List<string> slangs = new List<string>()
        { "en-gb", "en-us", "ar-xm", "cs-cz", "de-de", "el-gr", "es-es", "et-ee", "fi-fi", "fr-fr", "hu-hu", "it-it",
                "ja-jp", "ko-kr", "lv-lv", "nl-nl", "pl-pl", "pt-br", "pt-pt", "ru-ru", "sv-se", "zh-cn",
                "bg-bg", "da-dk", "ga", "hr-hr", "is-is", "kk-kz", "lt-lt", "mk-mk", "mt-mt", "nb-no",
                "ro-ro", "sk-sk", "sl-si", "sq-al", "sr-rs", "tr-tr", "uk-ua", "bs-ba",
                "ar-eg", "es-mx", "fa-ir", "fr-ca", "he-il", "hi-in", "id-id", "ms-my", "srl-rs", "th-th", "zh-tw"
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
            this.Text = "TSV Glossary Extractor (August 26th, 2024)";
            loadGlossaries();
        }
        public async void loadGlossaries()
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var files = Directory.GetFiles(path, "*.txt");
            foreach (var file in files)
            {
                if (Regex.IsMatch(file, @"\\MedDRA[^\\]*?\.txt"))
                {
                    var text = File.ReadAllText(file);
                    glossaryTermsMedDRA = await getTermsFromTSV(text);
                    if (glossaryTermsMedDRA != null)
                    {
                        richTextBox1.Text += "MedDRA terms loaded: " + glossaryTermsMedDRA.Count + "\n";
                        var fname = (new FileInfo(file)).Name;
                        var termsNote = fname.Substring(0, 6);
                        if (Regex.IsMatch(fname, "_"))
                            termsNote = fname.Substring(0, fname.IndexOf("_"));
                        textBox1.Text = file;
                        textBox4.Text = termsNote;
                    }
                }
                if(Regex.IsMatch(file, @"\\EDQM[^\\]*?\.txt"))
                {
                    var text1 = File.ReadAllText(file);
                    glossaryTermsEDQM = await getTermsFromTSV(text1);
                    if (glossaryTermsEDQM != null)
                    {
                        richTextBox1.Text += "EDQM terms loaded: " + glossaryTermsEDQM.Count + "\n";
                        var fname1 = (new FileInfo(file)).Name;
                        var termsNote1 = fname1.Substring(0, 6);
                        if (Regex.IsMatch(fname1, "_"))
                            termsNote1 = fname1.Substring(0, fname1.IndexOf("_"));
                        textBox3.Text = file;
                        textBox5.Text = termsNote1;
                    }

                }
            }
            glossaryTerms = glossaryTermsMedDRA.Union(glossaryTermsEDQM).ToList();
            if (glossaryTerms == null || glossaryTerms.Count == 0)
            {
                richTextBox1.Text += "No terms loaded. Check text files.\n";
                return;
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
                return;
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
                //var filterExpression = string.Format($"ru_ru LIKE '%{filterText}%'");//Join(" OR ", properties.Select(p => $"{p.Name} LIKE '%{filterText}%'"));

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
                                if (s == "ar-eg") term.ar_eg = s;
                                if (s == "es-mx") term.es_mx = s;
                                if (s == "fa-ir") term.fa_ir = s;
                                if (s == "fr-ca") term.fr_ca = s;
                                if (s == "he-il") term.he_il = s;
                                if (s == "hi-in") term.hi_in = s;
                                if (s == "id-id") term.id_id = s;
                                if (s == "ms-my") term.ms_my = s;
                                if (s == "srl-rs") term.srl_rs = s;
                                if (s == "th-th") term.th_th = s;
                                if (s == "zh-tw") term.zh_tw = s;

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
                                if (langIndex[i] == "ar-eg") term.ar_eg = s;
                                if (langIndex[i] == "es-mx") term.es_mx = s;
                                if (langIndex[i] == "fa-ir") term.fa_ir = s;
                                if (langIndex[i] == "fr-ca") term.fr_ca = s;
                                if (langIndex[i] == "he-il") term.he_il = s;
                                if (langIndex[i] == "hi-in") term.hi_in = s;
                                if (langIndex[i] == "id-id") term.id_id = s;
                                if (langIndex[i] == "ms-my") term.ms_my = s;
                                if (langIndex[i] == "srl-rs") term.srl_rs = s;
                                if (langIndex[i] == "th-th") term.th_th = s;
                                if (langIndex[i] == "zh-tw") term.zh_tw = s;

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
            SOURCE_LANG_CODE = sLang;
            TARGET_LANG_CODE = tLang;
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
            SOURCE_LANG_CODE = sLang;
            TARGET_LANG_CODE = tLang;
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
                        if (value != null && value.ToString().Trim() != "")
                            glossaryTerm1.sLang = value.ToString();
                        else
                            glossaryTerm1.sLang = null;
                    }
                    if (propertyInfo != null && propertyName == tLang.Replace("-", "_"))
                    {
                        var value = propertyInfo.GetValue(term, null);
                        if (value != null && value.ToString().Trim() != "")
                            glossaryTerm1.tLang = value.ToString();
                        else
                            glossaryTerm1.tLang = null;
                    }
                }
                if(glossaryTerm1.sLang!=null && glossaryTerm1.tLang!=null && glossaryTerm1.tLang != "-" && glossaryTerm1.tLang.Trim() != "")
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
            SOURCE_LANG_CODE = slang;
            TARGET_LANG_CODE = lang;
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
            SOURCE_LANG_CODE = slang;
            TARGET_LANG_CODE = tlang;
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
            }
            return true;
        }
        public static string SOURCE_LANG_CODE = "en-gb";
        public static string TARGET_LANG_CODE = "de-de";
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
            SOURCE_LANG_CODE = slang;
            TARGET_LANG_CODE = lang;
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

        public string ar_eg { get; set; }
        public string es_mx { get; set; }
        public string fa_ir { get; set; }
        public string fr_ca { get; set; }
        public string he_il { get; set; }
        public string hi_in { get; set; }
        public string id_id { get; set; }
        public string ms_my { get; set; }
        public string srl_rs { get; set; }
        public string th_th { get; set; }
        public string zh_tw { get; set; }
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
        public bool Search(List<string> words, out TrieNode endNode)
        {
            TrieNode node = root;
            foreach (var word in words)
            {
                if (!node.Children.ContainsKey(word))
                {
                    endNode = null;
                    return false;
                }
                node = node.Children[word];
            }
            endNode = node;
            return node.IsEndOfTerm;
        }

        public bool Search(List<string> words, out TrieNode endNode, int ignoreLastNLetters = 0)
        {
            TrieNode node = root;

            foreach (var word in words)
            {
                bool found = false;

                // Check for an exact match first
                if (node.Children.ContainsKey(word))
                {
                    node = node.Children[word];
                    found = true;
                }
                else if (ignoreLastNLetters > 0 && (Form1.SOURCE_LANG_CODE != "en-gb" && Form1.SOURCE_LANG_CODE != "en-us"))
                {
                    // Check for a match with the last N letters removed
                    for (int n = 1; n <= ignoreLastNLetters && word.Length > n + 5; n++)
                    {
                        var truncatedWord = word.Substring(0, word.Length - n);
                        foreach (var child in node.Children)
                        {
                            if (child.Key.StartsWith(truncatedWord) && Math.Abs(child.Key.Length - truncatedWord.Length) < 2)
                            {
                                node = child.Value;
                                //potentialTerm.Add(child.Key);
                                found = true;
                                break;
                            }
                        }
                    }
                }

                if (!found)
                {
                    endNode = null;
                    return false;
                }
            }

            endNode = node;
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
                if (term.sLang != null)
                {
                    trie.Insert(term.sLang.ToLower(), term.sLang, term.tLang, term.Level);
                }
            }
        }
        static List<string> GenerateTransformedTerms(string term)
        {
            var words = term.Split(' ');
            var results = new List<string>();
            GenerateCombinations(words, 0, new List<string>(), results);
            return results;
        }
        static void GenerateCombinations(string[] words, int index, List<string> current, List<string> results)
        {
            if (index == words.Length)
            {
                results.Add(string.Join(" ", current));
                return;
            }

            // Original word
            current.Add(words[index]);
            GenerateCombinations(words, index + 1, current, results);
            current.RemoveAt(current.Count - 1);

            // Transformed words
            var transformedWords = TransformWord(words[index]);
            foreach (var transformedWord in transformedWords)
            {
                current.Add(transformedWord);
                GenerateCombinations(words, index + 1, current, results);
                current.RemoveAt(current.Count - 1);
            }
        }
        static List<string> TransformWord(string word)
        {
            // Example transformation function: returns the original word and its reversed version
            var transformedWords = new List<string>();
            transformedWords.Add(word);
            switch (Form1.SOURCE_LANG_CODE)
                {
                case "en-gb":
                case "en-us":
                    transformedWords.Add(Pluralize_en(word));
                    break;
                case "da-dk":
                    transformedWords.Add(Pluralize_da(word));
                    break;
                case "ru-ru":
                    var ru_sigular_terms = GetPossibleForms_ru(word);
                    transformedWords.AddRange(ru_sigular_terms);
                    var plural_ru_term = Pluralize_ru(word);
                    transformedWords.Add(plural_ru_term);
                    var ru_plural_terms = GetPossiblePluralForms_ru(plural_ru_term);
                    transformedWords.AddRange(ru_plural_terms);
                    break;
            }
            return transformedWords;
        }
        static List<string> GetPossiblePluralForms_ru(string noun)
        {
            var forms = new List<string> { noun };

            // Irregular plural nouns dictionary
            var irregularNouns = new Dictionary<string, List<string>>
        {
            { "дети", new List<string> { "детей", "детям", "детей", "детьми", "детях" } },
            { "люди", new List<string> { "людей", "людям", "людей", "людьми", "людях" } }
        };

            // Check for irregular plural nouns
            if (irregularNouns.ContainsKey(noun))
            {
                forms.AddRange(irregularNouns[noun]);
                return forms;
            }

            // Nouns ending in "ы" or "и"
            if (noun.EndsWith("ы") || noun.EndsWith("и"))
            {
                forms.Add(noun.Substring(0, noun.Length - 1) + "ов"); // Genitive
                forms.Add(noun.Substring(0, noun.Length - 1) + "ам"); // Dative
                forms.Add(noun.Substring(0, noun.Length - 1) + "ы"); // Accusative
                forms.Add(noun.Substring(0, noun.Length - 1) + "ами"); // Instrumental
                forms.Add(noun.Substring(0, noun.Length - 1) + "ах"); // Prepositional
            }
            // Nouns ending in "а"
            else if (noun.EndsWith("а"))
            {
                forms.Add(noun.Substring(0, noun.Length - 1) + ""); // Genitive
                forms.Add(noun.Substring(0, noun.Length - 1) + "ам"); // Dative
                forms.Add(noun.Substring(0, noun.Length - 1) + ""); // Accusative
                forms.Add(noun.Substring(0, noun.Length - 1) + "ами"); // Instrumental
                forms.Add(noun.Substring(0, noun.Length - 1) + "ах"); // Prepositional
            }
            // Nouns ending in "я"
            else if (noun.EndsWith("я"))
            {
                forms.Add(noun.Substring(0, noun.Length - 1) + "й"); // Genitive
                forms.Add(noun.Substring(0, noun.Length - 1) + "ям"); // Dative
                forms.Add(noun.Substring(0, noun.Length - 1) + "й"); // Accusative
                forms.Add(noun.Substring(0, noun.Length - 1) + "ями"); // Instrumental
                forms.Add(noun.Substring(0, noun.Length - 1) + "ях"); // Prepositional
            }
            // Nouns ending in "о"
            else if (noun.EndsWith("о"))
            {
                forms.Add(noun.Substring(0, noun.Length - 1) + ""); // Genitive
                forms.Add(noun.Substring(0, noun.Length - 1) + "ам"); // Dative
                forms.Add(noun.Substring(0, noun.Length - 1) + ""); // Accusative
                forms.Add(noun.Substring(0, noun.Length - 1) + "ами"); // Instrumental
                forms.Add(noun.Substring(0, noun.Length - 1) + "ах"); // Prepositional
            }
            // Nouns ending in "е"
            else if (noun.EndsWith("е"))
            {
                forms.Add(noun.Substring(0, noun.Length - 1) + "й"); // Genitive
                forms.Add(noun.Substring(0, noun.Length - 1) + "ям"); // Dative
                forms.Add(noun.Substring(0, noun.Length - 1) + "й"); // Accusative
                forms.Add(noun.Substring(0, noun.Length - 1) + "ями"); // Instrumental
                forms.Add(noun.Substring(0, noun.Length - 1) + "ях"); // Prepositional
            }

            return forms;
        }
        static List<string> GetPossibleForms_ru(string noun)
        {
            var forms = new List<string> { noun };

            // Irregular nouns dictionary
            var irregularNouns = new Dictionary<string, List<string>>
        {
            { "ребёнок", new List<string> { "ребёнка", "ребёнку", "ребёнком", "ребёнке" } },
            { "человек", new List<string> { "человека", "человеку", "человеком", "человеке" } }
        };

            // Check for irregular nouns
            if (irregularNouns.ContainsKey(noun))
            {
                forms.AddRange(irregularNouns[noun]);
                return forms;
            }

            // Masculine nouns ending in a consonant
            if (IsMasculineNoun_ru(noun))
            {
                forms.Add(noun + "а"); // Genitive
                forms.Add(noun + "у"); // Dative
                forms.Add(noun + "ом"); // Instrumental
                forms.Add(noun + "е"); // Prepositional
            }
            // Feminine nouns ending in "а"
            else if (noun.EndsWith("а"))
            {
                forms.Add(noun.Substring(0, noun.Length - 1) + "ы"); // Genitive
                forms.Add(noun.Substring(0, noun.Length - 1) + "е"); // Dative
                forms.Add(noun.Substring(0, noun.Length - 1) + "у"); // Accusative
                forms.Add(noun.Substring(0, noun.Length - 1) + "ой"); // Instrumental
                forms.Add(noun.Substring(0, noun.Length - 1) + "е"); // Prepositional
            }
            // Feminine nouns ending in "я"
            else if (noun.EndsWith("я"))
            {
                forms.Add(noun.Substring(0, noun.Length - 1) + "и"); // Genitive
                forms.Add(noun.Substring(0, noun.Length - 1) + "е"); // Dative
                forms.Add(noun.Substring(0, noun.Length - 1) + "ю"); // Accusative
                forms.Add(noun.Substring(0, noun.Length - 1) + "ей"); // Instrumental
                forms.Add(noun.Substring(0, noun.Length - 1) + "е"); // Prepositional
            }
            // Neuter nouns ending in "о"
            else if (noun.EndsWith("о"))
            {
                forms.Add(noun.Substring(0, noun.Length - 1) + "а"); // Genitive
                forms.Add(noun.Substring(0, noun.Length - 1) + "у"); // Dative
                forms.Add(noun.Substring(0, noun.Length - 1) + "ом"); // Instrumental
                forms.Add(noun.Substring(0, noun.Length - 1) + "е"); // Prepositional
            }
            // Neuter nouns ending in "е"
            else if (noun.EndsWith("е"))
            {
                forms.Add(noun.Substring(0, noun.Length - 1) + "я"); // Genitive
                forms.Add(noun.Substring(0, noun.Length - 1) + "ю"); // Dative
                forms.Add(noun.Substring(0, noun.Length - 1) + "ем"); // Instrumental
                forms.Add(noun.Substring(0, noun.Length - 1) + "е"); // Prepositional
            }
            // Nouns ending in "ь"
            else if (noun.EndsWith("ь"))
            {
                if (IsMasculineNoun_ru(noun))
                {
                    forms.Add(noun.Substring(0, noun.Length - 1) + "я"); // Genitive
                    forms.Add(noun.Substring(0, noun.Length - 1) + "ю"); // Dative
                    forms.Add(noun.Substring(0, noun.Length - 1) + "ь"); // Accusative
                    forms.Add(noun.Substring(0, noun.Length - 1) + "ем"); // Instrumental
                    forms.Add(noun.Substring(0, noun.Length - 1) + "е"); // Prepositional
                }
                else
                {
                    forms.Add(noun.Substring(0, noun.Length - 1) + "и"); // Genitive
                    forms.Add(noun.Substring(0, noun.Length - 1) + "и"); // Dative
                    forms.Add(noun.Substring(0, noun.Length - 1) + "ь"); // Accusative
                    forms.Add(noun.Substring(0, noun.Length - 1) + "ью"); // Instrumental
                    forms.Add(noun.Substring(0, noun.Length - 1) + "и"); // Prepositional
                }
            }

            return forms;
        }
        static string Pluralize_ru(string noun)
        {
            // Irregular nouns dictionary
            var irregularNouns = new Dictionary<string, string>
        {
            { "ребёнок", "дети" },
            { "человек", "люди" }
        };

            // Check for irregular nouns
            if (irregularNouns.ContainsKey(noun))
            {
                return irregularNouns[noun];
            }

            // Masculine nouns ending in a consonant
            if (IsMasculineNoun_ru(noun))
            {
                return noun + "ы";
            }
            // Feminine nouns ending in "а"
            else if (noun.EndsWith("а"))
            {
                return noun.Substring(0, noun.Length - 1) + "ы";
            }
            // Feminine nouns ending in "я"
            else if (noun.EndsWith("я"))
            {
                return noun.Substring(0, noun.Length - 1) + "и";
            }
            // Neuter nouns ending in "о"
            else if (noun.EndsWith("о"))
            {
                return noun.Substring(0, noun.Length - 1) + "а";
            }
            // Neuter nouns ending in "е"
            else if (noun.EndsWith("е"))
            {
                return noun.Substring(0, noun.Length - 1) + "я";
            }
            // Nouns ending in "ь"
            else if (noun.EndsWith("ь"))
            {
                return noun.Substring(0, noun.Length - 1) + "и";
            }
            // Default rule: add "ы"
            else
            {
                return noun + "ы";
            }
        }
        static bool IsMasculineNoun_ru(string noun)
        {
            // List of common masculine nouns ending in "ь"
            var masculineNounsEndingInSoftSign = new HashSet<string>
        {
            "день", "конь", "путь", "гость", "камень"
        };

            // List of common feminine nouns ending in "ь"
            var feminineNounsEndingInSoftSign = new HashSet<string>
        {
            "ночь", "мышь", "дочь", "тень", "площадь"
        };

            // Check if the noun is in the list of masculine or feminine nouns ending in "ь"
            if (masculineNounsEndingInSoftSign.Contains(noun))
            {
                return true;
            }
            if (feminineNounsEndingInSoftSign.Contains(noun))
            {
                return false;
            }

            // Simplified check for masculine nouns ending in a consonant or soft sign
            // In a real application, you might need a more comprehensive check
            return !noun.EndsWith("а") && !noun.EndsWith("я") && !noun.EndsWith("о") && !noun.EndsWith("е") && !noun.EndsWith("ь");
        }
        static string Pluralize_da(string noun)
        {
            // Irregular nouns dictionary
            var irregularNouns = new Dictionary<string, string>
            {
            { "mand", "mænd" },
            { "barn", "børn" },
            { "tand", "tænder" },
            { "fod", "fødder" },
            { "mus", "mus" },
            { "ko", "køer" },
            { "gås", "gæs" },
            { "bog", "bøger" },
            { "nat", "nætter" },
            { "and", "ænder" },
            { "hånd", "hænder" },
            { "bror", "brødre" },
            { "mor", "mødre" },
            { "far", "fædre" },
            { "søster", "søstre" },
            { "datter", "døtre" },
            { "fisk", "fisk" },
            { "lam", "lam" },
            { "kvinde", "kvinder" }
            };

            // Check for irregular nouns
            if (irregularNouns.ContainsKey(noun))
            {
                return irregularNouns[noun];
            }

            // Common gender nouns (en-words)
            if (IsCommonGender_da(noun))
            {
                if (noun.EndsWith("e"))
                {
                    return noun + "r";
                }
                else if (noun.EndsWith("r"))
                {
                    return noun + "e";
                }
                else
                {
                    return noun + "er";
                }
            }

            // Neuter gender nouns (et-words)
            if (IsNeuterGender_da(noun))
            {
                if (noun.EndsWith("e"))
                {
                    return noun + "r";
                }
                else
                {
                    return noun + "e";
                }
            }

            // Default rule: add "er"
            return noun + "er";
        }
        static bool IsCommonGender_da(string noun)
        {
            // Simplified check for common gender (en-words)
            // In a real application, you might need a more comprehensive check
            return !IsNeuterGender_da(noun);
        }
        static bool IsNeuterGender_da(string noun)
        {
            // Simplified check for neuter gender (et-words)
            // In a real application, you might need a more comprehensive check
            return noun.EndsWith("hus") || noun.EndsWith("barn");
        }
        static bool IsVowel(char c)
        {
            return "aeiou".IndexOf(c) >= 0;
        }
        static string Pluralize_en(string noun)
        {
            // Irregular nouns dictionary
            var irregularNouns = new Dictionary<string, string>
            {
            { "child", "children" },
            { "man", "men" },
            { "woman", "women" },
            { "mouse", "mice" },
            { "goose", "geese" },
            { "foot", "feet" },
            { "tooth", "teeth" },
            { "person", "people" },
            { "cactus", "cacti" },
            { "focus", "foci" },
            { "fungus", "fungi" },
            { "nucleus", "nuclei" },
            { "syllabus", "syllabi" },
            { "analysis", "analyses" },
            { "diagnosis", "diagnoses" },
            { "oasis", "oases" },
            { "thesis", "theses" },
            { "crisis", "crises" },
            { "phenomenon", "phenomena" },
            { "criterion", "criteria" }
            };

            // Check for irregular nouns
            if (irregularNouns.ContainsKey(noun))
            {
                return irregularNouns[noun];
            }

            // Nouns ending in "s", "x", "z", "ch", "sh"
            if (noun.EndsWith("s") || noun.EndsWith("x") || noun.EndsWith("z") || noun.EndsWith("ch") || noun.EndsWith("sh"))
            {
                return noun + "es";
            }

            // Nouns ending in a consonant + "y"
            if (noun.EndsWith("y") && noun.Length > 1 && !IsVowel(noun[noun.Length - 2]))
            {
                return noun.Substring(0, noun.Length - 1) + "ies";
            }

            // Nouns ending in "f" or "fe"
            if (noun.EndsWith("f"))
            {
                return noun.Substring(0, noun.Length - 1) + "ves";
            }
            if (noun.EndsWith("fe"))
            {
                return noun.Substring(0, noun.Length - 2) + "ves";
            }

            // Default rule: add "s"
            return noun + "s";
        }
        public Dictionary<string, List<string>> ExtractTermsFromText(string text)
        {
            Dictionary<string, List<string>> foundTerms = new Dictionary<string, List<string>>();
            string[] paragraphs = text.Split(new[] { "\r\n\r\n", "\n\n", ".", ",", "?", "!" }, StringSplitOptions.None);

            foreach (string paragraph in paragraphs)
            {
                var paragraph1 = paragraph;
                paragraph1 = AddPluralsAndEdForms(paragraph1);
                SearchTermsInParagraph(paragraph1, foundTerms);
            }
            return RemoveSingleTermsIfLongerVersionsExist(foundTerms, text); //foundTerms; // 
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
            if (Form1.SOURCE_LANG_CODE == "en-gb" || Form1.SOURCE_LANG_CODE == "en-us")
            {
                newwords.Add(".");
                List<string> newwords1 = new List<string>();
                foreach (var word in words)
                {
                    if (word.EndsWith("s"))
                        newwords1.Add(word.Substring(0, word.Length - 1));
                    else
                        newwords1.Add(word);
                }
                newwords1.Add(".");
                if (string.Join("", newwords) != string.Join("", newwords1))
                    newwords.AddRange(newwords1);
            }
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
            TrieNode root = trie.GetRoot();
            for (int i = 0; i < words.Length; i++)
            {
                var potentialTerm = new List<string>();
                TrieNode node = root;
                int j = i;

                while (j < words.Length)
                {
                    string cleanedWord = CleanWord(words[j]);
                    bool found = false;

                    // Check for exact match first
                    if (node.Children.ContainsKey(cleanedWord))
                    {
                        node = node.Children[cleanedWord];
                        potentialTerm.Add(cleanedWord);
                        found = true;
                    }
                    else if (Form1.SOURCE_LANG_CODE != "en-gb" && Form1.SOURCE_LANG_CODE != "en-us")
                    {
                        // Check for match with the last 1 or 2 letters removed
                        for (int n = 1; n <= 2 && cleanedWord.Length > n + 5; n++)
                        {
                            string truncatedWord = cleanedWord.Substring(0, cleanedWord.Length - n);
                            foreach (var child in node.Children)
                            {
                                if (child.Key.StartsWith(truncatedWord) && Math.Abs(child.Key.Length - truncatedWord.Length) < 2)
                                {
                                    node = child.Value;
                                    potentialTerm.Add(child.Key);
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (found)
                    {
                        if (node.IsEndOfTerm)
                        {
                            TrieNode endNode;
                            if (trie.Search(potentialTerm, out endNode, 2))
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
                        // If no match is found, break the loop and move to the next starting word
                        break;
                    }

                    j++;
                }
            }            
        }
        private void SearchTermsInParagraph1(string paragraph, Dictionary<string, List<string>> foundTerms, double similarityThreshold = 0.97)
        {
            var words = paragraph.Split(new[] { ' ', '\t', '\n', '\r', '/' }, StringSplitOptions.RemoveEmptyEntries);
            TrieNode root = trie.GetRoot();

            for (int i = 0; i < words.Length; i++)
            {
                TrieNode node = root;
                var potentialTerm = new List<string>();

                for (int j = i; j < words.Length; j++)
                {
                    string cleanedWord = CleanWord(words[j]);
                    potentialTerm.Add(cleanedWord);

                    if (node.Children.ContainsKey(cleanedWord))
                    {
                        node = node.Children[cleanedWord];

                        // Check if the current node is the end of a term in the Trie
                        if (node.IsEndOfTerm)
                        {
                            AddToFoundTerms(foundTerms, node);
                        }

                        // Continue searching for exact matches in subsequent words
                        SearchRemainingWords(node, words, j + 1, potentialTerm, foundTerms, similarityThreshold);
                    }
                    else
                    {
                        // Attempt fuzzy matching if no exact match is found
                        FuzzyMatchRemainingWords(node, words, j, potentialTerm, foundTerms, similarityThreshold);
                        break; // Break the loop if no exact match is found
                    }
                }
            }
        }
        //private void SearchTermsInParagraph(string paragraph, Dictionary<string, List<string>> foundTerms, double similarityThreshold = 0.97, int suffixLength = 3)
        //{
        //    var words = paragraph.Split(new[] { ' ', '\t', '\n', '\r', '/' }, StringSplitOptions.RemoveEmptyEntries);
        //    TrieNode root = trie.GetRoot();

        //    for (int i = 0; i < words.Length; i++)
        //    {
        //        TrieNode node = root;
        //        var potentialTerm = new List<string>();
        //        int j = i;
        //        bool foundExactMatch = true;

        //        while (j < words.Length)
        //        {
        //            string cleanedWord = CleanWord(words[j]);

        //            if (node.Children.ContainsKey(cleanedWord))
        //            {
        //                node = node.Children[cleanedWord];
        //                potentialTerm.Add(cleanedWord);

        //                if (node.IsEndOfTerm)
        //                {
        //                    AddToFoundTerms(foundTerms, node);
        //                }
        //                // Continue searching for exact matches in subsequent words
        //                int maxDistance = CalculateMaxEditDistance(potentialTerm, similarityThreshold);
        //                SearchRemainingWords(node, words, j + 1, potentialTerm, foundTerms, maxDistance, similarityThreshold, suffixLength);
        //                //SearchRemainingWords(node, words, j + 1, potentialTerm, foundTerms, similarityThreshold);
        //            }
        //            else if (potentialTerm.Count >= 1) // Trigger fuzzy matching 
        //            {
        //                foundExactMatch = false;

        //                // Calculate the max allowable edit distance based on the 90% similarity threshold
        //                int maxDistance = CalculateMaxEditDistance(potentialTerm, similarityThreshold);

        //                var fuzzyMatches = FuzzySearchForCombinedTerm(potentialTerm, cleanedWord, node, maxDistance, suffixLength);

        //                foreach (var fuzzyMatch in fuzzyMatches)
        //                {
        //                    TrieNode fuzzyNode = fuzzyMatch.Value;

        //                    if (fuzzyNode.IsEndOfTerm)
        //                    {
        //                        AddToFoundTerms(foundTerms, fuzzyNode);
        //                    }

        //                    SearchRemainingWords(fuzzyNode, words, j + 1, fuzzyMatch.Key, foundTerms, maxDistance, similarityThreshold, suffixLength);
        //                }
        //                break;
        //            }
        //            else
        //            {
        //                foundExactMatch = false;
        //                break;
        //            }
        //            j++;
        //        }

        //        if (!foundExactMatch)
        //        {
        //            continue;
        //        }
        //    }
        //}
        private void FuzzyMatchRemainingWords(TrieNode currentNode, string[] words, int startIndex, List<string> potentialTerm, Dictionary<string, List<string>> foundTerms, double similarityThreshold)
        {
            string combinedTermString = string.Join(" ", potentialTerm);

            foreach (var child in currentNode.Children)
            {
                var newPotentialTerm = new List<string>(potentialTerm) { child.Key };
                string currentPrefix = string.Join(" ", newPotentialTerm);

                int editDistance = CalculateEditDistance(combinedTermString, currentPrefix);
                double similarity = 1.0 - (double)editDistance / Math.Max(combinedTermString.Length, currentPrefix.Length);

                if (similarity >= similarityThreshold)
                {
                    TrieNode childNode = child.Value;

                    if (childNode.IsEndOfTerm)
                    {
                        AddToFoundTerms(foundTerms, childNode);
                    }

                    // Continue searching recursively down the Trie
                    FuzzyMatchRemainingWords(childNode, words, startIndex, newPotentialTerm, foundTerms, similarityThreshold);

                    // Continue searching in subsequent words if needed
                    if (startIndex + 1 < words.Length)
                    {
                        SearchRemainingWords(childNode, words, startIndex + 1, newPotentialTerm, foundTerms, 1, similarityThreshold, 3);
                    }
                }
            }
        }
        private void SearchRemainingWords(TrieNode currentNode, string[] words, int startIndex, List<string> potentialTerm, Dictionary<string, List<string>> foundTerms, double similarityThreshold)
        {
            if (startIndex >= words.Length)
            {
                return;
            }

            for (int i = startIndex; i < words.Length; i++)
            {
                string cleanedWord = CleanWord(words[i]);

                // Attempt to traverse the Trie node based on the current word
                if (currentNode.Children.ContainsKey(cleanedWord))
                {
                    TrieNode nextNode = currentNode.Children[cleanedWord];
                    potentialTerm.Add(cleanedWord);

                    // If this node marks the end of a term, add it to the found terms
                    if (nextNode.IsEndOfTerm)
                    {
                        AddToFoundTerms(foundTerms, nextNode);
                    }

                    // Continue searching the remaining words in the paragraph
                    SearchRemainingWords(nextNode, words, i + 1, potentialTerm, foundTerms, similarityThreshold);

                    // After recursion, remove the last word to backtrack and explore other paths
                    potentialTerm.RemoveAt(potentialTerm.Count - 1);
                }
                else
                {
                    // If exact match is not found, attempt fuzzy matching
                    FuzzyMatchRemainingWords(currentNode, words, i, potentialTerm, foundTerms, similarityThreshold);
                    break; // Stop further traversal if no match is found
                }
            }
        }
        private void SearchRemainingWords(TrieNode node, string[] words, int startIndex, List<string> currentPrefix, Dictionary<string, List<string>> foundTerms, int maxDistance, double similarityThreshold, int suffixLength)
        {
            for (int i = startIndex; i < words.Length; i++)
            {
                string cleanedWord = CleanWord(words[i]);

                if (node.Children.ContainsKey(cleanedWord))
                {
                    node = node.Children[cleanedWord];
                    currentPrefix.Add(cleanedWord);

                    if (node.IsEndOfTerm)
                    {
                        AddToFoundTerms(foundTerms, node);
                    }
                }
                else if (maxDistance > 0 && currentPrefix.Count >= 1)
                {
                    var fuzzyMatches = FuzzySearchForCombinedTerm(currentPrefix, cleanedWord, node, maxDistance, suffixLength);

                    foreach (var fuzzyMatch in fuzzyMatches)
                    {
                        TrieNode fuzzyNode = fuzzyMatch.Value;

                        if (fuzzyNode.IsEndOfTerm)
                        {
                            AddToFoundTerms(foundTerms, fuzzyNode);
                        }

                        SearchRemainingWords(fuzzyNode, words, i + 1, fuzzyMatch.Key, foundTerms, maxDistance, similarityThreshold, suffixLength);
                    }
                    break;
                }
                else
                {
                    break;
                }
            }
        }

        // Fuzzy search for a combined term with suffix-based edit distance check
        private Dictionary<List<string>, TrieNode> FuzzySearchForCombinedTerm(List<string> potentialTerm, string nextWord, TrieNode node, int maxDistance, int suffixLength)
        {
            var results = new Dictionary<List<string>, TrieNode>();

            foreach (var child in node.Children)
            {
                var combinedTerm = new List<string>(potentialTerm) { child.Key };
                string combinedTermString = string.Join(" ", combinedTerm);
                string searchString = string.Join(" ", combinedTerm) + " " + nextWord;

                //int editDistance = CalculateEditDistanceWithSuffixCheck(combinedTermString, searchString, suffixLength);
                int editDistance = CalculateEditDistance(combinedTermString, searchString);
                if (editDistance <= maxDistance)
                {
                    results[combinedTerm] = child.Value;
                }
            }

            return results;
        }
        // Helper function to continue searching for the remaining words
        //private void SearchRemainingWords(TrieNode node, string[] words, int startIndex, List<string> currentPrefix, Dictionary<string, List<string>> foundTerms, int maxDistance)
        //{
        //    for (int i = startIndex; i < words.Length; i++)
        //    {
        //        string cleanedWord = CleanWord(words[i]);

        //        if (node.Children.ContainsKey(cleanedWord))
        //        {
        //            node = node.Children[cleanedWord];
        //            currentPrefix.Add(cleanedWord);

        //            if (node.IsEndOfTerm)
        //            {
        //                AddToFoundTerms(foundTerms, node);
        //            }
        //        }
        //        else if (maxDistance > 0)
        //        {
        //            var fuzzyMatches = FuzzySearchForWord(node, cleanedWord, maxDistance);

        //            foreach (var fuzzyMatch in fuzzyMatches)
        //            {
        //                var extendedTerm = new List<string>(currentPrefix) { fuzzyMatch.Key };
        //                TrieNode fuzzyNode = fuzzyMatch.Value;

        //                if (fuzzyNode.IsEndOfTerm)
        //                {
        //                    AddToFoundTerms(foundTerms, fuzzyNode);
        //                }

        //                SearchRemainingWords(fuzzyNode, words, i + 1, extendedTerm, foundTerms, maxDistance);
        //            }
        //            break;
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //}

        // Fuzzy search for a single word
        private Dictionary<string, TrieNode> FuzzySearchForWord(TrieNode node, string searchWord, int maxDistance)
        {
            var results = new Dictionary<string, TrieNode>();

            foreach (var child in node.Children)
            {
                int editDistance = CalculateEditDistanceWithSuffixCheck(child.Key, searchWord, 2);
                if (editDistance <= maxDistance)
                {
                    results[child.Key] = child.Value;
                }
            }

            return results;
        }
        // Function to calculate the maximum allowable edit distance based on the similarity threshold
        private int CalculateMaxEditDistance(List<string> potentialTerm, double similarityThreshold)
        {
            int termLength = potentialTerm.Sum(word => word.Length);
            return (int)Math.Floor(termLength * (1 - similarityThreshold));
        }
        private int CalculateEditDistanceWithSuffixCheck(string word1, string word2, int suffixLength)
        {
            int m = word1.Length;
            int n = word2.Length;

            // If words differ significantly in length, it's not just a suffix change
            if (Math.Abs(m - n) > suffixLength)
            {
                return int.MaxValue;
            }

            var dp = new int[m + 1, n + 1];

            for (int i = 0; i <= m; i++)
            {
                dp[i, 0] = i;
            }

            for (int j = 0; j <= n; j++)
            {
                dp[0, j] = j;
            }

            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    int cost = (word1[i - 1] == word2[j - 1]) ? 0 : 1;

                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), // Deletion or Insertion
                        dp[i - 1, j - 1] + cost); // Substitution
                }
            }

            // Check if the difference is only in the suffix
            if (dp[m, n] <= suffixLength)
            {
                int minLen = Math.Min(m, n);
                for (int i = 0; i < minLen - suffixLength; i++)
                {
                    if (word1[i] != word2[i])
                    {
                        return int.MaxValue; // Prefixes are different, so it's not just a suffix change
                    }
                }
            }

            return dp[m, n];
        }
       
        // Function to calculate the Edit Distance between two words
        private int CalculateEditDistance(string word1, string word2)
        {
            int m = word1.Length;
            int n = word2.Length;
            var dp = new int[m + 1, n + 1];

            for (int i = 0; i <= m; i++)
            {
                dp[i, 0] = i;
            }

            for (int j = 0; j <= n; j++)
            {
                dp[0, j] = j;
            }

            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    int cost = (word1[i - 1] == word2[j - 1]) ? 0 : 1;

                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), // Deletion or Insertion
                        dp[i - 1, j - 1] + cost); // Substitution
                }
            }

            return dp[m, n];
        }
        private string CleanWord(string word)
        {
            return word.ToLower().Trim('.', ',', ';', ':', '!', '?', '(', ')', '[', ']', '{', '}', '\"', '\'', '/', '\\', '<', '>', '”', '“');
        }
        // Helper function to add found terms to the dictionary
        private void AddToFoundTerms(Dictionary<string, List<string>> foundTerms, TrieNode node)
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
