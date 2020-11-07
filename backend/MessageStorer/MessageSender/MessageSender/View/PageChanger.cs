using MessageSender.Model.Http;
using MessageSender.ViewModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MessageSender.View
{
    public class PageChanger: IPageChanger
    {
        private App _app;
        private MainPage _mainPage;

        public PageChanger(App app, ISmsSource smsSource, 
            IContactSource contactSource, IPermisionsService permisionsService)
        {
            _app = app;
            _mainPage = new MainPage(smsSource, contactSource, permisionsService, this);
        }

        public void ShowInitialPage()
        {
            var session = new SessionStorage();
            if (session.IsUserLoggedIn())
            {
                ShowMainPage();
            }
            else
            {
                ShowLoginPage();
            }
        }

        public void ShowLoginPage()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _app.MainPage = new LoginPage(this);
            });
        }

        public void ShowMainPage()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _app.MainPage = _mainPage;
            });
            
        }
    }
}
