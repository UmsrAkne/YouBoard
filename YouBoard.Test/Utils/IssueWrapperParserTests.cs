using YouBoard.Models;
using YouBoard.Utils;

namespace YouBoard.Test.Utils;

public class IssueWrapperParserTests
{
    [Test]
    public void ParseIssueWrappersFromJson_SimpleFieldsAreParsed()
    {
        var jsonResponse = """"
                [
                    {
                        "customFields": [
                            {
                                "value": {
                                    "name": "Normal",
                                    "$type": "EnumBundleElement"
                                },
                                "name": "Priority",
                                "$type": "SingleEnumIssueCustomField"
                            },
                            {
                                "value": {
                                    "name": "Feature",
                                    "$type": "EnumBundleElement"
                                },
                                "name": "Type",
                                "$type": "SingleEnumIssueCustomField"
                            },
                            {
                                "value": {
                                    "name": "未完了",
                                    "$type": "StateBundleElement"
                                },
                                "name": "State",
                                "$type": "StateIssueCustomField"
                            },
                            {
                                "value": null,
                                "name": "Assignee",
                                "$type": "SingleUserIssueCustomField"
                            },
                            {
                                "value": null,
                                "name": "Subsystem",
                                "$type": "SingleOwnedIssueCustomField"
                            },
                            {
                                "value": null,
                                "name": "経過時間",
                                "$type": "PeriodIssueCustomField"
                            },
                            {
                                "value": 417,
                                "name": "EntryNo",
                                "$type": "SimpleIssueCustomField"
                            }
                        ],
                        "summary": "Generation",
                        "idReadable": "AIIG-416",
                        "id": "2-5798",
                        "$type": "Issue"
                    }
                ]
                """";

        // act
        var result = IssueWrapperParser.ParseIssueWrappersFromJson(jsonResponse).ToList();

        // assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));

        var issue = result[0];
        Assert.Multiple(() =>
        {
            Assert.That(issue.Id, Is.EqualTo("AIIG-416"));
            Assert.That(issue.Title, Is.EqualTo("Generation"));

            // State "未完了" should map to Created and IsComplete should be false
            Assert.That(issue.State, Is.EqualTo(IssueState.Created));
            Assert.That(issue.IsComplete, Is.False);

            // Type "Feature" should map to IssueType.Feature
            Assert.That(issue.Type, Is.EqualTo(IssueType.Feature));

            // Durations are not provided as minutes objects -> should remain default(TimeSpan)
            Assert.That(issue.EstimatedDuration, Is.EqualTo(TimeSpan.Zero));
            Assert.That(issue.ElapsedDuration, Is.EqualTo(TimeSpan.Zero));

            // EntryNo is numeric and should be captured
            Assert.That(issue.EntryNo, Is.EqualTo(417));
        });
    }
}