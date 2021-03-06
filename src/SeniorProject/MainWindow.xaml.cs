﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Markup;

// Fhir 
using Hl7.Fhir.Rest;
using Hl7.Fhir;
using Hl7.Fhir.Model;
using Hl7.Fhir.Validation;
using Hl7.Fhir.Serialization;

namespace SeniorProject
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		static private XNamespace ns = "http://hl7.org/fhir";
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void Post_Click(object sender, RoutedEventArgs e)
		{
			// Connect to server.
			Uri endpoint = new Uri("http://spark.furore.com/fhir");
			FhirClient client = new FhirClient(endpoint);

			// Load xml document
			StringReader reader =
				new StringReader(File.ReadAllText(@"../../TestData/Questionnaire.xml"));

			// Create resource
			Questionnaire questionnaire = 
				(Questionnaire)FhirParser.ParseResource(XmlReader.Create(reader));

			// Post to server
			client.Create<Questionnaire>(questionnaire);
		}
		private void Retrieve_Click(object sender, RoutedEventArgs e)
		{
			// Load XML file.
			//XDocument document = XDocument.Load(@"../../TestData/Questionnaire.xml");
			XDocument document = XDocument.Load(@"../../TestData/Questionnaire.xml");
			

			// Create Xaml file for questionnaire.
			string questionnaireXaml =
				"<Window xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n" +
				"xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n" +
				"Height=\"400\" Width=\"500\" WindowStartupLocation=\"CenterScreen\">\n" +
				"<ScrollViewer VerticalScrollBarVisibility=\"Auto\"><StackPanel>\n";


			var groups = from groupElement in document.Descendants("group")
						select groupElement;
			int currentGroup = 0;
			// For each group
			foreach (var groupElement in groups)
			{
				// Output the title
				questionnaireXaml = OutputTitle(questionnaireXaml, currentGroup, groupElement);

				// Output questions
				questionnaireXaml = OutputQuestions(questionnaireXaml, currentGroup, groupElement);
				++currentGroup;
			}
			questionnaireXaml += "<Button HorizontalAlignment=\"Center\">Submit</Button>";

			// Add ending to xaml file.
			questionnaireXaml += "</StackPanel></ScrollViewer>\n" +
				"</Window>";
			File.WriteAllText("Questionnaire.xaml", questionnaireXaml);

			// Create and show window using the xaml file.
			DisplayWindow();

		}

		private static void DisplayWindow()
		{

			Window myWindow = null;
			try
			{
				using (Stream reader = File.Open("Questionnaire.xaml", FileMode.Open))
				{
					// Connect the XAML to the Window object
					myWindow = (Window)XamlReader.Load(reader);

					// Show window as a dialog and clean up.
					myWindow.ShowDialog();
					myWindow.Close();
					myWindow = null;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private static string OutputQuestions(string questionnaireXaml, int currentGroup, XElement groupElement)
		{
			var questions = from questionElement in groupElement.Elements("question")
							select questionElement;

			int currentQuestion = 0;
			// For each question
			foreach (var question in questions)
			{
				// Output the title
				questionnaireXaml += "<TextBlock>" +
					question.Element("text").Attribute("value").Value.ToString() +
					"</TextBlock>\n";

				var options = from optionElement in question.Elements("option")
							  select optionElement;
				// Output every option for the question
				for (int currOption = 0; currOption < options.Count(); ++currOption)//foreach (var option in options)
				{
					questionnaireXaml += "<RadioButton GroupName=\"" + "group" + currentQuestion.ToString() + "\" ";

					if (currOption == options.Count() - 1)
					{
						questionnaireXaml += "MinHeight=\"35\"";
					}
					questionnaireXaml += ">" + 
						options.ElementAt(currOption).Element("display").Attribute("value").Value.ToString() +
						"</RadioButton>\n";
				}
				++currentQuestion;

			}
			return questionnaireXaml;
		}

		private static string OutputTitle(string questionnaireXaml, int currentGroup, XElement groupElement)
		{
			var title = from titleElement in groupElement.Elements("title")
						select titleElement.Attribute("value").Value;

			// output the title
			if (currentGroup == 0)
			{
				questionnaireXaml += "<TextBlock HorizontalAlignment=\"Center\" FontSize=\"16\" " +
					"MinHeight=\"30\" FontWeight=\"Bold\"" +
					">" +
					title.ElementAt(0) + "</TextBlock>\n";
			}
			else
			{
				questionnaireXaml += "<TextBlock FontSize=\"14\" MinHeight=\"20\">" +
					title.ElementAt(0) + "</TextBlock>\n";
			}
			return questionnaireXaml;
		}
	}
}
