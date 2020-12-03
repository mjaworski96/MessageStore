using MessageSender.View;
using MessageSender.ViewModel.Interfaces;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MessageSender
{
    public partial class App : Application
    {
        PageChanger _pageChanger;
        public App(ISmsSource smsSource, IContactSource contactSource, IPermisionsService permisionsService)
        {
            InitializeComponent();

            _pageChanger = new PageChanger(this, smsSource, contactSource, permisionsService);
            _pageChanger.ShowInitialPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            _pageChanger.OnSleep();
        }

        protected override void OnResume()
        {
            _pageChanger.OnResume();
        }
    }
}
