using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using Proje_filmolog.Other;
using Proje_filmolog.Admin;
using Proje_filmolog.Users;

namespace Proje_filmolog
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private SQLiteConnection connection = new SQLiteConnection("data source = filmolog.db; Version=3;");
        private void Click_btn_singin(object sender, RoutedEventArgs e)
        {
            string user=tb_userName.Text;
            string password=tb_userPassword.Password;
            Boolean isAdmin = false;
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SQLiteCommand cmd_findUser = new SQLiteCommand("select * from Users where userName = @uname", connection);
                cmd_findUser.Parameters.AddWithValue("@uname", user);
                SQLiteDataReader reader = cmd_findUser.ExecuteReader();
                if (reader.Read())
                {
                    if (password.Equals(reader["password"].ToString()))
                    {
                        Who_Am_I.active_User=user;
                        isAdmin =Convert.ToBoolean(reader["isAdmin"]);
                        reader.Close();
                        if (isAdmin)
                        {
                            AdminWindow window = new AdminWindow();
                            window.Show();
                        }
                        else
                        {
                            UsersWindow window = new UsersWindow();
                            window.Show();
                        }
                    }
                    else
                        MessageBox.Show("Şifre Yanlış");

                }
                else
                    MessageBox.Show("Kullanıcı Bulunamıyor!!\nKullanıcı Adını kontrol Edip Tekrar Deneyin..");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata No: 003\nHatan mesajı:" + ex.ToString());
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            tb_userName.Clear();
            tb_userPassword.Clear();
        }
        private void Click_btn_singup(object sender, RoutedEventArgs e)
        {
            if (grid_MainWindow_mainGrid.Children.Count > 0)
            {
                grid_MainWindow_mainGrid.Children.Clear();
                grid_MainWindow_mainGrid.Children.Add(new SİngUpUC());
            }
            else
            {
                grid_MainWindow_mainGrid.Children.Add(new SİngUpUC());
            }
        }
        private void Click_btn_cancel(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Click_btn_aboutUs(object sender, RoutedEventArgs e)
        {

            if (grid_MainWindow_mainGrid.Children.Count > 0)
            {
                grid_MainWindow_mainGrid.Children.Clear();
                grid_MainWindow_mainGrid.Children.Add(new aboutUs());
            }
            else
            {
                grid_MainWindow_mainGrid.Children.Add(new aboutUs());
            }
        }
    }
}
