using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace Alto_IT
{
    /// <summary>
    /// Logique d'interaction pour ModifierMesure.xaml
    /// </summary>
    public partial class ModifierMesure : Window
    {
        public List<string> ListeExigenceCheck { get; set; }

        public Vue_Mesures Vue { get; set; }

        public List<int> ListExigenceCheckId { get; set; }
        public MainWindow mw { get; set; }

        public ModifierMesure()
        {
            InitializeComponent();
            ListeExigenceCheck = new List<string>();
            ListExigenceCheckId = new List<int>();
        }

        public ModifierMesure(MainWindow m, Vue_Mesures vu)
        {
            InitializeComponent();
            mw = m;
            Vue = vu;
            ListeExigenceCheck = new List<string>();
            ListExigenceCheckId = new List<int>();
        }

        private void ModifierMesure_Click(object sender, RoutedEventArgs e)
        {
            if (Vue.MesureSelectionne != null && Vue.MesureSelectionne.Nom != "Menu")
            {
                string CurrentItem = Vue.Dash.FormaterToSQLRequest("_" + Vue.Dash.ProjetEncours.Id + Vue.MesureSelectionne.Nom);
                string CurrentTitle = Vue.Dash.SimpleCotFormater(Vue.MesureSelectionne.Nom);
                string CurrentDesc = Vue.Dash.SimpleCotFormater(Vue.MesureSelectionne.Description);



                using (ApplicationDatabase context = new ApplicationDatabase())
                {
                    string newTableName = Vue.Dash.TableFormaterMesures(Vue.Dash.SimpleCotFormater(Vue.Dash.FormaterToSQLRequest(Title.Text)));

                    try
                    {
                        //renomme la table
                        var w = context.Database.ExecuteSqlCommand("EXEC sp_rename '" + CurrentItem + "', '" + newTableName + "'");
                        mw.WebQueryMySql("RENAME TABLE " + CurrentItem + " TO " + newTableName);
                        //modif dans la table Exigence

                        var yy = context.Database.ExecuteSqlCommand("UPDATE Mesures" + " SET Description = '" + Vue.Dash.SimpleCotFormater(Content.Text) + "' WHERE Id = " + "'" + Vue.MesureSelectionne.Id + "'" + " ");
                        var y = context.Database.ExecuteSqlCommand("UPDATE Mesures" + " SET Nom = '" + Vue.Dash.SimpleCotFormater(Title.Text) + "' WHERE Id = " + "'" + Vue.MesureSelectionne.Id + "'" + " ");
                        mw.WebQueryMySql("UPDATE Mesures" + " SET Description = '" + Vue.Dash.SimpleCotFormater(Content.Text) + "' WHERE Id = " + "'" + Vue.MesureSelectionne.Id + "'" + " ");
                        mw.WebQueryMySql("UPDATE Mesures" + " SET Nom = '" + Vue.Dash.SimpleCotFormater(Title.Text) + "' WHERE Id = " + "'" + Vue.MesureSelectionne.Id + "'" + " ");
                        try
                        {
                            //modif dans table parents
                            var ParentName = context.Database.SqlQuery<string>("SELECT Name from Exigences WHERE Id= " + Vue.MesureSelectionne.FKToMesure).FirstOrDefault();
                            if (ParentName != "Menu" && ParentName != null)
                            {
                                ParentName = Vue.Dash.TableFormaterMesures(Vue.Dash.SimpleCotFormater(Vue.Dash.FormaterToSQLRequest(ParentName)));
                                var zz = context.Database.ExecuteSqlCommand("UPDATE " + ParentName + " SET Description = '" + Vue.Dash.SimpleCotFormater(Content.Text) + "' WHERE Titre = '" + Vue.MesureSelectionne.Nom + "'");
                                var z = context.Database.ExecuteSqlCommand("UPDATE " + ParentName + " SET Titre = '" + Vue.Dash.SimpleCotFormater(Title.Text) + "' WHERE Titre = '" + Vue.MesureSelectionne.Nom + "'");

                                mw.WebQueryMySql("UPDATE " + ParentName + " SET Description = '" + Vue.Dash.SimpleCotFormater(Content.Text) + "' WHERE Titre = '" + Vue.MesureSelectionne.Nom + "'");
                                mw.WebQueryMySql("UPDATE " + ParentName + " SET Titre = '" + Vue.Dash.SimpleCotFormater(Title.Text) + "' WHERE Titre = '" + Vue.MesureSelectionne.Nom + "'");

                            }
                            Vue.MesureSelectionne.Nom = Title.Text;
                            Vue.MesureSelectionne.Description = Content.Text;



                        }
                        catch (Exception)
                        {
                            var ww = context.Database.ExecuteSqlCommand("EXEC sp_rename '" + newTableName + "', '" + CurrentItem + "'");
                            mw.WebQueryMySql("RENAME TABLE " + newTableName + " TO " + CurrentItem);


                            var yty = context.Database.ExecuteSqlCommand("UPDATE Mesures" + " SET Description = '" + CurrentDesc + "' WHERE Id = " + "'" + Vue.MesureSelectionne.Id + "'" + " ");
                            var yt = context.Database.ExecuteSqlCommand("UPDATE Mesures" + " SET Nom = '" + CurrentTitle + "' WHERE Id = " + "'" + Vue.MesureSelectionne.Id + "'" + " ");

                            mw.WebQueryMySql("UPDATE Mesures" + " SET Description = '" + CurrentDesc + "' WHERE Id = " + "'" + Vue.MesureSelectionne.Id + "'" + " ");
                            mw.WebQueryMySql("UPDATE Mesures" + " SET Nom = '" + CurrentTitle + "' WHERE Id = " + "'" + Vue.MesureSelectionne.Id + "'" + " ");

                            MessageBox.Show("Impossible d ajouter à la table Parent", "erreur", MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Impossible de Modifier la table", "erreur", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    switch (ComboBoxStatus.Text)
                    {
                        case "Appliquée":
                            Vue.MesureSelectionne.Status = STATUS.appliquee;
                            break;
                        case "Programmée":
                            Vue.MesureSelectionne.Status = STATUS.programmee;
                            break;
                        case "Non Appliquée":
                            Vue.MesureSelectionne.Status = STATUS.non_appliquee;
                            break;
                        case "Non Évaluée":
                            Vue.MesureSelectionne.Status = STATUS.non_evalue;
                            break;
                        case "Non Applicable":
                            Vue.MesureSelectionne.Status = STATUS.non_applicable;
                            break;
                        default:
                            break;
                    }

                    mw.database.SaveChanges();
                    Vue.AfficherTreeViewMesures();
                    Close();

                }

            }
            else
            {
                MessageBox.Show("Selectionnez une ligne", "error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Bouton_AjouterDocument_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.ShowDialog();
            string filename = "(" + Vue.MesureSelectionne.Id + ")" + open.SafeFileName;
            string targetPath = @"C:\Users\stagiaire\Desktop\ALTO-IT\Casper\Casper\bin\Debug\Files\" + filename;
            try
            {
                File.Copy(open.FileName, targetPath);
                Vue.MesureSelectionne.DocumentPath = targetPath;
                Vue.MesureSelectionne.DocumentName = filename;
            }
            catch (System.Exception)
            {
                if (MessageBox.Show("Ce fichier éxiste déja voulez vous le supprimer ?", "Erreur", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        File.Delete(targetPath);
                        File.Copy(open.FileName, targetPath);
                        Vue.MesureSelectionne.DocumentPath = targetPath;
                        Vue.MesureSelectionne.DocumentName = filename;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Impossible de supprimer");
                    }
                }
            }
            MessageBox.Show("Document bien enregistré");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Vue.MesureSelectionne != null)
            {
                foreach (KeyValuePair<Exigence, bool> item in Vue.MesureSelectionne.Dico_ExigenceCheck)
                {
                    if (item.Key.FKToProjet == Vue.Dash.ProjetEncours.Id)
                    {
                        CheckBox C = new CheckBox();
                        C.Content = item.Key.Name;
                        C.IsChecked = item.Value;
                        C.Checked += CheckboxExigences_Checked;
                        C.Unchecked += CheckboxExigence_Unchecked;
                        listviewMesures.Items.Add(C);
                    }

                }
            }
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Vue.Dash.FenetreOuverte = false;
            Vue.RemplirTab();
        }


        private void CheckboxExigences_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox Cb = (CheckBox)sender;
            ListeExigenceCheck.Add(Cb.Content.ToString());


            var query = from i in mw.database.ExigenceDatabase
                        where i.Name == Cb.Content.ToString()
                        select i.Id;

            ListExigenceCheckId.Add(query.FirstOrDefault());

            RelationsMesuresExigences rel = new RelationsMesuresExigences(query.FirstOrDefault(), Vue.MesureSelectionne.Id);

            mw.database.RelationMesuresExigenceDatabase.Add(rel);

            var Exisel = from exi in mw.database.ExigenceDatabase
                         where exi.Name == Cb.Content.ToString()
                         select exi;

            mw.WebQueryMySql("INSERT INTO RelationMesuresExigences(IdExigence, IdMesure) VALUES(" + Exisel.FirstOrDefault().Id + ", " + Vue.MesureSelectionne.Id + "");

            Vue.MesureSelectionne.Dico_ExigenceCheck[Exisel.FirstOrDefault()] = true;
            Exisel.FirstOrDefault().Dico_MesuresCheck[Vue.MesureSelectionne] = true;
            mw.database.SaveChanges();


        }

        private void CheckboxExigence_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox Cb = (CheckBox)sender;
            ListeExigenceCheck.Remove(Cb.Content.ToString());

            var query = from i in mw.database.ExigenceDatabase
                        where i.Name == Cb.Content.ToString()
                        select i.Id;

            ListExigenceCheckId.Remove(query.FirstOrDefault());

            var delete = from relation in mw.database.RelationMesuresExigenceDatabase
                         where relation.IdMesures == Vue.MesureSelectionne.Id && relation.IdExigence == query.FirstOrDefault()
                         select relation;

            mw.database.RelationMesuresExigenceDatabase.Remove(delete.FirstOrDefault());

            var Exisel = from m in mw.database.ExigenceDatabase
                         where m.Name == Cb.Content.ToString()
                         select m;

            mw.WebQueryMySql("DELETE FROM RelationMesuresExigences WHERE IdExigence = " + Exisel.FirstOrDefault().Id + " AND IdMesure = " + Vue.MesureSelectionne.Id + "");

            Vue.MesureSelectionne.Dico_ExigenceCheck[Exisel.FirstOrDefault()] = false;
            Exisel.FirstOrDefault().Dico_MesuresCheck[Vue.MesureSelectionne] = false;
            mw.database.SaveChanges();

        }
    }
}
