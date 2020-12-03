using MessageSender.ViewModel;
using MessageSender.ViewModel.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MessageSender.View
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MessageSenderViewModel ViewModel { get; set; }
        public MainPage(ISmsSource smsSource, IContactSource contactSource, 
            IPermisionsService permisionsService, IPageChanger pageChanger)
        {
            InitializeComponent();
            BindingContext = ViewModel = new MessageSenderViewModel(smsSource, contactSource,
                permisionsService, pageChanger);
        }
    }
}
