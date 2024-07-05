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
            //richTextBox2.Text = lang;
            var terms = extractLangGlossaryTerms(slang, lang);
            var fio = new FileInfo(inputFile);
            var extension = fio.Extension;
            var fname = fio.Name.Substring(0, fio.Name.LastIndexOf(".")); 
            var dir = fio.DirectoryName;
            var outputFile = Path.Combine(dir, fname + "_withNotes" + extension);
            if(File.Exists(outputFile)) File.Delete(outputFile);
            File.Copy(inputFile, outputFile);
            GlossaryExtractor extractor = new GlossaryExtractor(terms);
            int newCounter = 0;
            var extractedTerms = new List<GlossaryTermScript>();
            var checks = new List<string>();
            var xlz = new Xlz(outputFile);
            var tus = xlz.TranslatableTransUnits;
            foreach (var tu in tus)
            {
                var source = tu.GetSource(false);
                Dictionary<string, List<string>> foundTerms = extractor.ExtractTermsFromText(source);
                if (foundTerms.Count > 0)
                {
                    //Console.WriteLine("Source id: " + tu.TransUnitID() + " ---------------");
                    //Console.WriteLine("Found terms: " + foundTerms.Count);
                    var note = "";
                    foreach (var term in foundTerms)
                    {
                        note += term.Key + " = " + term.Value[0] + "\n";
                        //Console.WriteLine("Term: " + term.Key);
                        var extractedTerm = new GlossaryTermScript { Level = term.Value[1], sLang = term.Key, tLang = term.Value[0] };
                        if (!checks.Contains(term.Key))
                        {
                            checks.Add(term.Key);
                            extractedTerms.Add(extractedTerm);
                        }
                    }
                    tu.Add(new XElement("note", new XAttribute("annotates", "Glossary"), new XAttribute("from", "MedDRA"), note));
                    newCounter += foundTerms.Count;
                }
            }
            //Console.WriteLine("Found terms: " + newCounter);
            xlz.Save2();
            //var uniqueSortedTerms = extractedTerms.OrderBy(term => term.en_gb).ToList();
            //SaveSimpleGlossaryTermsToTsv(uniqueSortedTerms, inputFile + "_" + lang + ".txt", lang);
            richTextBox2.Text += String.Format("Extracted Terms added to xlz file: <file://{0}> ", outputFile) + "\n";
        }
