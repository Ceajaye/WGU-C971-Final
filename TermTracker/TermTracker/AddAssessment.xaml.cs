using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TermTracker.Classes;
using System.Threading.Tasks;
using SQLite;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TermTracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAssessment : ContentPage
    {
        public Course _course;
        public MainPage _main;
        public AddAssessment(Course course, MainPage main)
        {
            _course = course;
            _main = main;
            InitializeComponent();
        }

        private void btnDiscardChanges_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        async private void btnAddAssessment_Clicked(object sender, EventArgs e)
        {
            if (ValidateUserInput())
            {
                Assessment newAssessment = new Assessment();
                newAssessment.AssessmentName = txtAssessmentName.Text;
                newAssessment.AssessType = pickerAssessmentType.SelectedItem.ToString();
                newAssessment.End = dpDueDate.Date;
                newAssessment.GetNotified = pickerNotifications.SelectedIndex;
                newAssessment.Course = _course.Id;

                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    var objectiveCount = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}' AND AssessType = 'Objective'");
                    var performanceCount = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}' AND AssessType = 'Performance'");
                    if (newAssessment.AssessType.ToString() == "Objective" && objectiveCount.Count == 0)
                    {
                        con.Insert(newAssessment);
                        _main.assessments.Add(newAssessment);
                        await Navigation.PopModalAsync();
                    }
                    else if (newAssessment.AssessType.ToString() == "Performance" && performanceCount.Count == 0)
                    {
                        con.Insert(newAssessment);
                        _main.assessments.Add(newAssessment);
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        await Navigation.PushModalAsync(new AssessmentError());
                    }
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

            if (txtAssessmentName == null ||
                pickerAssessmentType.SelectedItem == null ||
                dpDueDate.Date == null ||
                pickerNotifications.SelectedItem == null
                )

            {
                return false;
            }
            return valid;
        }
    }
}