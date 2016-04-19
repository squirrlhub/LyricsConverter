// Modified stuff

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

        enum ParagraphType {
            /// <summary>
            /// Verse
            /// </summary>
            v=1,
            /// <summary>
            /// Chorus
            /// </summary>
            c,
            /// <summary>
            /// Bridge
            /// </summary>
            b
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
            /// Number of Type. Ex - Chorus [1] or Verse [5].
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
            public string ID, Title, Copyright, VerseOrder = "";
            public string[] RawParagraphs, Authors;
            public List<Paragraph> Verses = new List<Paragraph>();
            public List<Paragraph> Choruses = new List<Paragraph>();
            public List<Paragraph> Bridges = new List<Paragraph>();

            /// <summary>
            /// Contains a mix of verses, choruses and bridges in the sequence in which they are added.
            /// This simplifies conversion to OpenSong XML.
            /// </summary>
            public List<Paragraph> OutputParagraphs = new List<Paragraph>();

            public void ParseFromLyrixUsingSongVersesFile(string sourcetext)
            {
                RawParagraphs = sourcetext.Split(new[] { "\n\n" }, StringSplitOptions.None);
                
                // Only use the OutputParagraphs List - everything is already indexed!

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
                                || RawParagraphs[i].StartsWith("(Alternatiewe harmonisasie: "))
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

                                //First, assume new Chorus:
                                Paragraph p = new Paragraph();
                                p.Type = ParagraphType.c;
                                p.Text = RawParagraphs[i];

                                /*
                                 * 0: Intro
                                 * 1: Verse
                                 * 2: Bridge
                                 * 3: Chorus
                                 * 4: Ending
                                 * 
                                 * 5: Page
                                 * 6: Slide
                                 * */

                                string searchText = p.Text.Replace("\n", "; ");

                                if (!String.IsNullOrWhiteSpace(p.Text)) // Ignore if empty
                                {
                                    //First, check if this is a re-occurring paragraph:
                                    Paragraph existingParagraph = OutputParagraphs.SingleOrDefault(o => o.Text.Equals(p.Text, StringComparison.Ordinal));
                                    if (existingParagraph != null)
                                    {
                                        // Existing paragraph, get its ID and update the verse order
                                        //existingParagraph.ID = SongVerses.Parent.Elements("SongVerses").Where(sv => sv.Element(""))
                                        VerseOrder += existingParagraph.ID + " ";
                                    }
                                } // if (not empty)

                                //{
                                //    //Compare text to existing choruses to determine if this is a new chorus
                                //    Paragraph existingChorus = Choruses.SingleOrDefault(c => c.Text.Equals(p.Text, StringComparison.Ordinal));
                                //    if (existingChorus != null)
                                //    {
                                //        // Existing chorus, get its ID and update the verse order
                                //        VerseOrder += existingChorus.ID + " ";
                                //    }
                                //    else
                                //    {
                                //        //Not an existing chorus. Check if existing bridge
                                //        Paragraph existingBridge = Bridges.SingleOrDefault(b => b.Text.Equals(p.Text, StringComparison.Ordinal));
                                //        if (existingBridge != null)
                                //        {
                                //            //Existing bridge, get its ID and update the verse order
                                //            VerseOrder += existingBridge.ID + " ";
                                //        }
                                //        else
                                //        {
                                //            //Not an existing bridge either.

                                //            // TODO: Add code to distinguish better between choruses, pre-choruses and bridges

                                //            // Add as new chorus
                                //            p.Number = Choruses.Count + 1;
                                //            p.ID = "c" + p.Number;

                                //            Choruses.Add(p);
                                //            OutputParagraphs.Add(p);

                                //            VerseOrder += p.ID + " ";
                                //        }
                                //    }
                                //        }
                                //    }
                                //}
                                //catch (Exception e2)
                                //{
                                //    Console.WriteLine("Exception caught while trying to decode paragraph "+i+" of this song: "+sourcetext+"\n\nThe exception was: "+e2.ToString());
                                //}
                            } // if (not verse)
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    } // paragraph-for
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

            //public XElement ToOpenLyricsXML()
            //{
            //    // First create the authors element, then just add it later
            //    XElement authors = new XElement("authors");
            //    if (Authors != null)
            //    {
            //        foreach (string Author in Authors)
            //        {
            //            authors.Add(new XElement("author", Author));
            //        }
            //    }

            //    // Next, create the lyrics element, add it later
            //    XElement lyrics = new XElement("lyrics");
            //    foreach (Paragraph p in OutputParagraphs)
            //    {
            //        string text = p.Text.Replace("\n", "<br />");
            //        XElement lines = new XElement("lines");
            //        lines.SetValue(lines);
                    
            //        lyrics.Add(new XElement("verse", 
            //            new XAttribute("name", p.ID),
            //            lines
            //            ));
            //    }

            //    // Create the root xml element and populate its fields
            //    XElement xml = new XElement("song", 
            //    //new XAttribute("xmlns", "http://openlyrics.info/namespace/2009/song"), 
            //    new XAttribute("version", "0.8"),
            //    new XAttribute("createdIn", "OpenLP 2.4"),
            //    new XAttribute("modifiedIn", "OpenLP 2.4"),
            //    new XAttribute("modifiedDate", "2016-01-01T12:00:00"), //TODO:use real date

            //    new XElement("properties",
            //        new XElement("titles",
            //            new XElement("title"), Title
            //        ),
            //        new XElement("copyright", Copyright),
            //        new XElement("verseOrder", VerseOrder.Trim()),
            //        new XElement("ccliNo", ID),
            //        authors
            //        ),

            //    lyrics
            //    );

            //    return xml;
            //}

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

                    w.WriteStartElement("ccliNo");
                    w.WriteString(ID);
                    w.WriteEndElement();

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

                    //w.WriteRaw("\r\n</song>");
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
