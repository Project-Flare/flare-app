﻿namespace flare_app
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            //Application.Current.UserAppTheme = AppTheme.Unspecified;
        }
    }
}
