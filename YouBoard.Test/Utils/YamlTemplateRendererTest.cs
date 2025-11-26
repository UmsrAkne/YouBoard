using System.Collections.ObjectModel;
using YouBoard.Models;
using YouBoard.Utils;

namespace YouBoard.Test.Utils
{
    [TestFixture]
    [TestOf(typeof(YamlTemplateRenderer))]
    public class YamlTemplateRendererTest
    {

        [Test]
        public void ExportIssue_Should_Render_Yaml()
        {
            // arrange
            var issue = new IssueWrapper
            {
                Id = "Proj-1",
                Title = "Test Issue",
                State = IssueState.Created,
                EntryNo = 10,
                Description = "Hello\nWorld",
                Comments = new ObservableCollection<IssueCommentWrapper>
                {
                    new ()
                    {
                        Text = "First comment",
                        Created = DateTimeOffset.Parse("2025-01-01 10:00:00").ToUnixTimeMilliseconds(),
                    },
                    new ()
                    {
                        Text = "Second comment",
                        Created = DateTimeOffset.Parse("2025-01-01 11:00:00").ToUnixTimeMilliseconds(),
                    },
                },
            };

            var template = """

                           id: {{ Id }}
                           title: {{ Title }}
                           state: {{ State }}
                           entry: {{ EntryNo }}
                           description: |
                             {{ Description }}
                           comments:
                           {{ for c in Comments }}
                             - text: | {{ c.Text }} ({{ c.Created }})
                           {{ end }}
                           {{ End }}
                                           
                           """;

            var baseDic = issue.ToDictionary();

            // act
            var result = YamlTemplateRenderer.Render(template, baseDic);

            // assert
            StringAssert.Contains("Proj-1", result, "Issue ID");
            StringAssert.Contains("Test Issue", result, "title");
            StringAssert.Contains("Hello\n", result, "description");
            StringAssert.Contains("World", result, "description");
            StringAssert.Contains("First comment", result, "comment 1");
            StringAssert.Contains("Second comment", result, "comment 2");
            StringAssert.Contains("2025-01-01 10:00:00", result, "comment date");
            StringAssert.Contains("----", result, "separator");

            TestContext.Out.WriteLine(result);
        }
    }
}