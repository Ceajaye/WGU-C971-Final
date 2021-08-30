using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TermTracker.Classes;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;

namespace TermTracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessmentPage : ContentPage
    {
        public Course _course;
        public MainPage _main;
        public AssessmentPage(Course course, MainPage main)
        {
            _course = course;
            _main = main;
            InitializeComponent();
            AssessmentsListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(ItemTapped);
            Title = course.CourseName;
        }

        protected override void OnAppearing()
        {
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.CreateTable<Assessment>();
                var assessmentsForCourse = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}'");
                AssessmentsListView.ItemsSource = assessmentsForCourse;
            }
        }


        async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Assessment assessment = (Assessment)e.Item;
            await Navigation.PushAsync(new EditAssessmentPage(_course, _main, assessment));
        }

        async private void btnNewAssessment_Clicked(object sender, EventArgs e)
        {
            // Only allow 2 assessments per course
            // The Add\Edit pages will make sure that there will not be 2 of a single type (Performance or Objective)
            if (getAssessmentCount() < 2)
            {
                await Navigation.PushModalAsync(new AddAssessment(_course, _main));
            }

            else
            {
                // modal windows saying "can't add more than 2 assessments"
                await Navigation.PushModalAsync(new AssessmentError());
            }
        }

        int getAssessmentCount()
        {
            int count = 0;
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                var assessmentCount = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}'");
                count = assessmentCount.Count;
            }

            return count;
        }
    }
}