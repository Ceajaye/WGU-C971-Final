using TermTracker.Classes;
using Plugin.LocalNotifications;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TermTracker
{
    public partial class MainPage : ContentPage
    {
        public List<Term> terms = new List<Term>();
        public List<Course> courses = new List<Course>();
        public List<Assessment> assessments = new List<Assessment>();
        public MainPage main;
        //Flag to only run CreateEvaluationData() once.
        bool firstTime = true;
        public MainPage()
        {
            InitializeComponent();
            termsListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(ItemTapped);
            main = this;
        }



        protected override void OnAppearing()
        {

            //var terms = new List<Term>();
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.CreateTable<Term>();
                con.CreateTable<Course>();
                con.CreateTable<Assessment>();
                terms = con.Table<Term>().ToList();
                //termsListView.ItemsSource = terms;
            }
            if (terms.Any() && firstTime)
            {
                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    con.DropTable<Assessment>();
                    con.DropTable<Course>();
                    con.DropTable<Term>();

                    con.CreateTable<Term>();
                    con.CreateTable<Course>();
                    con.CreateTable<Assessment>();

                    CreateEvaluationData(1);
                }
                firstTime = false;
                runAlerts();
            }
            else if (firstTime)
            {

                CreateEvaluationData(1);

                firstTime = false;
                runAlerts();
            }
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                terms = con.Table<Term>().ToList();
                termsListView.ItemsSource = terms;
            }

            base.OnAppearing();
        }

        private void runAlerts()
        {
            foreach (Term t in terms)
            {
                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    var courses = con.Query<Course>($"SELECT * FROM Courses WHERE Term = '{t.Id}'");
                    foreach (Course c in courses)
                    {
                        // Check for courses starting within 3 days
                        if ((c.Start - DateTime.Now).TotalDays < 3 && c.GetNotified == 1)
                        {
                            CrossLocalNotifications.Current.Show("Course Starting Soon", $"{c.CourseName} is starting on {c.Start.Date.ToString()}");
                        }
                        // Check for courses ending within 7 days
                        if ((c.End - DateTime.Now).TotalDays < 7 && c.GetNotified == 1)
                        {
                            CrossLocalNotifications.Current.Show("Course Ending Soon", $"{c.CourseName} is ending on {c.End.Date.ToString()}");
                        }

                        //Check for assessments that are coming up within 3 days
                        var assessments = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{c.Id}'");
                        foreach (Assessment a in assessments)
                        {
                            if ((a.End - DateTime.Now).TotalDays < 3 && a.GetNotified == 1)
                            {
                                CrossLocalNotifications.Current.Show("Assessment Due Soon", $"{a.AssessmentName} is starting on {a.End.Date.ToString()}");
                            }
                        }

                    }
                }
            }


        }

        private void CreateEvaluationData(int termNumber)
        {
            ////                          EVALUATION DATA CREATION
            ////       ----SAMPLE TERM----
            Term newTerm = new Term();
            newTerm.TermName = "Term " + termNumber.ToString();
            newTerm.Start = new DateTime(2021, 06, 01);
            newTerm.End = new DateTime(2021, 11, 30);
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.Insert(newTerm);
            }
            ////       ----SAMPLE COURSE----
            Course newCourse = new Course();
            newCourse.Term = newTerm.Id;
            newCourse.CourseName = "Intro To Philosophy";
            newCourse.CourseStatus = "Plan To Take";
            newCourse.Start = new DateTime(2021, 08, 12);
            newCourse.End = new DateTime(2021, 10, 12);
            newCourse.InstructorName = "Joseph Kennedy";
            newCourse.InstructorEmail = "jken252@wgu.edu";
            newCourse.InstructorPhone = "620-412-6906";
            newCourse.Notes = "Welcome to class";
            newCourse.GetNotified = 1;
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.Insert(newCourse);
            }
            ////       ----SAMPLE OBJECTIVE ASSESSMENT----
            Assessment newObjectiveAssessment = new Assessment();
            newObjectiveAssessment.AssessmentName = "BOP1";
            newObjectiveAssessment.Start = new DateTime(2021, 09, 10);
            newObjectiveAssessment.End = new DateTime(2021, 09, 10);
            newObjectiveAssessment.AssessType = "Objective";
            newObjectiveAssessment.Course = newCourse.Id;
            newObjectiveAssessment.GetNotified = 0;
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.Insert(newObjectiveAssessment);
            }
            ////       ----SAMPLE PERFORMANCE ASSESSMENT----
            Assessment newPerformanceAssessment = new Assessment();
            newPerformanceAssessment.AssessmentName = "LAG1";
            newPerformanceAssessment.Start = new DateTime(2021, 10, 13);
            newPerformanceAssessment.End = new DateTime(2021, 10, 13);
            newPerformanceAssessment.AssessType = "Performance";
            newPerformanceAssessment.Course = newCourse.Id;
            newPerformanceAssessment.GetNotified = 0;
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.Insert(newPerformanceAssessment);
            }
        }




        async private void btnNewTerm_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddTerm(this));
        }
        async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Term term = (Term)e.Item;
            await Navigation.PushAsync(new TermPage(term, main));
        }
    }
}
