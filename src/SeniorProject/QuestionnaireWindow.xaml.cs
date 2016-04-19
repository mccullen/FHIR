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
using System.Xml.Linq;

// Fhir 
using Hl7.Fhir.Rest;
using Hl7.Fhir;
using Hl7.Fhir.Model;
using Hl7.Fhir.Validation;
using Hl7.Fhir.Serialization;
using System.IO;
using System.Xml;

namespace SeniorProject
{
	/// <summary>
	/// Interaction logic for Questionnaire.xaml
	/// </summary>
	public partial class QuestionnaireWindow : Window
	{
		private string title_;
		private XDocument _questionnaireDoc;
		public QuestionnaireWindow()
		{
			InitializeComponent();
		}
		public QuestionnaireWindow(string title)
		{
			InitializeComponent();
			title_ = title;
			this.Title = title + " Questionnaire";
			if (title == "International Prostate Symptom")
			{
				createIPSQuestionnaire();
			}
		}
		private void createIPSQuestionnaire()
		{ 
			// Load XML file.
			_questionnaireDoc = XDocument.Load(@"../../TestData/InternationalProstateSymptom.xml");

			// Get IEnumerator of the groupElements
			var groups = from groupElement in _questionnaireDoc.Descendants("group")
						select groupElement;

			// For each group
			for (int ithGroup = 0; ithGroup < groups.Count(); ++ithGroup)
			{
				// add the title text block.
				addTitles(groups.ElementAt(ithGroup), ithGroup);
				//questionnaireXaml = OutputTitle(questionnaireXaml, currentGroup, groupElement);

				// add the question text block and radio button options.
				addQuestions(groups.ElementAt(ithGroup), ithGroup);
				//questionnaireXaml = OutputQuestions(questionnaireXaml, currentGroup, groupElement);
			}
		}
		private void addTitles(XElement groupElement, int ithGroup)
		{ 
			// Get title
			var title = from titleElement in groupElement.Elements("title")
						select titleElement.Attribute("value").Value;

			// If first title, add the title text block.
			if (ithGroup == 0)
			{
				TextBlock titleTextBlock = new TextBlock();
				titleTextBlock.FontSize = 16;
				titleTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
				titleTextBlock.FontWeight = FontWeights.Bold;
				titleTextBlock.MinHeight = 30;
				titleTextBlock.Text = title.ElementAt(0);
				int indexOfSubmit = questionsStackPanel.Children.IndexOf(submitButton);
				questionsStackPanel.Children.Insert(indexOfSubmit, titleTextBlock);
			}
			// Else, add a normal text block.
			else
			{
				TextBlock titleTextBlock = new TextBlock();
				titleTextBlock.FontSize = 14;
				titleTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
				titleTextBlock.MinHeight = 20;
				titleTextBlock.Text = title.ElementAt(0);
				int indexOfSubmit = questionsStackPanel.Children.IndexOf(submitButton);
				questionsStackPanel.Children.Insert(indexOfSubmit, titleTextBlock);
			}
		
		}
		private void addQuestions(XElement groupElement, int ithGroup)
		{ 
			// Get IEnumeration of questionElements
			var questions = from questionElement in groupElement.Elements("question")
							select questionElement;

			// For each question
			for (int ithQuestion = 0; ithQuestion < questions.Count(); ++ithQuestion)
			{
				// add title text block
				TextBlock titleTextBlock = new TextBlock();
				titleTextBlock.Text =
					questions.ElementAt(ithQuestion).Element("text").Attribute("value").Value.ToString();
				int submitIndex = questionsStackPanel.Children.IndexOf(submitButton);
				questionsStackPanel.Children.Insert(submitIndex, titleTextBlock);

				// Get IEnumberable of option elements
				var options = 
					from optionElement in questions.ElementAt(ithQuestion).Elements("option")
					select optionElement;

				// Add radio buttons for each option.

				for (int currOption = 0; currOption < options.Count(); ++currOption)//foreach (var option in options)
				{
					RadioButton option = new RadioButton();
					option.Content = 
						options.ElementAt(currOption).Element("display").Attribute("value").Value.ToString();
					option.GroupName = "group" + ithQuestion.ToString();
					option.Name = "name" + currOption.ToString() + option.GroupName;
					submitIndex = questionsStackPanel.Children.IndexOf(submitButton);
					questionsStackPanel.Children.Insert(submitIndex, option);
					// If last question, add some space for the next question.
					if (currOption == options.Count() - 1)
					{
						option.MinHeight = 35;
					}
				}
			}
		
		}

		private void submitButton_Click(object sender, RoutedEventArgs e)
		{
			// Get resource type

			if (_title == "International Prostate Symptom")
			{
				postIPSS();
			}

			this.Close();
		}
		private StringBuilder createQuestionnaireResponseXMLForIPSS()
		{

			StringBuilder doc = new StringBuilder();
			doc.AppendLine("<QuestionnaireResponse xmlns=\"http://hl7.org/fhir\">");

			// Add id
			string id = _questionnaireDoc.Descendants("id").
				Attributes("value").ElementAt(0).Value.ToString();
			doc.AppendLine("<id value=\"" + id + "\"/>");

			// Add status
			doc.AppendLine("<status value=\"completed\"/>");

			// Add outer group (the group with the link id and title).

			doc.AppendLine("<group>");
			string groupTitle = _questionnaireDoc.Descendants("group").ElementAt(0)
				.Descendants("title").Attributes("value").ElementAt(0).Value.ToString();
			string linkId = _questionnaireDoc.Descendants("linkId").ElementAt(0)
				.Attributes("value").ElementAt(0).Value.ToString();
			doc.AppendLine("<title value=\"" + groupTitle + "\"/>");
			doc.AppendLine("<linkId value=\"" + linkId + "\"/>");
			doc.AppendLine("</group>");

			// Add inner group and questions within it.
			doc.AppendLine("<group>");
			linkId = _questionnaireDoc.Descendants("linkId").ElementAt(1)
				.Attributes("value").ElementAt(0).Value.ToString();
			doc.AppendLine("<linkId value=\"" + linkId + "\"/>");

			// For each question
			var questions = from questionElement in _questionnaireDoc.Descendants("question")
							select questionElement;
			foreach (var question in questions)
			{
				// Add linkId, text, and answer

				doc.AppendLine("<question>");

				linkId = question.Descendants("linkId").ElementAt(0).
					Attribute("value").Value.ToString();
				doc.AppendLine("<linkId value=\"" + linkId + "\"/>");

				string text = question.Descendants("text").ElementAt(0).
					Attribute("value").Value.ToString();
				doc.AppendLine("<text value=\"" + text + "\"/>");

				doc.AppendLine("<answer>");
				// TODO: print valuestring.
				doc.AppendLine("<valueString value=\"testData\"/>");

				doc.AppendLine("</answer>");

				doc.AppendLine("</question>");
			}

			doc.AppendLine("</group>");

			doc.AppendLine("</QuestionnaireResponse>");

			return doc;
		}
		private void postIPSS()
		{
			// Connect to server.
			Uri endpoint = new Uri("http://spark.furore.com/fhir");
			FhirClient client = new FhirClient(endpoint);

			// Create XML document from user input
			StringBuilder doc = createQuestionnaireResponseXMLForIPSS();

			string str = doc.ToString();

			// Create resource
			QuestionnaireResponse questionnaireResponse = 
				(QuestionnaireResponse)FhirParser.ParseResource(
					XmlReader.Create(str));

			// Post to server
			client.Create<QuestionnaireResponse>(questionnaireResponse);

		}
	}
}
