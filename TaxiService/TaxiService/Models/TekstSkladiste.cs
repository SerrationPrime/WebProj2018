using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Configuration;

namespace TaxiService.Models
{
    public static class TekstSkladiste
    {
        static JsonSerializer Serializer = new JsonSerializer();

        static public bool Upisi(Korisnik korisnik)
        {
            string fileLoc = ConfigurationManager.AppSettings.Get("UserStoragePath");
            if (Postoji("username", korisnik.Username, fileLoc))
            {
                LogUpisi("Pokusan upis korisnika, ali korisnicko ime " + korisnik.Username + " vec postoji.");
                return false;
            }

            try
            {
                using (StreamWriter file = File.AppendText(fileLoc))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, korisnik);
                }
            }
            catch (Exception e)
            {
                LogUpisi("Neuspesan upis, greska: " + e.Message);
                return false;
            }

            LogUpisi("Uspesan upis korisnika " + korisnik.Username + ".");
            return true;
        }

        static bool Postoji(string token, string vrednost, string file)
        {

            if (File.Exists(file)) {
                using (JsonTextReader reader = new JsonTextReader(File.OpenText(file)))
                {
                    reader.Read();
                    if (reader.TokenType.ToString() == token && (string)reader.Value == vrednost)
                    {
                        return true;
                    }
                }
            }
            return false;

            //return File.ReadAllLines(file).Contains("\"" + atribut + "\"" + ": " + vrednost);
        }

        static void LogUpisi(string poruka)
        {
            using (TextWriter file = File.AppendText(ConfigurationManager.AppSettings.Get("LogStoragePath")))
            {

                file.WriteLine("poruka");
            }
        }
    }
}