using System;
using System.Net;
using System.Text;
using System.Windows;
using HtmlAgilityPack;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;

namespace Proje_filmolog.Admin
{
    class filmAddComponent
    {
        public struct PropertyFilms//siteden Çekilen filmlerin özelliklerini listeye eklemek için kullanılacak yapı.
        {
            public string _name;
            public string _director;
            public string _starring;
            public string _rate;
            public string _info;
            public string _rank;
        }
        PropertyFilms propertyFilms = new PropertyFilms();
        public static List<PropertyFilms> ListOfFilm = new List<PropertyFilms>();//siteden çekilen filmleri tutacak liste.
        private const string MainURL = "https://www.sinemalar.com/en-iyi-filmler";//site adresi.
        private const string MainPATH = "//*[@id='container']/div[2]/div[3]/div[2]/article[";
        private const string NAME = "]/div/div[4]/h3/a";//film adı.
        private const string DIRECTOR = "]/div/div[4]/p[1]/a";//yönetmen.
        private const string STARRING = "]/div/div[4]/p[2]";//oyuncu.
        private const string RATE = "]/div/div[2]/div";//puan.
        private const string INFO = "]/div/div[4]/p[3]/text()";//açıklama.
        private const string RANK = "]/div/div[4]/div";//sıralama.
        private const int COLUMN_COUNTER = 6;//siteden çekilen filmlerin özelliklerinin sırasının indeksi için sayaç.
        public static int Page;//siteden çekilecek sayfanın indeksi.
        private string html;//sitenin html kodunu tutacak string.
        private short Idx;
        private string[] listItems;//filmlerin çekme sırasında özelliklerini tutacak string.
        private HtmlDocument doc;//sitenin html kodu.
        //verilen path e göre film özelliğini listItems e atayan method.
        private string connectStrg = "data source = filmolog.db; Version=3;";
        //verilen path e göre web den ilgili veriyi çekip listitems e atayan metod.
        private void GetValueWithPath(string xpath)
        {
            try
            {
                if (Idx == 2)
                {
                    try
                    {
                        string[] pureStr = doc.DocumentNode.SelectSingleNode(xpath).InnerText.Split(',');
                        listItems[Idx] = pureStr[1].TrimStart();
                    }
                    catch (IndexOutOfRangeException)
                    {
                        listItems[Idx] = " - ";
                    }
                }
                else
                {
                    listItems[Idx] = doc.DocumentNode.SelectSingleNode(xpath).InnerText;
                }
                Idx++;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Hata No:01\n"+"NullReferenceException");
            }
        }
        //web den çekilen filmleri listview e ekleyen metod. 
        public void AddfilmsListView(ListView FilmListView)
        {
            if (Page > -1)
            {
                WebClient client = new WebClient
                {
                    Encoding = Encoding.UTF8
                };
                try
                {
                    html = client.DownloadString(MainURL + Page);
                }
                catch (WebException ex)
                {
                    MessageBox.Show("Hata No:02\n" + "İnternet bağlantısını kontrol ediniz !" + ex.Message);
                    return;
                }
                doc = new HtmlDocument();
                doc.LoadHtml(html);
                try
                {
                    for (int i = 1; i < 26; i++)
                    {
                        listItems = new string[COLUMN_COUNTER];
                        Idx = 0;

                        GetValueWithPath(MainPATH + i + NAME);
                        GetValueWithPath(MainPATH + i + DIRECTOR);
                        GetValueWithPath(MainPATH + i + STARRING);
                        GetValueWithPath(MainPATH + i + RATE);
                        GetValueWithPath(MainPATH + i + INFO);
                        GetValueWithPath(MainPATH + i + RANK);
                        FilmListView.Items.Add(new { rank = listItems[5], name = listItems[0], rate = listItems[3] });//film adlarının ListView e eklendiği kısım.

                        if (!listItems[0].Equals(null)) propertyFilms._name = listItems[0]; else continue;
                        if (!listItems[1].Equals(null)) propertyFilms._director = listItems[1]; else propertyFilms._director = "-";
                        if (!listItems[2].Equals(null)) propertyFilms._starring = listItems[2]; else propertyFilms._starring = "-";
                        if (!listItems[3].Equals(null)) propertyFilms._rate = listItems[3]; else propertyFilms._rate = "-";
                        if (!listItems[4].Equals(null)) propertyFilms._info = listItems[4]; else propertyFilms._info = "-";
                        if (!listItems[5].Equals(null)) propertyFilms._rank = listItems[5]; else propertyFilms._rank = "-";

                        ListOfFilm.Add(propertyFilms);
                    }
                }
                catch (UriFormatException)
                {
                    MessageBox.Show("Hata No:03\n" + "UriFormatException");
                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show("Hata No:04\n" + "ArgumentNullException");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata No:05\n" + ex.Message);
                }
            }
        }
        //database deki filmleri listview e ekleyen metod.
        public void AddfilmsListView(ListView FilmListView, int i)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectStrg))
            {
                using (SQLiteCommand cmd_select = new SQLiteCommand("select rank,name from film", connection))
                {
                    try
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        using (SQLiteDataReader reader = cmd_select.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                FilmListView.Items.Add(new { rank = reader["rank"], name = reader["name"] });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata No:06\n" + ex.Message);
                    }
                }
            }
        }
        //listview temizleyen metod.
        public void clearListView(ListView listView)
        {
            listView.Items.Clear();
        }
        //web den çekilen filmlerden isteneni database ye ekleyen metod.
        public void addFilmDB(List<PropertyFilms> kayitListesi, int i)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectStrg))
                {
                    connection.Open();
                    using (SQLiteCommand cmd_insert = new SQLiteCommand("insert into film(name,dir,staring,rate,info,rank) values(@name,@dir,@staring,@rate,@info,@rank)", connection))
                    {
                        using (SQLiteCommand cmd_varmi = new SQLiteCommand("select *from film where name=@name", connection))
                        {
                            cmd_varmi.Parameters.AddWithValue("@name", kayitListesi[i]._name);
                            using (SQLiteDataReader reader = cmd_varmi.ExecuteReader())
                            {
                                if (!reader.Read())
                                {
                                    try
                                    {

                                        cmd_insert.Parameters.AddWithValue("@name", kayitListesi[i]._name);
                                        cmd_insert.Parameters.AddWithValue("@dir", kayitListesi[i]._director);
                                        cmd_insert.Parameters.AddWithValue("@staring", kayitListesi[i]._starring);
                                        cmd_insert.Parameters.AddWithValue("@rate", kayitListesi[i]._rate);
                                        cmd_insert.Parameters.AddWithValue("@info", kayitListesi[i]._info);
                                        cmd_insert.Parameters.AddWithValue("@rank", kayitListesi[i]._rank);
                                        int ex = cmd_insert.ExecuteNonQuery();
                                        if (ex > 0)
                                        {
                                            MessageBox.Show("kayıt tamam..");
                                        }
                                        else
                                        {
                                            MessageBox.Show("kayıt başarısız..");
                                        }
                                    }
                                    catch (SQLiteException)
                                    {
                                        MessageBox.Show("Hata No:07\n" + "Filmi kaydedemedik Kusura Bakmayın!!\nTekrar Deneyebilirsiniz İsterseniz");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Fİlm Zaten Mevcut", "Dikkat", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata No:08\n" + ex.ToString());
            }
        }
        //database den film silen metod.
        public void deleteFilmDB(ListView listView)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectStrg))
                {
                    dynamic selectedItem = listView.SelectedItems[0];
                    int _Rank = (int)selectedItem.rank;
                    connection.Open();
                    using (SQLiteCommand cmd_delete = new SQLiteCommand("delete from film where rank=@_rank", connection))
                    {
                        cmd_delete.Parameters.AddWithValue("@_rank", _Rank);
                        cmd_delete.ExecuteNonQuery();
                        MessageBox.Show("Film Listeden Kaldırıldı..");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata No: 09\n" + ex.ToString());
            }

        }
        //database den film arayan metod.
        public void findFilm(string film_find, ListView listView)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectStrg))
            {
                try
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    using (SQLiteCommand cmd_findFilm = new SQLiteCommand("select *from film where name like '%"+film_find+"%' ", connection))
                    {
                        SQLiteDataReader reader = cmd_findFilm.ExecuteReader();
                        while (reader.Read())
                        {
                            listView.Items.Add(new { rank=reader["rank"], name = reader["name"] });
                            //MessageBox.Show(reader["name"].ToString());
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata No:10\n" + ex.ToString());
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }
        //açılışta kullanıcı listview ini database den dolduran metod.
        public void AddUsersListViev_inDB(ListView listView)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectStrg))
            {
                try
                {
                    connection.Open();
                        using (SQLiteCommand cmd_selectuser = new SQLiteCommand("select *from Users  userName where isAdmin=0", connection))
                        {
                            try
                            {
                                using (SQLiteDataReader reader = cmd_selectuser.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        listView.Items.Add(new { uname = reader["userName"] });
                                    }
                                    reader.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Hata No:11\n" + ex.Message);
                            }
                        }

                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show("Hata No:12\n" + ex.Message);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
        }
        //kullanıcı arayan metod.
        public void findUser_inDB(ListView listView,string findUser)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectStrg))
            {
                using (SQLiteCommand cmd_selectuser = new SQLiteCommand("select *from Users where userName  like '%" + findUser + "%' and isAdmin=0", connection))
                {
                    try
                    {
                        connection.Open();
                        using (SQLiteDataReader reader = cmd_selectuser.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listView.Items.Add(new { uname = reader["userName"] });
                            }
                            reader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata No:13\n" + ex.ToString());
                    }
                }
            }
        }
        //kullanıcının yapyığı yorumları db den alan metod.
        public void showRemark(ListView listView, string uName)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectStrg))
                {
                    connection.Open();
                    using (SQLiteCommand cmd_selectRemark = new SQLiteCommand("select *from comment where userName=@uName", connection))
                    {
                        cmd_selectRemark.Parameters.AddWithValue("@uName", uName);
                        using (SQLiteDataReader reader = cmd_selectRemark.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string ig = "-";
                                if (Convert.ToBoolean(reader["isGood"]))
                                    ig = "+";
                                listView.Items.Add(new { usersFilm = reader["filName"], remark = reader["remark"], isgood = ig });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata No:15\n" + ex.ToString());
            }
        }
        // label lerde kullanıcı bilgilerini gösteren metod.
        public bool showUser(ListView userListView,Label lbl_realName,Label lbl_UserName,Label lbl_TelNo,Label lbl_active)
        {
            bool act=true;
            using (SQLiteConnection connection = new SQLiteConnection(connectStrg))
            {
                using (SQLiteCommand cmd_selecteduser = new SQLiteCommand("select *from Users where userName=@uName", connection))
                {
                    try
                    {
                        dynamic selectedind = userListView.SelectedItems[0];
                        string x = selectedind.uname;
                        cmd_selecteduser.Parameters.AddWithValue("@uName", x);
                        connection.Open();
                        using (SQLiteDataReader reader = cmd_selecteduser.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lbl_realName.Content = reader["realName"].ToString();
                                lbl_UserName.Content = reader["userName"].ToString();
                                lbl_TelNo.Content = reader["telNo"].ToString();
                                act = Convert.ToBoolean(reader["isActive"]);
                                lbl_active.Content = act.ToString();
                            }
                            reader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata No:14\n" + ex.ToString());
                    }
                    connection.Close();
                }
            }
            return act;
        }
    }
}