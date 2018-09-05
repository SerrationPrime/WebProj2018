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
        static string LogLoc = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings.Get("LogStorageName");
        static string AutoLoc = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings.Get("CarStorageName");

        static public string Upisi(Korisnik korisnik)
        {
            string fileLoc = AppDomain.CurrentDomain.BaseDirectory;
            fileLoc += ConfigurationManager.AppSettings.Get("UserStorageName");

            if (Postoji("Username", korisnik.Username, fileLoc))
            {
                LogUpisi("Pokusan upis korisnika, ali korisnicko ime " + korisnik.Username + " vec postoji.");
                return "postoji";
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
                return e.Message;
            }

            LogUpisi("Uspesan upis korisnika " + korisnik.Username + ".");
            return "ok";
        }

        public static string UpisiVoznju(Voznja voznja)
        {
            string fileLoc = AppDomain.CurrentDomain.BaseDirectory;
            fileLoc += ConfigurationManager.AppSettings.Get("RideStorageName");

            try
            {
                using (StreamWriter file = File.AppendText(fileLoc))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, voznja);
                }
            }
            catch (Exception e)
            {
                LogUpisi("Neuspesan upis, greska: " + e.Message);
                return e.Message;
            }

            LogUpisi("Uspesan upis voznje.");
            return "ok";
        }

        static bool Postoji(string token, string vrednost, string file)
        {

            if (File.Exists(file))
            {
                using (JsonTextReader reader = new JsonTextReader(File.OpenText(file)))
                {
                    reader.SupportMultipleContent = true;
                    while (reader.Read())
                    {
                        if (reader.Value != null && reader.Value.ToString() == token && reader.TokenType.ToString() == "PropertyName")
                        {
                            reader.Read();
                            if (reader.Value.ToString() == vrednost)
                            {
                                return true;
                            }
                            else
                            {
                                while (reader.TokenType.ToString() != "StartObject")
                                {
                                    if (!reader.Read()) break;
                                }
                            }
                        }
                    }
                }
            }
            return false;

            //return File.ReadAllLines(file).Contains("\"" + atribut + "\"" + ": " + vrednost);
        }

        public static void LogUpisi(string poruka)
        {
            using (TextWriter file = File.AppendText(LogLoc))
            {

                file.WriteLine(DateTime.Now + ": " + poruka);
            }
        }

        public static CookiePomoc Autentikacija(string username, string password)
        {
            string file = AppDomain.CurrentDomain.BaseDirectory;
            file += ConfigurationManager.AppSettings.Get("UserStorageName");

            CookiePomoc retVal = new CookiePomoc();
            retVal.Uloga = "Nepoznat";
            retVal.Username = username;

            using (JsonTextReader reader = new JsonTextReader(File.OpenText(file)))
            {
                reader.SupportMultipleContent = true;

                //Petlja 1: samo citaj
                while (reader.Read())
                {
                    if (reader.Value != null && reader.Value.ToString() == "Username" && reader.TokenType.ToString() == "PropertyName")
                    {
                        reader.Read();
                        if (reader.Value.ToString() == username)
                        //Ako je korisnicko ime dobro, citaj do lozinke
                        {
                            while (reader.Value.ToString() != "Password" || reader.TokenType.ToString() != "PropertyName")
                            {
                                reader.Read();
                            }
                            reader.Read();
                            //Ako je i lozinka dobra, pokupi ulogu
                            if (reader.Value.ToString() == password)
                            {
                                while (reader.Value.ToString() != "Uloga" || reader.TokenType.ToString() != "PropertyName")
                                {
                                    reader.Read();
                                }
                                reader.Read();
                                retVal.Uloga = reader.Value.ToString();
                                //I onda konacno proveri stanje blokranja
                                while (reader.Value == null || reader.Value.ToString() != "Blokiran" || reader.TokenType.ToString() != "PropertyName")
                                {
                                    reader.Read();
                                }
                                reader.Read();
                                retVal.Blokiran = Convert.ToBoolean(reader.Value.ToString());
                                if (retVal.Blokiran)
                                {
                                    retVal.Uloga = "Blokiran";
                                }
                                return retVal;
                            }
                            //Korisnik postoji, ali lozinka nije dobra
                            else
                            {
                                return retVal;
                            }
                        }
                        //To nije korisnik, preskoci na sledećeg
                        else
                        {
                            while (reader.TokenType.ToString() != "StartObject")
                            {
                                if (!reader.Read()) break;
                            }
                        }
                    }

                }
            }
            //Korisnik nije na evidenciji
            return retVal;
        }

        public static List<VozacSkracena> PronadjiNajblizeVozace(double lat, double lng)
        {
            var retList = new List<VozacSkracena>();

            string file = AppDomain.CurrentDomain.BaseDirectory;
            file += ConfigurationManager.AppSettings.Get("UserStorageName");

            string fileText;

            using (StreamReader reader = File.OpenText(file))
            {
                fileText = reader.ReadToEnd();
            }

            fileText=fileText.Replace("}{", "}*{");
            var fileObjects = fileText.Split('*');

            var razdaljine = new List<double>();

            foreach (var obj in fileObjects)
            {
                if (obj.Contains("Uloga\":\"Vozac"))
                {
                    Vozac vozac;
                    vozac = JsonConvert.DeserializeObject<Vozac>(obj);
                    double razdaljina = calcDist(vozac.Lokacija.Latitude, vozac.Lokacija.Longitude, lat, lng);
                    if (!vozac.Blokiran && vozac.Status == "Slobodan")
                    {
                        var vozilo=ocitajVozilo(vozac.Username);
                        retList.Add(new VozacSkracena(vozac.Username, razdaljina, vozilo.Tip));
                    }
                    
                }
            }
            List<VozacSkracena> retListSorted = retList.OrderBy(o => o.distanca).ToList();
            retListSorted.RemoveRange(5, retList.Count - 5);

            return retListSorted;
        }

        static double calcDist(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 * x2 - x1 * x1), 2) + Math.Pow((y2 * y2 - y1 * y1), 2));
        }

        public static Automobil ocitajVozilo(string username)
        {
            string fileText;

            using (StreamReader reader = File.OpenText(AutoLoc))
            {
                fileText = reader.ReadToEnd();
            }

            fileText.Replace("}{", "}" + "}\n{");
            var fileObjects = fileText.Split('\n');

            foreach (var obj in fileObjects)
            {
                var auto = JsonConvert.DeserializeObject<Automobil>(obj);
                if (auto.VozacUsername == username)
                {
                    return auto;
                }
            }
            return null;
        }
        
    }
}