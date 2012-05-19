﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;
using GG.Libraries;
using GG.UserControls;
using GG.ViewModels;

namespace GG
{
    class MainWindowViewModel : BaseViewModel
    {
        public ObservableCollection<RepositoryViewModel> RepositoryViewModels { get; set; }
        public ObservableCollection<RepositoryViewModel> RecentRepositories { get; set; }

        public MainWindowViewModel()
        {
            RepositoryViewModels = new ObservableCollection<RepositoryViewModel> { };
            RecentRepositories = new ObservableCollection<RepositoryViewModel> { };

            CreateTabCommand = new DelegateCommand(CreateTab);
            CloseTabCommand = new DelegateCommand(CloseTab);
        }

        /// <summary>
        /// Loads the initial configuration.
        /// </summary>
        public void Load()
        {
            if (File.Exists("./Configuration.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                using (FileStream fileStream = new FileStream("./Configuration.xml", FileMode.Open))
                {
                    Configuration configuration = (Configuration) serializer.Deserialize(fileStream);

                    // Fill the "RecentRepositories" collection and load them.
                    foreach (RecentRepositoryConfiguration recent in configuration.RecentRepositories)
                    {
                        RepositoryViewModel repo = new RepositoryViewModel { Name = recent.Name, RepositoryFullPath = recent.RepositoryFullPath };
                        repo.Load();

                        RepositoryViewModels.Add(repo);
                        RecentRepositories.Add(repo);
                    }
                }
            }

            CreateTab(new object()); // Create "New Tab".

            // Select the first tab.
            TabControl tabControl = UIHelper.FindChild<TabControl>(Application.Current.MainWindow, "RepositoryTabs");
            tabControl.SelectedIndex = 0;
        }

        #region Commands.

        public DelegateCommand CreateTabCommand { get; private set; }
        public DelegateCommand CloseTabCommand { get; private set; }

        /// <summary>
        /// Closes the current tab.
        /// </summary>
        /// <param name="action"></param>
        private void CloseTab(object action)
        {
            TabControl tabControl = UIHelper.FindChild<TabControl>(Application.Current.MainWindow, "RepositoryTabs");
            ObservableCollection<RepositoryViewModel> repositories = tabControl.ItemsSource as ObservableCollection<RepositoryViewModel>;
            repositories.Remove(tabControl.SelectedContent as RepositoryViewModel);

            if (tabControl.Items.Count == 0)
                CreateTab(new object());
        }

        /// <summary>
        /// Creates a new tab.
        /// </summary>
        /// <param name="action"></param>
        private void CreateTab(object action)
        {
            TabControl tabControl = UIHelper.FindChild<TabControl>(Application.Current.MainWindow, "RepositoryTabs");
            MainWindowViewModel mainWindowViewModel = Application.Current.MainWindow.DataContext as MainWindowViewModel;

            RepositoryViewModel repository = new RepositoryViewModel
            {
                Name = "New Tab",
                NotOpened = true,
                RepositoryFullPath = ""
            };

            mainWindowViewModel.RepositoryViewModels.Add(repository);

            tabControl.SelectedItem = repository;

            // TODO: Automatically give focus to the ListView's first item.
            //ListView recentRepositoriesList = UIHelper.FindChild<ListView>(tabControl.ItemContainerGenerator.ContainerFromIndex(0), "RecentRepositoriesList");
        }

        #endregion
    }
}