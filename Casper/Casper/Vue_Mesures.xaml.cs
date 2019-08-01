﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Alto_IT
{
    /// <summary>
    /// Logique d'interaction pour Vue_Mesures.xaml
    /// </summary>
    public partial class Vue_Mesures : Page
    {
        public Mesures MesureSelectionne { get; set; }
        public Dashboard Dash { get; set; }

        public Mesures ROOT_Mesures { get; set; }

        readonly object _LockCollection = new object();
        public Vue_Mesures()
        {
            InitializeComponent();

        }

        public Vue_Mesures(Dashboard D)
        {
            InitializeComponent();
            Dash = D;
            ROOT_Mesures = new Mesures("Menu");
            Dash.Vue_Mesure = this;
            treeviewFrame.Items.Add(ROOT_Mesures);

        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                String Filename = MesureSelectionne.DocumentPath;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = Filename;
                process.Start();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Veuillez sélectionner une Mesure");
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Aucun document associé");
            }
        }

        private void TreeviewFrame_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MesureSelectionne = (Mesures)treeviewFrame.SelectedItem;
        }



        public void AfficherTreeViewMesures()
        {
            Application.Current.Dispatcher.Invoke(delegate ()
            {
                ROOT_Mesures.MesureObservableCollec.Clear();
            });
            Mesures[] Li = Dash.mw.database.MesuresDatabase.ToArray();
            Mesures[] Lj = Li;
            int[] Ls = new int[Lj.Length];
            int[] lar = new int[Lj.Length];
            for (int i = 0; i < Lj.Length; i++)
            {
                Ls[i] = Lj[i].Id;
            }
            for (int i = 0; i < Li.Length; i++)
            {
                int M = Li[i].Id;
                if ((Li[i].Id == Lj[i].FKToMesure) && (Array.BinarySearch(Ls, M) < 0))
                {
                    lar[i] = M;
                    lock (_LockCollection)
                    {
                        Application.Current.Dispatcher.Invoke(delegate ()
                        {
                            Dash.mw.database.MesuresDatabase.ToList()[i].MesureObservableCollec.Add(Dash.mw.database.MesuresDatabase.ToList()[i]);
                        });
                        Thread.Sleep(2);
                    }
                }
                else if ((Li[i].FKToMesure == 0) && (Dash.ProjetEncours.Id == Li[i].FKToProjets))
                {
                    int MM = Li[i].Id;
                    if (Array.BinarySearch(lar, MM) < 0)
                    {
                        lar[i] = MM;
                        lock (_LockCollection)
                        {
                            Application.Current.Dispatcher.Invoke(delegate ()
                            {
                                ROOT_Mesures.MesureObservableCollec.Add(Dash.mw.database.MesuresDatabase.ToList()[i]);
                            });
                            Thread.Sleep(2);
                        }

                    }
                }
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(AfficherTreeViewMesures);
            RemplirTab();
        }

        public void RemplirTab()
        {

            foreach (Mesures mesureencours in Dash.mw.database.MesuresDatabase)
            {
                mesureencours.ListeExigenceCheck.Clear();

                mesureencours.Dico_ExigenceCheck.Clear();

                var rechercheid = (from idrecherche in Dash.mw.database.RelationMesuresExigenceDatabase
                                   where idrecherche.IdMesures == mesureencours.Id
                                   select idrecherche.IdExigence).ToList();


                foreach (Exigence item in Dash.mw.database.ExigenceDatabase)
                {
                    if (rechercheid.Contains(item.Id) && !mesureencours.Dico_ExigenceCheck.ContainsKey(item))
                    {
                        mesureencours.Dico_ExigenceCheck.Add(item, true);
                        mesureencours.ListeExigenceCheck.Add(item.Name);
                    }
                    else if (!rechercheid.Contains(item.Id) && !mesureencours.Dico_ExigenceCheck.ContainsKey(item))
                    {
                        mesureencours.Dico_ExigenceCheck.Add(item, false);
                    }
                }

            }

        }

    }
}