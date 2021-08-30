using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TermTracker.Classes;
using SQLite;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TermTracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTerm : ContentPage
    {
        public MainPage mainPage;
        public AddTerm(MainPage main)
        {
            mainPage = main;
            InitializeComponent();
        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            if (ValidateUserInput())
            {


                Term newTerm = new Term();
                newTerm.TermName = txtTermTitle.Text;
                newTerm.Start = dpStartDate.Date;
                newTerm.End = dpEndDate.Date;
                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    con.Insert(newTerm);

                    
                    mainPage.terms.Add(newTerm);
                    await Navigation.PopModalAsync();
                }
            }
            else
            {
                await Navigation.PushModalAsync(new InputError());
            }

        }
        private bool ValidateUserInput()
        {
            bool valid = true;

            if (txtTermTitle.Text == null ||
                dpStartDate.Date == null ||
                dpEndDate.Date == null ||
                dpEndDate.Date < dpStartDate.Date
                )

            {
                return false;
            }
            return valid;
        }

        private void btnExit_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}