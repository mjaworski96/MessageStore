using MessageSender.ViewModel;
using MessageSender.ViewModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MessageSender.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage(IPageChanger pageChanger)
        {
            InitializeComponent();
            BindingContext = new LoginPageViewModel(pageChanger);
        }
    }
}