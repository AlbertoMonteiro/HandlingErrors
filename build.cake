#tool nuget:?package=ReportGenerator&version=5.0.2

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Build")
    .Does(() =>
{
    DotNetBuild("HandlingErrors.sln");
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    if (System.IO.Directory.Exists("testresults"))
        System.IO.Directory.Delete("testresults", true);

    var settings = new DotNetTestSettings
    {
        NoRestore = true,
        ArgumentCustomization  = x => x.Append("--collect:\"XPlat Code Coverage\""),
        Verbosity = DotNetVerbosity.Quiet
    };

    DotNetTest("HandlingErrors.sln", settings);
});

Task("Coverage")
    .IsDependentOn("Test")
    .Does(() =>
{
    if (System.IO.Directory.Exists("coverage"))
        System.IO.Directory.Delete("coverage", true);

    GlobPattern coverageFiles = "./tests/**/*.cobertura.xml";

    ReportGenerator(coverageFiles, "./coverage", new ReportGeneratorSettings  { ReportTypes = new []
    {
        ReportGeneratorReportType.TextSummary,
        ReportGeneratorReportType.lcov,
        ReportGeneratorReportType.Cobertura,
        ReportGeneratorReportType.HtmlInline_AzurePipelines_Dark
    }});
});

RunTarget(target);
