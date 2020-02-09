using MessageSender.View;
using MessageSender.ViewModel.Interfaces;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MessageSender
{
    public partial class App : Application
    {
        public App(ISmsSource smsSource, IContactSource contactSource, IPermisionsService permisionsService)
        {
            InitializeComponent();

            MainPage = new MainPage(smsSource, contactSource, permisionsService);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
