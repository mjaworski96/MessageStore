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
        enum CurrentPage
        {
            UNDEFINED,
            LOGIN,
            MAIN,
        }
        private readonly App _app;
        private readonly MainPage _mainPage;
        private CurrentPage _currentPage;

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
                _currentPage = CurrentPage.LOGIN;
            });
        }

        public void ShowMainPage()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _app.MainPage = _mainPage;
                _currentPage = CurrentPage.MAIN;
            });
            
        }

        public void OnSleep()
        {
            if (_currentPage == CurrentPage.MAIN)
            {
                _mainPage.ViewModel.OnSleep();
            }
        }
        public void OnResume()
        {
            if (_currentPage == CurrentPage.MAIN)
            {
                _mainPage.ViewModel.OnResume();
            }
        }
    }
}
