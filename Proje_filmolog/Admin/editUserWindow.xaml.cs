using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;

namespace Proje_filmolog.Admin
{
    /// <summary>
    /// editUserWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class editUserWindow : Window
    {
        public editUserWindow()
        {
            InitializeComponent();
        }
        filmAddComponent filmAddComponent = new filmAddComponent();
        static private bool activite;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            filmAddComponent.AddUsersListViev_inDB(userListView);
        }
        private void userListView_doubleClik(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            filmAddComponent.clearListView(remark_ListView);
            activite=filmAddComponent.showUser(userListView, lbl_realName, lbl_UserName, lbl_TelNo, lbl_active);
            filmAddComponent.showRemark(remark_ListView, lbl_UserName.Content.ToString());
            if (activite)
                btn_eject.Content = "BANLA";
            else
                btn_eject.Content = "BANI KALDIR";
        }
        private void tb_searchUser_textChange(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
                filmAddComponent.clearListView(userListView);
            if (tb_searchUser.Text.Equals(null) || tb_searchUser.Text.Equals(""))
                filmAddComponent.AddUsersListViev_inDB(userListView);
            else
                filmAddComponent.findUser_inDB(userListView, tb_searchUser.Text);
        }
        private void btn_eject_Click(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection("data source = filmolog.db; Version=3;"))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand cmd_update = new SQLiteCommand("update Users set isActive=@actv where userName=@uname", connection))
                    {
                        try
                        {
                            cmd_update.Parameters.AddWithValue("@uname", lbl_UserName.Content.ToString());
                            if (activite)
                            {
                                cmd_update.Parameters.AddWithValue("@actv", false);
                                activite = false;
                            }
                            else
                            {
                                cmd_update.Parameters.AddWithValue("@actv", true);
                                activite = true;
                            }
                            cmd_update.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Hata No:16\n" + ex);
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show("Hata No:17\n" + ex.Message);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            activite=filmAddComponent.showUser(userListView, lbl_realName, lbl_UserName, lbl_TelNo, lbl_active);
            if (activite)
                btn_eject.Content = "BANLA";
            else
                btn_eject.Content = "BANI KALDIR";
        }
    }
}
