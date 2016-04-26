using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LyricTools
{
    class ConversionTools
    {
        public XElement SongVerses;

        enum ParagraphType
        {
            /// <summary>
            /// Verse
            /// </summary>
            v = 1,
            /// <summary>
            /// Chorus
            /// </summary>
            c,
            /// <summary>
            /// Bridge
            /// </summary>
            b,
            /// <summary>
            /// Intro
            /// </summary>
            i,
            /// <summary>
            /// Ending
            /// </summary>
            e
        }
        class Paragraph
        {
            public string Text;
            /// <summary>
            /// Might not be initialised! Manually set this if you know you want to use it later.
            /// </summary>
            public string UppercaseText;
            public ParagraphType Type;
            /// <summary>
            /// Number of Type. Ex - Chorus [1] or Verse [5]. Note: Might not be assigned!
            /// </summary>
            public int Number;
            /// <summary>
            /// "ID" of this paragraph. Ex - "C1" or "V5".
            /// </summary>
            public string ID;

            public Paragraph()
            {

            }
        }

        class Song
        {
            public string ID = "", Title = "", Copyright = "", VerseOrder = "";
            public string[] RawParagraphs, Authors;
            public List<Paragraph> Verses = new List<Paragraph>();
            public List<Paragraph> Choruses = new List<Paragraph>();
            public List<Paragraph> Bridges = new List<Paragraph>();

            /// <summary>
            /// Contains a mix of verses, choruses and bridges in the sequence in which they are added.
            /// This simplifies conversion to OpenSong XML.
            /// </summary>
            public List<Paragraph> OutputParagraphs = new List<Paragraph>();

            /// <summary>
            /// XML description of this song, exported from Lyrix's LXDATA database
            /// (which can be given a .mbd extension, then opened in Microsoft Access. Export the Songs table as XML, also include SongVerses.)
            /// </summary>
            XElement SongData;
            /// <summary>
            /// Not initialised the same as SongData, so might be null.
            /// Holds a "simplified" version of SongData - i.e. -  all verses are lowercase and have been trimmed, etc
            /// The idea is to initialise this if a verse wasn't found in SongData
            /// </summary>
            XElement SongDataSimplified;

            public void ParseFromLyrixUsingSongVersesFile(string sourcetext)
            {
                RawParagraphs = sourcetext.Split(new[] { "\n\n" }, StringSplitOptions.None);
                List<Paragraph> unassignedChoruses = new List<Paragraph>();

                // Get ID and Title:
                try
                {
                    string[] firstLines = RawParagraphs[0].Split('\n');
                    for (int i = 0; i < firstLines.Length; i++)
                    {
                        if (firstLines[i].StartsWith("--")) // Find the first line of dashes
                        {
                            // Set song ID to the next line's text
                            ID = firstLines[i + 1];

                            // If there is another line, set the song's title to that
                            if (!string.IsNullOrWhiteSpace(firstLines[i + 2]))
                                Title = firstLines[i + 2];
                            else
                                Title = "";
                            break;
                        }
                    }

                    // Find the song in SongVerses:
                    string lowercaseID = ID.ToLowerInvariant();
                    // First try ID:
                    SongData = Program.conversionTools.SongVerses.Elements("Songs").Where(s => s.Element("SongID").Value.Trim().ToLowerInvariant() == lowercaseID).First();
                    if (SongData == null)
                    {//Otherwise just try the title
                        string lowercaseName = Title.ToLowerInvariant();
                        SongData = Program.conversionTools.SongVerses.Elements("Songs").Where(s => s.Element("Name").Value.Trim().ToLowerInvariant() == lowercaseName).First();
                    }
                    if (SongData == null)
                        throw new Exception("Song not found in Lyrix database: " + ID);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception caught while trying to get song ID and name: " + e.ToString());
                }

                // Decode the lyrics:
                for (int i = 1; i < RawParagraphs.Length; i++) // iterate through the rest of the paragraphs
                {
                    try
                    {
                        if (String.IsNullOrWhiteSpace(RawParagraphs[i]))
                        {
                            //Do nothing
                        }
                        else if (Regex.IsMatch(RawParagraphs[i], @"^\d+. ")) // if the paragraph starts with a number, then a dot, then a space...
                        {
                            //...then this is a verse.
                            //Get the verse number:
                            int versenum = 9; // default in case of an error
                            int indexOfFirstSpace = RawParagraphs[i].IndexOf(' ');
                            int.TryParse(RawParagraphs[i].Substring(0, indexOfFirstSpace - 1), out versenum);

                            Paragraph p = new Paragraph();
                            p.Type = ParagraphType.v;
                            p.Text = RawParagraphs[i].Substring(indexOfFirstSpace + 1);
                            if (!String.IsNullOrWhiteSpace(p.Text)) // Ignore if empty
                            {
                                p.Number = versenum;
                                p.ID = "v" + versenum;

                                //Only add if this to the verse list is the first occurrence
                                if (!OutputParagraphs.Any(v => v.Text.Equals(p.Text, StringComparison.Ordinal)))
                                {
                                    OutputParagraphs.Add(p);
                                }

                                //Either way, update the verse order
                                VerseOrder += p.ID + " ";
                            }
                        }
                        else if (RawParagraphs[i].StartsWith("Words/Music: ")
                            || RawParagraphs[i].StartsWith("(c)")
                            || RawParagraphs[i].StartsWith("(Alternatiewe ")
                            || RawParagraphs[i].StartsWith("(alternatiewe ")
                            || RawParagraphs[i].StartsWith("(Harmonisasie")
                            || RawParagraphs[i].StartsWith("(harmonisasie"))
                        {
                            //If the paragraph contains additional info, first split it into lines and process each of them:
                            string[] lines = RawParagraphs[i].Split('\n');
                            foreach (string line in lines)
                            {
                                if (line.StartsWith("Words/Music: "))
                                {
                                    Authors = line.Substring(line.IndexOf(": ") + 2).Split(new[] { ", " }, StringSplitOptions.None);
                                }
                                else if (line.StartsWith("(c)"))
                                {
                                    Copyright = line.Trim();
                                }
                                else
                                {
                                    // Add this as as Other verse
                                    Paragraph Other1 = new Paragraph();
                                    Other1.ID = "o1";
                                    Other1.Text = line;
                                    OutputParagraphs.Add(Other1);
                                    // ...but don't add it to the verseOrder. Thus, this isn't displayed by default.
                                }
                            }
                        }
                        else
                        {
                            //Either chorus or bridge.

                            //First, assume new Chorus:
                            Paragraph p = new Paragraph();
                            p.Type = ParagraphType.c;
                            p.Text = RawParagraphs[i];

                            if (!String.IsNullOrWhiteSpace(p.Text)) // Ignore if empty
                            {
                                string searchText = p.Text.Replace("\n", " "); // the text to search for doesn't have newlines
                                XElement foundVerse = SongData.Elements("SongVerses").Where(sv => sv.Element("PartText").Value == searchText).FirstOrDefault();
                                if (foundVerse == null)
                                {
                                    //Use/create the simplified version of SongData
                                    if (SongDataSimplified == null)
                                    {
                                        SongDataSimplified = new XElement(SongData); // deep copy!
                                        foreach (var verse in SongDataSimplified.Elements("SongVerses"))
                                        {
                                            string newText = verse.Element("PartText").Value.Trim().ToLowerInvariant().Replace("‘", @"'"); // Convert to lowercase, trim, replace wrong apostrophes
                                            newText = Regex.Replace(newText, @"[ ]{2,}", " "); // Replace multiple spaces with just one
                                            verse.Element("PartText").SetValue(newText);
                                        }
                                    }
                                    searchText = searchText.Trim().ToLowerInvariant().Replace("‘", @"'"); // Convert to lowercase and trim
                                    searchText = Regex.Replace(searchText, @"[ ]{2,}", " "); // Replace multiple spaces with just one
                                        
                                    var foundVerseList = SongDataSimplified.Elements("SongVerses").Where(sv => sv.Element("PartText").Value == searchText);
                                    if (foundVerseList.Count() == 1)
                                        foundVerse = foundVerseList.FirstOrDefault();
                                    else if (foundVerseList.Count() > 0)
                                    {
                                        Console.WriteLine("Multiple possible verses for song found");
                                        foundVerse = foundVerseList.FirstOrDefault();
                                    }
                                    else
                                    {
                                        Console.WriteLine(">> Verse type not found, assuming chorus: " + Title + ": " + p.Text);

                                        //Add this to the list of unsorted choruses, which will be checked after all "fixed" choruses have been added
                                        Paragraph existingUnassignedChorus = unassignedChoruses.SingleOrDefault(c => c.Text.Equals(p.Text, StringComparison.Ordinal));
                                        if (existingUnassignedChorus != null)
                                        {
                                            // Existing chorus, get its ID and update the verse order
                                            VerseOrder += existingUnassignedChorus.ID + " ";
                                        }
                                        else
                                        {
                                            // Non-existing chorus, add it and update the verse order
                                            p.ID = "TEMP" + (unassignedChoruses.Count() + 1);
                                            unassignedChoruses.Add(p);
                                            OutputParagraphs.Add(p); // Should this happen here or rather later?
                                            VerseOrder += p.ID + " ";
                                        }
                                    }
                                }

                                if (foundVerse != null)
                                {
                                    // Get verse type:
                                    switch (foundVerse.Element("PartID").Value)
                                    {
                                        /*
                                            * 0: Intro 
                                            * 1: Verse
                                            * 2: Bridge
                                            * 3: Chorus
                                            * 4: Ending
                                            * 
                                            * (5: Page)
                                            * (6: Slide)
                                            * */
                                        case "0":
                                            p.Type = ParagraphType.i;
                                            p.ID = "i";
                                            break;

                                        case "1":
                                            p.Type = ParagraphType.v;
                                            p.ID = "v";
                                            break;

                                        case "2":
                                            p.Type = ParagraphType.b;
                                            p.ID = "b";
                                            break;

                                        default:
                                        case "3":
                                            p.Type = ParagraphType.c;
                                            p.ID = "c";
                                            break;

                                        case "4":
                                            p.Type = ParagraphType.e;
                                            p.ID = "e";
                                            break;
                                    }
                                    // Get verse number:
                                    p.Number = Int16.Parse(foundVerse.Element("PartNumber").Value);
                                    p.ID += p.Number;
                                    
                                    //First, check if this is a re-occurring paragraph:
                                    Paragraph existingParagraph = OutputParagraphs.SingleOrDefault(o => o.Text.Equals(p.Text, StringComparison.Ordinal));
                                    if (existingParagraph != null)
                                    {
                                        // Existing paragraph, get its ID and update the verse order
                                        //existingParagraph.ID = SongVerses.Parent.Elements("SongVerses").Where(sv => sv.Element(""))
                                        VerseOrder += existingParagraph.ID + " ";
                                    }
                                    else
                                    {
                                        // Add as new paragraph and update the verse order
                                        OutputParagraphs.Add(p);
                                        VerseOrder += p.ID + " ";
                                    }
                                } // if (not empty)
                            } // if (found)
                            else
                            {
                                if (!String.IsNullOrWhiteSpace(p.Text))
                                    Console.WriteLine(">>Couldn't find verse in song \""+Title+"\" ("+ID+"):\n"+p.Text);
                            }
                        } // if (not verse)
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception caught: "+e.ToString());
                        throw e;
                    }
                } // paragraph-for

                // Assign all unassigned choruses (if any), i.e., assign them valid IDs
                if (unassignedChoruses.Any())
                {
                    
                    // First, find the highest chorus number in OutputParagraphs
                    var list1 = OutputParagraphs.Where(p => p.Type == ParagraphType.c && !p.ID.StartsWith("TEMP")).OrderByDescending(p => p.ID);
                    int k = 0; // chorus index
                    if (list1.Any())
                    {
                        int.TryParse(list1.FirstOrDefault().ID.Substring(1), out k); // Get the chorus number
                    }
                    k++; // k is now the next available chorus number
                    foreach (var chorus in unassignedChoruses)
                    {
                        try
                        {
                            // Get the old and new IDs
                            string oldID = chorus.ID;
                            string newID = "c" + k;

                            // Replace old IDs with the new ones
                            VerseOrder = VerseOrder.Replace(oldID, newID);
                            chorus.ID = newID;
                            //OutputParagraphs.Where(p => p.ID == oldID).Single().ID = newID; // already changed - the same Paragraph object is referenced
                        }
                        catch (Exception e1)
                        {
                            Console.WriteLine("Error in chorus assignment - song might be wrong!");
                        }
                    }
                }

                //Remove any empty outputParagraphs
                //Find empty items
                try
                {
                    var EmptyParagraphs = OutputParagraphs.Where(p => String.IsNullOrWhiteSpace(p.Text)).ToList();
                    foreach (Paragraph p in EmptyParagraphs)
                    {
                        // Remove the paragraph from the verse order
                        VerseOrder = VerseOrder.Replace(p.ID, "");

                        // Remove this paragraph from OutputParagraphs
                        OutputParagraphs.Remove(p);
                    }
                }
                catch (Exception e2)
                {
                    Console.WriteLine("Error removing empty paragraph: " + e2.ToString());
                }
            }

            public void ParseFromLyrix(string sourcetext)
            {
                try
                {
                    RawParagraphs = sourcetext.Split(new[] { "\n\n" }, StringSplitOptions.None);

                    // Get ID and Title:
                    try
                    {
                        string[] firstLines = RawParagraphs[0].Split('\n');
                        for (int i = 0; i < firstLines.Length; i++)
                        {
                            if (firstLines[i].StartsWith("--")) // Find the first line of dashes
                            {
                                // Set song ID to the next line's text
                                ID = firstLines[i + 1];

                                // If there is another line, set the song's title to that
                                if (!string.IsNullOrWhiteSpace(firstLines[i + 2]))
                                    Title = firstLines[i + 2];
                                else
                                    Title = "";
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception caught while trying to get song ID and name: " + e.ToString());
                    }

                    // Decode the lyrics:
                    for (int i = 1; i < RawParagraphs.Length; i++) // iterate through the rest of the paragraphs
                    {
                        try
                        {
                            if (String.IsNullOrWhiteSpace(RawParagraphs[i]))
                            {
                                //Do nothing
                            }
                            if (Regex.IsMatch(RawParagraphs[i], @"^\d. ")) // if the paragraph starts with a number, then a dot, then a space...
                            {
                                //...then this is a verse.
                                //Get the verse number:
                                int versenum = 9; // default in case of an error
                                int indexOfFirstSpace = RawParagraphs[i].IndexOf(' ');
                                int.TryParse(RawParagraphs[i].Substring(0, indexOfFirstSpace-1) , out versenum);

                                Paragraph p = new Paragraph();
                                p.Type = ParagraphType.v;
                                p.Text = RawParagraphs[i].Substring(indexOfFirstSpace + 1);
                                if (!String.IsNullOrWhiteSpace(p.Text)) // Ignore if empty
                                {
                                    p.Number = versenum;
                                    p.ID = "v" + versenum;

                                    //Only add if this to the verse list is the first occurrence
                                    if (!Verses.Any(v => v.Text.Equals(p.Text, StringComparison.Ordinal)))
                                    {
                                        Verses.Add(p);
                                        OutputParagraphs.Add(p);
                                    }

                                    //Either way, update the verse order
                                    VerseOrder += p.ID + " ";
                                }
                            }
                            else if (RawParagraphs[i].StartsWith("Words/Music: ")
                                || RawParagraphs[i].StartsWith("(c)")
                                || RawParagraphs[i].StartsWith("(Alternatiewe harmonisasie: "))
                            {
                                //If the paragraph contains additional info, first split it into lines and process each of them:
                                string[] lines = RawParagraphs[i].Split('\n');
                                foreach (string line in lines)
                                {
                                    if (line.StartsWith("Words/Music: "))
                                    {
                                        Authors = line.Substring(line.IndexOf(": ")+2).Split(new[] { ", " }, StringSplitOptions.None);
                                    }
                                    else if (line.StartsWith("(c)"))
                                    {
                                        Copyright = line.Trim();
                                    }
                                    else if (line.StartsWith("(Alternatiewe harmonisasie: "))
                                    {
                                        // Add this as as Other verse
                                        Paragraph Other1 = new Paragraph();
                                        Other1.ID = "o1";
                                        Other1.Text = line;
                                        OutputParagraphs.Add(Other1);
                                        // ...but don't add it to the verseOrder. Thus, this isn't displayed by default.
                                    }
                                }
                            }
                            else
                            {
                                //Either chorus or bridge.

                                //New Chorus:
                                Paragraph p = new Paragraph();
                                p.Type = ParagraphType.c;
                                p.Text = RawParagraphs[i];

                                if (!String.IsNullOrWhiteSpace(p.Text)) // Ignore if empty
                                {
                                    //Compare text to existing choruses to determine if this is a new chorus
                                    Paragraph existingChorus = Choruses.SingleOrDefault(c => c.Text.Equals(p.Text, StringComparison.Ordinal));
                                    if (existingChorus != null)
                                    {
                                        // Existing chorus, get its ID and update the verse order
                                        VerseOrder += existingChorus.ID + " ";
                                    }
                                    else
                                    {
                                        //Not an existing chorus. Check if existing bridge
                                        Paragraph existingBridge = Bridges.SingleOrDefault(b => b.Text.Equals(p.Text, StringComparison.Ordinal));
                                        if (existingBridge != null)
                                        {
                                            //Existing bridge, get its ID and update the verse order
                                            VerseOrder += existingBridge.ID + " ";
                                        }
                                        else
                                        {
                                            //Not an existing bridge either.

                                            // TODO: Add code to distinguish better between choruses, pre-choruses and bridges

                                            // Add as new chorus
                                            p.Number = Choruses.Count + 1;
                                            p.ID = "c" + p.Number;

                                            Choruses.Add(p);
                                            OutputParagraphs.Add(p);

                                            VerseOrder += p.ID + " ";
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e2)
                        {
                            Console.WriteLine("Exception caught while trying to decode paragraph "+i+" of this song: "+sourcetext+"\n\nThe exception was: "+e2.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            public void WriteOpenLyricsXML(string path)
            {
                try
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    XmlWriter w = XmlWriter.Create(path, settings);

                    w.WriteStartDocument();
                    w.WriteStartElement("song", "http://openlyrics.info/namespace/2009/song");
                    w.WriteAttributeString("version", "0.8");
                    w.WriteAttributeString("createdIn", "OpenLP 2.4");
                    w.WriteAttributeString("modifiedIn", "OpenLP 2.4");
                    w.WriteAttributeString("modifiedDate", "2016-04-01T00:00:01");

                    w.WriteStartElement("properties");

                    w.WriteStartElement("titles");
                    w.WriteStartElement("title");
                    w.WriteString(Title);
                    w.WriteEndElement();
                    w.WriteStartElement("title"); //Use the Lyrix ID field as the Alternative Title field here
                    w.WriteString(ID);
                    w.WriteEndElement();
                    w.WriteEndElement();

                    //w.WriteStartElement("comments");
                    //w.WriteStartElement("comment");
                    //w.WriteString("");
                    //w.WriteEndElement();
                    //w.WriteEndElement();

                    w.WriteStartElement("copyright");
                    w.WriteString(Copyright);
                    w.WriteEndElement();

                    w.WriteStartElement("verseOrder");
                    w.WriteString(VerseOrder.Trim());
                    w.WriteEndElement();

                    //w.WriteStartElement("ccliNo");
                    //
                    //w.WriteEndElement();

                    w.WriteStartElement("authors");
                    if (Authors == null)
                    {
                        w.WriteStartElement("author");
                        w.WriteString("Unknown Author");
                        w.WriteEndElement();
                    }
                    else
                    {
                        foreach (string Author in Authors)
                        {
                            w.WriteStartElement("author");
                            w.WriteString(Author);
                            w.WriteEndElement();
                        }
                    }
                    w.WriteEndElement();
                    w.WriteEndElement();//properties

                    w.WriteStartElement("lyrics");
                    if (OutputParagraphs != null)
                    {
                        foreach (Paragraph p in OutputParagraphs)
                        {
                            w.WriteStartElement("verse");
                            w.WriteAttributeString("name", p.ID);
                            w.WriteStartElement("lines");
                            w.WriteRaw(p.Text.Replace("\n", "<br/>"));
                            w.WriteEndElement();
                            w.WriteEndElement();
                        }
                    }
                    w.WriteEndElement();

                    w.WriteEndElement(); //song
                    w.WriteEndDocument();
                    w.Close();
                }
                catch (Exception e1)
                {
                    Console.WriteLine("Error writing XML file: " + e1.ToString());
                }
            }
        }

        public string StartConversion(string inputPath, string outputPath, string inputFormat = "lyrix", string outputFormat = "openlyrics")
        {
            try
            {
                Song song = new Song();

                switch (inputFormat)
                {
                    default:
                    case "lyrix":
                        {
                            if (SongVerses == null)
                                song.ParseFromLyrix(File.ReadAllText(inputPath, Encoding.Default).Replace("\r\n", "\n"));
                            else
                            {
                                song.ParseFromLyrixUsingSongVersesFile(File.ReadAllText(inputPath, Encoding.Default).Replace("\r\n", "\n"));
                            }
                            break;
                        }
                }

                switch (outputFormat)
                {
                    default:
                    case "openlyrics":
                        {
                            //XElement OutputXML = song.ToOpenLyricsXML();
                            if (!String.IsNullOrWhiteSpace(outputPath))
                            {
                                // Save output file
                                // Get full path of output file
                                string filename = song.ID + ".xml";

                                //OutputXML.Save(outputPath + "\\" + filename, SaveOptions.None);
                                //using (XmlTextWriter writer = new XmlTextWriter(outputPath + "\\" + filename, Encoding.Default))
                                //{
                                //    writer.WriteRaw(OutputXML.ToString());
                                //}
                                
                                song.WriteOpenLyricsXML(outputPath + "\\" + filename);
                            }
                            break;
                        }
                }
                
                return inputPath + ": Done";
            }
            catch(Exception e)
            {
                return inputPath + ": Failed: " + e.ToString();
            }
        }

        /// <summary>
        /// Reads the SongVerses file into an XElement object
        /// </summary>
        /// <param name="SongVersesFileLocation"></param>
        public void SetSongVersesFile(string SongVersesFileLocation)
        {
            try
            {
                SongVerses = XElement.Load(SongVersesFileLocation);
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception caught in SetSongVersesFile: "+e.ToString());
            }
        }
    } //class
} //namespace
