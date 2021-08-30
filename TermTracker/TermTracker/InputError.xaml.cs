using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TermTracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InputError : ContentPage
    {
        public InputError()
        {
            InitializeComponent();
        }

        private void btnOK_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}