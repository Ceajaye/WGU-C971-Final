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
    public partial class TermPage : ContentPage
    {
        public Term _term;
        public MainPage _main;

        public TermPage()
        {
            InitializeComponent();
        }

        public TermPage(Term term, MainPage main)
        {
            _term = term;
            _main = main;
            InitializeComponent();
            coursesListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(ItemTapped);
            Title = term.TermName;


        }
        protected override void OnAppearing()
        {
            termStart.Text = _term.Start.ToString("MM/dd/yyyy");
            termEnd.Text = _term.End.ToString("MM/dd/yyyy");
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.CreateTable<Course>();
                var coursesForTerm = con.Query<Course>($"SELECT * FROM Courses WHERE Term = '{_term.Id}'");
                coursesListView.ItemsSource = coursesForTerm;
            }
        }

        async private void btnAddCourse_Clicked(object sender, EventArgs e)
        {
            // Only allow 6 courses per term
            if (getCourseCount() < 6)
            {
                await Navigation.PushModalAsync(new AddCourse(_term, _main));
            }
            else
            {
                // modal windows saying "can't add more than 6 courses"
                await Navigation.PushModalAsync(new CourseMaximumError());
            }
        }

        int getCourseCount()
        {
            int count = 0;
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                var courseCount = con.Query<Course>($"SELECT * FROM Courses WHERE Term = '{_term.Id}'");
                count = courseCount.Count;
            }

            return count;
        }
        async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Course course = (Course)e.Item;
            await Navigation.PushAsync(new CoursePage(_term, _main, course));
        }

        private async void btnDeleteTerm_Clicked(object sender, EventArgs e)
        {
            // Before deleting term, you need to make sure to
            // delete the courses (and their assessments) associated with this term


            var result = await this.DisplayAlert("Alert!", "Do you really want to delete this term?", "Yes", "No");
            if (result)
            {
                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    var courses = con.Query<Course>($"SELECT * FROM Courses WHERE Term = '{_term.Id}'");
                    foreach (Course c in courses)
                    {
                        var assessments = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{c.Id}'");
                        foreach (Assessment a in assessments)
                        {
                            con.Delete(a);
                        }
                        con.Delete(c);
                    }
                    con.Delete(_term);
                    await Navigation.PopToRootAsync();
                }

            }
        }

        private async void btnEditTerm_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditTermPage(_term, _main));
        }
    }
}