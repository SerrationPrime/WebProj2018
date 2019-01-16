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
        static string UserLoc = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings.Get("UserStorageName");
        static string DriverLoc = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings.Get("DriverStorageName");
        static string AdminLoc = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings.Get("AdminStorageName");
        static string CarLoc = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings.Get("CarStorageName");
        static string RideLoc = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings.Get("RideStorageName");

        static public string Upisi(Korisnik korisnik)
        {

            if (Postoji("Username", korisnik.Username, UserLoc) || Postoji("Username", korisnik.Username, DriverLoc) || Postoji("Username", korisnik.Username, AdminLoc))
            {
                LogUpisi("Pokusan upis korisnika, ali korisnicko ime " + korisnik.Username + " vec postoji.");
                return "postoji";
            }

            string appropriateFile = "";
            switch (korisnik.Uloga)
            {
                case ("Korisnik"):
                    appropriateFile = UserLoc;
                    break;
                case ("Vozac"):
                    appropriateFile = DriverLoc;
                    break;
                //case ("Dispecer"):
                //appropriateFile = AdminLoc;
                //break;
                default:
                    appropriateFile = "";
                    break;
            }

            try
            {
                Dictionary<string, Korisnik> korisnici;
                if (!File.Exists(appropriateFile)) korisnici = new Dictionary<string, Korisnik>();
                else
                {
                    korisnici = JsonConvert.DeserializeObject<Dictionary<string, Korisnik>>(File.ReadAllText(appropriateFile));
                }
                korisnici.Add(korisnik.Username, korisnik);

                File.WriteAllText(appropriateFile, JsonConvert.SerializeObject(korisnici));
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
                Dictionary<DateTime, Voznja> voznje;
                if (!File.Exists(fileLoc)) voznje = new Dictionary<DateTime, Voznja>();
                else
                {
                    voznje = JsonConvert.DeserializeObject<Dictionary<DateTime, Voznja>>(File.ReadAllText(fileLoc));
                }
                voznje.Add(voznja.Vreme, voznja);

                File.WriteAllText(fileLoc, JsonConvert.SerializeObject(voznje));
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
            CookiePomoc retVal = new CookiePomoc();
            retVal.Uloga = "Nepoznat";
            retVal.Username = username;

            retVal = ProveriFajl(retVal, username, password, UserLoc);
            if (retVal.Uloga == "Nepoznat")
            {
                retVal = ProveriFajl(retVal, username, password, AdminLoc);
                if (retVal.Uloga == "Nepoznat")
                {
                    retVal = ProveriFajl(retVal, username, password, DriverLoc);
                }
            }

            return retVal;
        }

        static CookiePomoc ProveriFajl(CookiePomoc retVal, string username, string password, string fajl)
        {
            if (!File.Exists(fajl))
                return retVal;
            Korisnik trenutniKorisnik;

            Dictionary<string, Korisnik> currDict = new Dictionary<string, Korisnik>();

            currDict = JsonConvert.DeserializeObject<Dictionary<string, Korisnik>>(File.ReadAllText(fajl));
            if (currDict.TryGetValue(username, out trenutniKorisnik))
            {
                if (trenutniKorisnik.Password == password)
                {
                    if (trenutniKorisnik.Blokiran)
                    {
                        retVal.Uloga = "Blokiran";
                    }
                    else
                    {
                        retVal.Uloga = trenutniKorisnik.Uloga;
                    }
                }
            }
            return retVal;
        }

        public static List<VozacSkracena> PronadjiNajblizeVozace(double lat, double lng)
        {
            var retList = new List<VozacSkracena>();
            if (!File.Exists(DriverLoc))
                return retList;

            var vozDict = JsonConvert.DeserializeObject<Dictionary<string, Vozac>>(File.ReadAllText(DriverLoc));

            var razdaljine = new List<double>();

            foreach (var obj in vozDict)
            {

                double razdaljina = calcDist(obj.Value.Lokacija.Latitude, obj.Value.Lokacija.Longitude, lat, lng);
                if (!obj.Value.Blokiran && obj.Value.Status == "Slobodan")
                {
                    var vozilo = OcitajVozilo(obj.Value.Username);
                    if (vozilo != null)
                        retList.Add(new VozacSkracena(obj.Value.Username, razdaljina, vozilo.Tip));
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

        public static Automobil OcitajVozilo(string username)
        {
            var voziloDict = new Dictionary<string, Automobil>();
            Automobil retVal = null;

            if (File.Exists(RideLoc))
            {
                voziloDict = JsonConvert.DeserializeObject<Dictionary<string, Automobil>>(File.ReadAllText(CarLoc));
                voziloDict.TryGetValue(username, out retVal);
            }
            return retVal;
        }

        public static bool BlokDeblok(string username)
        {
            var dict = new Dictionary<string, Korisnik>();
            if (Postoji("Username", username, UserLoc))
            {
                dict = JsonConvert.DeserializeObject<Dictionary<string, Korisnik>>(File.ReadAllText(UserLoc));
            }
            else if (Postoji("Username", username, DriverLoc))
            {
                dict = JsonConvert.DeserializeObject<Dictionary<string, Korisnik>>(File.ReadAllText(DriverLoc));
            }
            //Administratori ne bi trebali sami da sebe blokiraju
            else
            {
                return false;
            }
            dict[username].Blokiran = !dict[username].Blokiran;
            return true;
        }

        public static List<Voznja> PokupiVoznje(string tipKorisnika, string username)
        {
            var retList = new List<Voznja>();

            var fullDict = JsonConvert.DeserializeObject<Dictionary<DateTime, Voznja>>(File.ReadAllText(RideLoc));

            if (tipKorisnika == "Dispecer")
            {
                foreach (var obj in fullDict)
                {
                    retList.Add(obj.Value);
                }
            }
            else if (tipKorisnika == "Vozac")
            {
                foreach (var obj in fullDict)
                {
                    if (String.IsNullOrEmpty(obj.Value.VozacUsername) || obj.Value.VozacUsername == username)
                    {
                        retList.Add(obj.Value);
                    }
                }
            }
            else
            {
                foreach (var obj in fullDict)
                {
                    if (obj.Value.MusterijaUsername == username)
                    {
                        retList.Add(obj.Value);
                    }
                }
            }
            return retList;
        }

        public static Vozac PronadjiVozaca(string username)
        {
            if (!File.Exists(DriverLoc))
                return null;

            var vozDict = JsonConvert.DeserializeObject<Dictionary<string, Vozac>>(File.ReadAllText(DriverLoc));

            Vozac retVal;
            if (!vozDict.TryGetValue(username, out retVal))
                return null;
            else return retVal;
        }

        public static bool ObnoviLokaciju(string username, double lat, double lng)
        {
            if (!File.Exists(DriverLoc))
                return false;

            var vozDict = JsonConvert.DeserializeObject<Dictionary<string, Vozac>>(File.ReadAllText(DriverLoc));


            if (!vozDict.TryGetValue(username, out Vozac temp))
                return false;
            else
            {
                vozDict[username].Lokacija.Latitude = lat;
                vozDict[username].Lokacija.Longitude = lng;
                File.WriteAllText(DriverLoc, JsonConvert.SerializeObject(vozDict));
                return true;
            }
        }

        public static bool NovoStanjeVoznje(StatusVoznje stanje, DateTime datum, string username)
        {
            if (!File.Exists(RideLoc))
                return false;

            var vozDict = JsonConvert.DeserializeObject<Dictionary<DateTime, Voznja>>(File.ReadAllText(RideLoc));


            if (!vozDict.TryGetValue(datum, out Voznja temp))
                return false;
            else
            {
                vozDict[datum].Status = stanje;
                File.WriteAllText(RideLoc, JsonConvert.SerializeObject(vozDict));
            }
            if (stanje == StatusVoznje.Obradjena || stanje == StatusVoznje.Neuspesna || stanje == StatusVoznje.Uspesna)
            {
                var vozaci = JsonConvert.DeserializeObject<Dictionary<string, Vozac>>(File.ReadAllText(DriverLoc));

                if (!vozaci.TryGetValue(username, out Vozac temp2))
                    return false;
                else
                {
                    if (stanje == StatusVoznje.Obradjena)
                        vozaci[username].Status = "Zauzet";
                    else
                        vozaci[username].Status = "Slobodan";
                    File.WriteAllText(DriverLoc, JsonConvert.SerializeObject(vozDict));
                }
            }
            return true;
        }

        public static string UpisiKomentar(Voznja voznja)
        {
            try
            {
                Dictionary<DateTime, Voznja> voznje;
                if (!File.Exists(RideLoc)) voznje = new Dictionary<DateTime, Voznja>();
                else
                {
                    voznje = JsonConvert.DeserializeObject<Dictionary<DateTime, Voznja>>(File.ReadAllText(RideLoc));
                }
                voznje.Remove(voznja.Vreme);
                voznje.Add(voznja.Vreme, voznja);

                File.WriteAllText(RideLoc, JsonConvert.SerializeObject(voznje));
            }
            catch (Exception e)
            {
                LogUpisi("Neuspesan upis, greska: " + e.Message);
                return e.Message;
            }

            LogUpisi("Uspesan upis voznje.");
            return "ok";
        }
    }
}