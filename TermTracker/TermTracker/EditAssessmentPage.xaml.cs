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
    public partial class EditAssessmentPage : ContentPage
    {
        public Course _course;
        public Assessment _assessment;
        public MainPage _main;
        public List<string> pickerStates = new List<string> { "Objective", "Performance" };
        public List<string> pickerNotificationsStates = new List<string> { "Yes", "No" };
        public EditAssessmentPage(Course course, MainPage main, Assessment assessment)
        {
            _course = course;
            _assessment = assessment;
            _main = main;
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            pickerAssessmentType.ItemsSource = pickerStates;
            pickerAssessmentType.SelectedIndex = pickerStates.FindIndex(status => status == _assessment.AssessType);
            txtAssessmentName.Text = _assessment.AssessmentName;
            dpDueDate.Date = _assessment.End.Date;
            if (_assessment.GetNotified == 0)
            {
                pickerNotifications.SelectedIndex = 0;
            }
            else
            {
                pickerNotifications.SelectedIndex = 1;
            }
            base.OnAppearing();
        }

        private void btnDiscardChanges_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void btnEditCourse_Clicked(object sender, EventArgs e)
        {
            bool changedAssessmentType = false;
            if (_assessment.AssessType.ToString() != pickerAssessmentType.SelectedItem.ToString())
            {
                changedAssessmentType = true;
            }
            _assessment.AssessmentName = txtAssessmentName.Text;
            _assessment.End = dpDueDate.Date;
            _assessment.GetNotified = pickerNotifications.SelectedIndex;
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                var objectiveCount = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}' AND AssessType = 'Objective'");
                var performanceCount = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}' AND AssessType = 'Performance'");
                if (_assessment.AssessType.ToString() == "Objective" && objectiveCount.Count == 0)
                {
                    _assessment.AssessType = pickerAssessmentType.SelectedItem.ToString();
                    con.Update(_assessment);
                    await Navigation.PopAsync();
                }
                else if (_assessment.AssessType.ToString() == "Performance" && performanceCount.Count == 0)
                {
                    _assessment.AssessType = pickerAssessmentType.SelectedItem.ToString();
                    con.Update(_assessment);
                    await Navigation.PopAsync();
                }
                else if (((_assessment.AssessType.ToString() == "Performance" && performanceCount.Count == 1) ||
                         (_assessment.AssessType.ToString() == "Objective" && objectiveCount.Count == 1)) &&
                         !(String.IsNullOrEmpty(_assessment.Id.ToString())) &&
                          !changedAssessmentType)
                {
                    con.Update(_assessment);
                    await Navigation.PopAsync();
                }
                else
                {
                    await Navigation.PushModalAsync(new AssessmentError());
                }
            }


        }

        private async void btnDeleteAssess_Clicked(object sender, EventArgs e)
        {
            // Delete an assessment


            var result = await this.DisplayAlert("Alert!", "Do you really want to delete this assessment?", "Yes", "No");
            if (result)
            {
                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    con.Delete(_assessment);
                    // PopToRootAsync() can send user to MainPage() if user testing
                    //shows that this is preferred

                    //await Navigation.PopToRootAsync();
                    await Navigation.PopAsync();
                }

            }
        }
    }
}