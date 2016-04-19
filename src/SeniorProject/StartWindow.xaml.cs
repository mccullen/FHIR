using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SeniorProject
{
	/// <summary>
	/// Interaction logic for App1.xaml
	/// </summary>
	public partial class StartWindow : Window
	{
		public StartWindow()
		{
			InitializeComponent();
		}

		private void LoadQuestionnaireButton_Click(object sender, RoutedEventArgs e)
		{
			if (questionnaireCombobox.SelectedIndex > -1)
			{
				string questionnaire = questionnaireCombobox.Text;
				QuestionnaireWindow questionnaireWindow = new QuestionnaireWindow(questionnaire);

				this.Close();
				questionnaireWindow.Show();
			}
			else
			{
				MessageBox.Show("You must select a questionnaire and a patient.");
			}
		}
	}
}
