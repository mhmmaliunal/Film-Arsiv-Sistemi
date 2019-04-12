using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace Proje_filmolog.Admin
{
    /// <summary>
    /// AdminWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
        }

        filmAddComponent filmAddComponent = new filmAddComponent();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i < 99; i++)
            {
                PageCbox.Items.Add(i);
            }
            filmAddComponent.AddfilmsListView(FilmListView_inDB, 0);//film adlarının ListView e eklendiği kısım.
        }
        private void PageCbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filmAddComponent.Page = PageCbox.SelectedIndex - 1;
        }
        private void GetButton_Click(object sender, RoutedEventArgs e)
        {
            filmAddComponent.clearListView(FilmListView);
            filmAddComponent.ListOfFilm.Clear();
            filmAddComponent.AddfilmsListView(FilmListView);

        }
        private void FilmListView_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FilmListView.SelectedIndex > -1)
            {
                filmAddComponent.addFilmDB(filmAddComponent.ListOfFilm, FilmListView.SelectedIndex);
                filmAddComponent.clearListView(FilmListView_inDB);
                filmAddComponent.AddfilmsListView(FilmListView_inDB, 0);//film adlarının ListView e eklendiği kısım.
            }
            else
                MessageBox.Show("Eklenecek Filmi Seçin!!");
        }
        private void FilmListView_DoubleClick_inDB(object sender, MouseButtonEventArgs e)
        {
            if (FilmListView_inDB.SelectedIndex > -1)
            {
                filmAddComponent.deleteFilmDB(FilmListView_inDB);
                FilmListView_inDB.Items.RemoveAt(FilmListView_inDB.SelectedIndex);
            }
            else if (FilmListView_search.SelectedIndex > -1)
            {
                filmAddComponent.deleteFilmDB(FilmListView_search);
                FilmListView_search.Items.RemoveAt(FilmListView_search.SelectedIndex);
                //seçimlerin diğer listeye uygulanması için bir yötem diğerleri btn_btn_Click metodunda
                filmAddComponent.clearListView(FilmListView_inDB);
                filmAddComponent.AddfilmsListView(FilmListView_inDB, 0);
            }
            else
                MessageBox.Show("Silinecek Filmi Seçin!!");
        }
        private void btn_editUser_Click(object sender, RoutedEventArgs e)
        {
            editUserWindow win = new editUserWindow();
            win.Show();
        }
        private void btn_btn_Click(object sender, RoutedEventArgs e)
        {

            filmAddComponent.clearListView(FilmListView_inDB);
            filmAddComponent.AddfilmsListView(FilmListView_inDB, 0);
            FilmListView_search.Width = 0;
            //AdminWindow win = new AdminWindow();
            //this.Close();
            //win.Show();
        }
        private void tb_search_filmTextChange(object sender, TextChangedEventArgs e)
        {
            if (tb_searchFilm_inDB.Text.Equals(null) || tb_searchFilm_inDB.Text.Equals(""))
                FilmListView_search.Width = 0;
            else
                FilmListView_search.Width = 200;
            filmAddComponent.clearListView(FilmListView_search);
            filmAddComponent.findFilm(tb_searchFilm_inDB.Text, FilmListView_search);
        }
    }
}
