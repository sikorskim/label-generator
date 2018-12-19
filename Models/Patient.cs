using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace label_generator
{
    public class Patient
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Pesel { get; set; }
        public string BookNumber { get; set; }

        public string FirstAndLastName {get {return FirstName + " "+LastName;}}
        public string HospitalName {get {return "Szpital w Śremie";}}
        public string BookNumberString {get {return "KG "+BookNumber;}}
        public string PeselString {get {return "PESEL: "+Pesel;}}


        public Patient()
        {}

        public Patient(string firstName, string lastName, string pesel, string bookNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            Pesel = pesel;
            BookNumber = bookNumber;
        }

         public string generate()
        {
            string path = "templates/labels_35x21.xml";
            XDocument doc = XDocument.Load(path);
            XElement root = doc.Element("Template");

            string header = root.Element("Header").Value;
            
            string labels = root.Element("Labels").Value;
            labels = string.Format(labels, HospitalName, BookNumberString, FirstAndLastName, PeselString);
                    
            string output = header + labels;

            output = output.Replace("~^~^", "{{");
            output = output.Replace("^~^~", "}}");
            output = output.Replace("~^", "{");
            output = output.Replace("^~", "}");


            string time = DateTime.Now.ToFileTime().ToString();

            string outputFile = getHash(FirstAndLastName + Pesel + time);
            File.WriteAllText("tmp/" + outputFile + ".tex", output);

            Process process = new Process();
            process.StartInfo.WorkingDirectory = "tmp";
            process.StartInfo.FileName = "pdflatex";
            process.StartInfo.Arguments = outputFile + ".tex";
            process.Start();

            // wait to avoid FileNotFoundException
            //Task.Delay(5000);
            process.Dispose();
            return outputFile + ".pdf";
        }

        
        public static string getHash(string input)
        {
            string hashAlgo = "SHA256";
            HashAlgorithm algo = HashAlgorithm.Create(hashAlgo);
            byte[] hashBytes = algo.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("X2"));
            }
            string computedHash = sb.ToString();

            return computedHash;
        }

        public string getDownloadFilename()
        {
            return "naklejki_" + LastName+"_"+DateTime.Now.ToShortDateString();
        }
 
    }
}