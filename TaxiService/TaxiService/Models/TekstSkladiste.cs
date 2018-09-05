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

        static bool Postoji(string token, string vrednost, string file)
        {

            if (File.Exists(file)) {
                using (JsonTextReader reader = new JsonTextReader(File.OpenText(file)))
                {
                    reader.SupportMultipleContent = true;
                    while (reader.Read())
                    {
                        if (reader.Value!=null && reader.Value.ToString() == token && reader.TokenType.ToString() == "PropertyName")
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

            CookiePomoc retVal= new CookiePomoc();
            retVal.Uloga = "Nepoznat";
            retVal.Username = username;

            using (JsonTextReader reader = new JsonTextReader(File.OpenText(file)))
            {
                reader.SupportMultipleContent = true;
                
                //Petlja 1: samo citaj
                while (reader.Read())
                {
                    if (reader.Value != null && reader.Value.ToString() == "Username" && reader.TokenType.ToString()=="PropertyName")
                    {
                        reader.Read();
                        if (reader.Value.ToString() == username)
                        //Ako je korisnicko ime dobro, citaj do lozinke
                        {
                            while( reader.Value.ToString() != "Password" || reader.TokenType.ToString() != "PropertyName")
                            {
                                reader.Read();
                            }
                            reader.Read();
                            //Ako je i lozinka dobra, pokupi ulogu
                            if (reader.Value.ToString()== password)
                            {
                                while (reader.Value.ToString() != "Uloga" || reader.TokenType.ToString() != "PropertyName")
                                {
                                    reader.Read();
                                }
                                reader.Read();
                                retVal.Uloga = reader.Value.ToString();
                                //I onda konacno proveri stanje blokranja
                                while (reader.Value==null || reader.Value.ToString() != "Blokiran" || reader.TokenType.ToString() != "PropertyName")
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
    }
}