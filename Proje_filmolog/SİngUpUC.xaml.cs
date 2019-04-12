using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using System.Data;
using System;

namespace Proje_filmolog
{
    /// <summary>
    /// SİngUpUC.xaml etkileşim mantığı
    /// </summary>
    public partial class SİngUpUC : UserControl
    {
        public SİngUpUC()
        {
            InitializeComponent();
        }
        private SQLiteConnection connection = new SQLiteConnection("data source = filmolog.db; Version=3;");

        private void Click_btn_singUp(object sender, RoutedEventArgs e)
        {
            string realName = tb_realName.Text;
            string userName = tb_userName.Text;
            string password = tb_password.Password;
            string adminKey = tb_adminKey.Text;
            long telno = Convert.ToInt64(tb_telNo.Text);
            Boolean isAdmin = false; if (adminKey.Equals("adminkey")) isAdmin = true;

            if (tb_password.Password != "" && tb_realName.Text.Trim() != "" && tb_userName.Text.Trim() != "" && tb_telNo.Text.Trim() != "" && password.Equals(tb_passwordAgain.Password))
            {
                using (SQLiteCommand cmd_isJoined = new SQLiteCommand("select *from Users where userName=@uName", connection))
                {
                    try
                    {
                        connection.Open();
                        cmd_isJoined.Parameters.AddWithValue("@uName", userName);
                        using (SQLiteDataReader reader = cmd_isJoined.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                try
                                {
                                    SQLiteCommand cmd_insert = new SQLiteCommand("insert into Users (userName,password,realName,telNo,isAdmin,isActive) values (@v_userName,@v_password,@v_realName,@v_telNo,@v_isAdmin,@v_isActive)", connection);
                                    cmd_insert.Parameters.AddWithValue("@v_userName", userName);
                                    cmd_insert.Parameters.AddWithValue("@v_password", password);
                                    cmd_insert.Parameters.AddWithValue("@v_realName", realName);
                                    cmd_insert.Parameters.AddWithValue("@v_telNo", telno);
                                    cmd_insert.Parameters.AddWithValue("@v_isAdmin", isAdmin);
                                    cmd_insert.Parameters.AddWithValue("@v_isActive", true);
                                    cmd_insert.ExecuteNonQuery();
                                    reader.Close();
                                    tb_realName.Clear(); tb_userName.Clear(); tb_password.Clear(); tb_passwordAgain.Clear(); tb_adminKey.Clear(); tb_telNo.Clear();
                                }
                                catch (Exception ex_)
                                {
                                    MessageBox.Show("Kayıtta hata..Hata No:001\nHata mesajı:\n" + ex_.ToString());
                                }
                            }
                            else
                            {
                                MessageBox.Show("Kullanıcı Kaydı Mevcut ya da Kullanıcı Adı önceden Alınmış Olabilir.\nKullanıcı Adını Değiştirip Tekrar Deneyin.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata No:002\nHata mesajı:\n" + ex.ToString());
                    }
                }
            }
            else
            {
                if (!password.Equals(tb_passwordAgain.Password))
                    MessageBox.Show("Şifreler Eşleşmiyor!!");
                else
                    MessageBox.Show("Lütfen Eksik Kısımları Doldurun!!");
            }
        }
    }
}
