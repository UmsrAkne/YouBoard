using System.Text.Json;
using YouBoard.Models.Request;

namespace YouBoard.Test.Models.Request
{
    public class IssueCustomFieldFactoryTests
    {
        [Test]
        public void Period_BuildsExpectedPayload_WhenMinutesPositive()
        {
            // Arrange
            const string name = "Spent time";
            const int minutes = 125;

            // Act
            var payload = IssueCustomFieldFactory.Period(name, minutes);
            var json = JsonSerializer.Serialize(payload);

            // Assert via JsonDocument to avoid ordering sensitivity
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            TestContext.WriteLine(
                JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true, }));

            Assert.Multiple(() =>
            {
                Assert.That(root.TryGetProperty("customFields", out var customFields), Is.True);
                Assert.That(customFields.ValueKind, Is.EqualTo(JsonValueKind.Array));
                Assert.That(customFields.GetArrayLength(), Is.EqualTo(1));

                var field = customFields[0];

                Assert.That(field.TryGetProperty("name", out var nameProp), Is.True);
                Assert.That(nameProp.GetString(), Is.EqualTo(name));

                Assert.That(field.TryGetProperty("$type", out var typeProp), Is.True);
                Assert.That(typeProp.GetString(), Is.EqualTo("PeriodIssueCustomField"));

                Assert.That(field.TryGetProperty("value", out var valueProp), Is.True);
                Assert.That(valueProp.ValueKind, Is.EqualTo(JsonValueKind.Object));

                Assert.That(valueProp.TryGetProperty("minutes", out var minutesProp), Is.True);
                Assert.That(minutesProp.GetInt32(), Is.EqualTo(minutes));
            });
        }

        [Test]
        public void Period_AllowsZeroMinutes()
        {
            // Arrange
            const string name = "Estimated";
            const int minutes = 0;

            // Act
            var payload = IssueCustomFieldFactory.Period(name, minutes);
            var json = JsonSerializer.Serialize(payload);

            // Assert
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.Multiple(() =>
            {
                Assert.That(root.TryGetProperty("customFields", out var customFields), Is.True);
                Assert.That(customFields.ValueKind, Is.EqualTo(JsonValueKind.Array));
                Assert.That(customFields.GetArrayLength(), Is.EqualTo(1));

                var field = customFields[0];

                Assert.That(field.GetProperty("name").GetString(), Is.EqualTo(name));
                Assert.That(field.GetProperty("$type").GetString(), Is.EqualTo("PeriodIssueCustomField"));
                Assert.That(field.GetProperty("value").GetProperty("minutes").GetInt32(), Is.EqualTo(0));
            });
        }
    }
}