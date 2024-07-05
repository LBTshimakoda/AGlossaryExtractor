        public List<GlossaryTermScript> getExtractedTermsForXlz(string inputFile)
        {
            if (glossaryTerms == null || glossaryTerms.Count == 0)
            {
                richTextBox2.Text += "Load glossary TXT first.\n";
                return null;
            }
            var slang = comboBox2.Text;
            var lang = comboBox1.Text;
            //richTextBox2.Text = lang;
            var terms = extractLangGlossaryTerms(slang, lang);
            GlossaryExtractor extractor = new GlossaryExtractor(terms);
            int newCounter = 0;
            var extractedTerms = new List<GlossaryTermScript>();
            var checks = new List<string>();
            var xlz = new Xlz(inputFile);
            var tus = xlz.TranslatableTransUnits;
            foreach (var tu in tus)
            {
                var source = tu.GetSource(false);
                Dictionary<string, List<string>> foundTerms = extractor.ExtractTermsFromText(source);
                if (foundTerms.Count > 0)
                {
                    //Console.WriteLine("Source id: " + tu.TransUnitID() + " ---------------");
                    //Console.WriteLine("Found terms: " + foundTerms.Count);
                    richTextBox2.Text += "\nSource id: " + tu.TransUnitID() + " ";
                    richTextBox2.Text += "(" + foundTerms.Count + "): ";
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
                            richTextBox2.Text += "| " + term.Key + " |";
                        }
                    }
                    tu.Add(new XElement("note", new XAttribute("annotates", "Glossary"), new XAttribute("from", "MedDRA"), note));
                    newCounter += foundTerms.Count;
                }
            }
            //Console.WriteLine("Found terms: " + newCounter);
            //xlz.Save2();
            richTextBox2.Text += "\n" + extractedTerms.Count + "\n";
            var uniqueSortedTerms = extractedTerms.OrderBy(term => term.sLang).ToList();
            richTextBox2.Text += "\n" + uniqueSortedTerms.Count + "\n";
            return uniqueSortedTerms;
        }
