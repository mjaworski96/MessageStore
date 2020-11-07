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

            var pageChanger = new PageChanger(this, smsSource, contactSource, permisionsService);
            pageChanger.ShowInitialPage();
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
