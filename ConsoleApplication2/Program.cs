using System;
using System.Data.SQLite;

namespace DBSM // komentorivi-pohjainen ohjelma tietokannan hallintaan
{
    class Program
    {
        static void Main() // ohjelman alkuruutu ja päävalikko
        {
            
            Console.WriteLine("DBMS");
            Console.WriteLine("****");
            string valinta1str = getInput("(1) Kirjaudu sisään\n(0) Lopeta");
            int valinta1 = int.Parse(valinta1str);
            if (valinta1 == 1)
            {
                login();
                dbMngment();
            }
            else
            {
                Environment.Exit(0);
            }
            
         }
        static void dbMngment() // tietokannan hakemisvalikko, jos tietokanta ei ole tallennettuna ohjelman kansioon, ei voida edetä. Vaatii siis valmiin tietokannan.
        {                       // Kehitys: Tietokannan haku palvelimelta & uuden tyhjän tietokannan luominen -> valikko tarvitsee taulun luonti mahdollisuuden.
            string valinta2str;
            string path2db;
            DBPath path;
            SQLiteConnection dbconnection;
                       
            do
            {
                valinta2str = getInput("(1) Hallitse tietokanta\n(0)Kirjaudu ulos");
                if (!string.IsNullOrEmpty(valinta2str))
                {
                    int valinta2 = int.Parse(valinta2str);
                    if (valinta2 == 1)
                    {
                        Console.WriteLine("Syötä tietokannan nimi: ");
                        path = new DBPath(Console.ReadLine());
                        path2db = path.getPath();
                        if (System.IO.File.Exists(path2db))
                        { 
                            dbconnection = new SQLiteConnection(@"Data Source=" + path2db + "; Version =3");
                            dbconnection.Open();
                            testDB(dbconnection);
                            menu(path2db, dbconnection);
                        }
                        else
                        {
                            Console.WriteLine("Tietokantaa ei löydetty, varmista että tietokanta on ohjelman kanssa samassa kansiossa tai varmista että kirjoitit oikein");
                            Console.WriteLine("Paina nappia yrittääksesi uudelleen");
                            Console.ReadLine();
                            dbMngment();
                        }
                    }
                    else
                    {
                        Main();
                    }
                }
                else
                {
                    Console.WriteLine("Ei syöttöä, yritä uudelleen\n");
                }
            } while (string.IsNullOrEmpty(valinta2str));
           
        }
        static void testDB(SQLiteConnection dbconnection) // testaa että tietokanta on avattu
        {
            if (dbconnection.State == System.Data.ConnectionState.Open)
            {
                Console.WriteLine("Tietokanta auki\n");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Tietokanta kiinni\n");
                Console.ReadLine();
            }
        }
        static void menu(string path, SQLiteConnection dbconnection) // toimintovalikko
        {
            string valinta3str = getInput("Valikko\n***********\n(1)Hae kaikki pelaajat\n(2)Haku 2\n(3)Haku 3\n(4)Haku 4\n(5)Vapaa select-haku\n(6)INSERT/ALTER\n(0)Lopeta");
            int valinta3 = int.Parse(valinta3str);
            if (valinta3 == 1) // hakee kaikki tiedot pelaaja-taulusta
            {
                string query2 = "select * from Pelaaja";
                SQLiteCommand komento = new SQLiteCommand(query2, dbconnection);
                SQLiteDataReader reader = komento.ExecuteReader();
                while (reader.Read())
                    Console.WriteLine(@"ID: " + reader["PelaajaID"] + "\tJoukkueID: " + reader["JoukkueID"] + "\tNimi: " + reader["Nimi"] + "\tPelinumero: " + reader["Pelinumero"] + "\tPisteet: " + reader["Pisteet"] + "\tOsoite: " + reader["Osoite"] + "\tPuhelinnumero: " + reader["Puhelinnumero"]);
                Console.WriteLine("Paina nappia palataksesi valikkoon");
                Console.ReadLine();
                menu(path, dbconnection);
            }
            else if (valinta3 == 2) // kesken, erilainen esimerkkihaku
            {
            }
            else if (valinta3 == 3) // kesken, placeholder, esimerkkihaku, joka käyttää where, group by, having, yms. joitakin näistä.
            {
            }
            else if (valinta3 == 4) // kesken, placeholder, JOIN hakuesimerkki
            {
            }
            else if (valinta3 == 5) // voidaan tehdä vapaavalintaisia SELECT-hakuja valitusta tietokannasta, muita kuin select-hakuja ei kuitenkaan voi ajaa tämän kautta
            {
                string query2 = getInput("SELECT-haku");
                if (query2.StartsWith("SELECT"))
                {
                try
                {
                        SQLiteCommand komento = new SQLiteCommand(query2, dbconnection);
                        SQLiteDataReader reader = komento.ExecuteReader();
                        while (reader.Read())
                        {                                       // tulostaa sarakkeiden nimet ja arvot \n kun rivi vaihtuu
                            int sarakelkm = reader.FieldCount;
                            for (int i = 0; i < sarakelkm; i++)
                            {
                                if (i == sarakelkm - 1)
                                {
                                    Console.Write(reader.GetName(i) + ": " + reader[sarakelkm - 1] + "\n"); 
                                }
                                else
                                {
                                    Console.Write(reader.GetName(i) + ": " + reader[i] + ", ");
                                }
                            }
                        }
                    }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Virhe komennossa tai taulua/attribuuttia ei ole, tarkista syntaksi"); 
                }
                }
               
            else
            {
                Console.WriteLine("Hei, käytäthän vain SELECT-hakuja!\n");
                Console.WriteLine("Paina nappia palataksesi valikkoon\n");
                Console.ReadLine();
                menu(path, dbconnection);
            }
                Console.WriteLine("Paina nappia palataksesi valikkoon\n");
                Console.ReadLine();
                menu(path, dbconnection);      
               
            }
            else if (valinta3 == 6) // kesken, insert/alter/delete
            {

            }
            else // mikäli syöttö muu kuin 1-6 sulkee tietokannan ja palaa tietokannan valinta valikkoon
            {
                dbconnection.Close();
                dbMngment();
            } // lisää toiminnallisuuksia, eri käyttäjäryhmille erilainen näkymä (oikeudet)
           }
        static void login() // aliohjelma kirjautumista varten, hakee valmiista tietokannasta login.db ja mikäli käyttäjätunnus ja salasana ovat pari antaa jatkaa ohjelmassa. Lopuksi sulkee tietokannan.
        {
            string user = getInput("Käyttäjänimi");
            string salasana = getInput("Salasana");
            using (SQLiteConnection login = new SQLiteConnection(@"Data Source=login.db; Version =3")) // samaan kantaan vai erillinen kanta
            {
                SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM Rekisteri WHERE username=@username AND password=@password", login);
                cmd.Parameters.AddWithValue("@username", user);
                cmd.Parameters.AddWithValue("@password", salasana);
                login.Open();
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                if (result == 1)
                {
                    Console.WriteLine("Onnistui");
                    Console.ReadLine();
                    login.Close();
                }
                else
                {
                    Console.WriteLine("Käyttäjätunnus ja salasana eivät täsmää");
                    Console.WriteLine("Yritä uudelleen");
                    Console.ReadLine();
                    login.Close();
                    Main();
                }

            }
        }
        static SQLiteConnection dbOpen(string path) // voidaan käyttää avaamaan käyttäjän syöttämä tietokanta (pitää implementoida)
        {
            SQLiteConnection dbconnection;
            dbconnection = new SQLiteConnection(@"Data Source=" + path + "; Version =3");
            dbconnection.Open();
            testDB(dbconnection);
            return dbconnection;

        }
        private static string getInput(string Prompt) // hakee käyttäjän syöttämän arvon, mikäli syöttö on tyhjä, palauttaa valikkoon ja kehoittaa syöttämään uudestaan. 
        {                                             // Ei voida edetä ilman käyttäjän syöttöä.
            string Result;
            do
            {
                Console.WriteLine(Prompt + ": ");
                Result = Console.ReadLine();
                if (string.IsNullOrEmpty(Result))
                {
                    Console.WriteLine("Syöttö tyhjä, yritä uudelleen\n");
                }
            } while (string.IsNullOrEmpty(Result));
            return Result;

        }
        

        }
    class DBPath // luokka, jota voidaan käyttää tietokannan sijainnin vaihtamiseen tarvittaessa (ei implementoitu kunnolla)
    {

        private string path;

        public DBPath(string path)
        {
            this.path = path;
        }

        public string getPath()
        {
            return path;
        }
        public void setDBPath(string newPath)
        {
            path = newPath;
        }
    }
}

