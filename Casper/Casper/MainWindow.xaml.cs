using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace Alto_IT
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MySqlConnection MyServerSql { get; set; }

        public ApplicationDatabase database;

        private MySqlCommand cmd;
        public MainWindow()
        {
            InitializeComponent();
            database = new ApplicationDatabase();
            MyServerSql = new MySqlConnection("server=137.74.118.171;user id=sta33;password=cu267a;persistsecurityinfo=True;database=sta33;allowuservariables=True");
            MyServerSql.Open();

        }

        private void SignIn_bouton_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    User us = (from u in database.UsersDatabase
            //             where u.Identifiant == Champ_identifiant.Text && u.Password == Champ_password.Password
            //               select u).First();

            //    Dashboard D = new Dashboard(this);
            //    D.Show();
            //    Close();
            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("Identifiant ou Mot de Passe invalide");
            //}

            cmd = new MySqlCommand("SELECT * FROM Users WHERE Identifiant = '"+ Champ_identifiant.Text+"' AND Password = '"+Champ_password.Password+"'", MyServerSql);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                Projet P = new Projet(this);
                P.Show();
                Close();
                reader.Close();
            }
            else
            {
                MessageBox.Show("User non identifié");
                reader.Close();
            }



            //Dashboard D = new Dashboard(this);
            //D.Show();


        }

        public async void WebQueryMySql(string Commande)
        {
            cmd = new MySqlCommand(Commande,MyServerSql);
            await cmd.ExecuteNonQueryAsync();
        }
        

    }
}
