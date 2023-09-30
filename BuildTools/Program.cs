using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Xml;

Dictionary<string, string> fileParameters = new Dictionary<string, string>();
fileParameters = getParameters();
var credentials = new VssBasicCredential(fileParameters["UserName"], fileParameters["PATToken"]);
var connection = new VssConnection(new Uri("<Url goes here>"), credentials);

using var projectClient = await connection.GetClientAsync<ProjectHttpClient>();
using var buildClient = await connection.GetClientAsync<BuildHttpClient>();
using var testClient = await connection.GetClientAsync<TestManagementHttpClient>();

var project = await projectClient.GetProject(fileParameters["ProjectName"]);
var test = buildClient.GetBuildsAsync(fileParameters["ProjectName"], top: 10).Result;
Console.WriteLine(test.ToString());
foreach (var bob in test)
{
    Console.WriteLine(bob.BuildNumber + "  " + bob.StartTime + "  " + bob.FinishTime);
    
}
var testCases = new List<TestCaseResult>();
var aggregatedTestResults = @"C:\Temp\AggregatedIndividualTests.txt";
var individualTestResults = @"C:\Temp\IndividualTests.txt";

foreach (var bd in test)
{
    var runId = await testClient.QueryTestRunsAsync2(project.Id, bd.FinishTime.Value.AddHours(-12),
        bd.FinishTime.Value.AddHours(12), buildIds: new[] { bd.Id });

    int[] runIds = runId.Select(r => r.Id).ToArray();

    foreach (var id in runIds)
    {
        var tests = await testClient.GetTestResultsAsync(project.Id, id);
        testCases.AddRange(tests);
    }
}

await using (StreamWriter writer = new StreamWriter(individualTestResults))
{
    foreach (var tc in testCases)
    {
        writer.WriteLine(tc.TestCaseTitle + "  " + TimeSpan.FromMilliseconds(tc.DurationInMs));
    }
}

var runTestCases = testCases.GroupBy(tc => tc.TestCaseTitle).Select(tcg =>
    $"{tcg.Key} : {TimeSpan.FromMilliseconds(tcg.Average(tc => tc.DurationInMs))}");

await using (StreamWriter writer = new StreamWriter(aggregatedTestResults))
{
    foreach (var tc in runTestCases)
        writer.WriteLine(tc);
}

Dictionary<String, String> getParameters()
{
    Dictionary<string, string> parameters = new Dictionary<string, string>();

    XmlDocument parametersDocument = new XmlDocument();
    parametersDocument.Load(@"parameters.xml");

    XmlNodeList? nodeList = parametersDocument.DocumentElement.SelectNodes("/Parameters/Parameter");
    
    foreach (XmlNode xmlNode in nodeList)
    {
        if (xmlNode.FirstChild.Name == "ProjectName")
        {
            parameters.Add("ProjectName", xmlNode.FirstChild.InnerText);
        }

        if (xmlNode.FirstChild.Name == "UserName")
        {
            parameters.Add("UserName", xmlNode.FirstChild.InnerText);
        }

        if (xmlNode.FirstChild.Name == "PATToken")
        {
            parameters.Add("PATToken", xmlNode.FirstChild.InnerText);
        }
    }
    return parameters;
}
