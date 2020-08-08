#tool "nuget:?package=OpenCover&version=4.7.922"
#tool "nuget:?package=ReportGenerator&version=4.2.20"

using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

var target = Argument("target", "Build");

Task("Build")
    .Does(() =>
{
    DotNetCoreBuild("./EnkiGroup.sln");
});

Task("Test")
    .Does(() =>
{
    DotNetCoreTest("./EnkiGroup.sln");
});

Task("Coverage")
    .Does(() =>
{
    if(System.IO.File.Exists("result.xml"))
        System.IO.File.Delete("result.xml");
    if(System.IO.Directory.Exists("coverageOutput"))
        System.IO.Directory.Delete("coverageOutput", true);
    
    OpenCover(tool =>
    {
        tool.DotNetCoreTest("./EnkiGroup.sln");
    },
    new FilePath("./result.xml"),
    new OpenCoverSettings() { OldStyle = true, ReturnTargetCodeOffset = 0 }
        .WithFilter("-[EnkiGroup.Web*]*Infra*")
        .WithFilter("-[EnkiGroup.Web*]*Program*")
        .WithFilter("-[EnkiGroup.Web*]*Startup*")
        .WithFilter("-[EnkiGroup.Shared*]*ViewModels*")
        .WithFilter("-[EnkiGroup.Shared*]*IEnumerableExtensions*")
        .WithFilter("-[EnkiGroup.Shared*]*OperationResult*")
        .WithFilter("-[EnkiGroup.Data*]*Profiles*")
        .WithFilter("-[EnkiGroup.Data*]*EntityTypeConfigurations*")
        .WithFilter("-[EnkiGroup.Data*]*EnkiGroupContext*")
        .WithFilter("-[EnkiGroup.Data*]*Microsoft*")
        .WithFilter("-[EnkiGroup.Data*]*System*")
        .WithFilter("-[EnkiGroup.IoC*]*SimpleInjectorBootstrap*")
        .WithFilter("+[EnkiGroup*]*")
        .WithFilter("-[EnkiGroup.*.Tests]*")
    );

    ReportGenerator("./result.xml", "./coverageOutput", new ReportGeneratorSettings  { ReportTypes = new []
    {
        ReportGeneratorReportType.TextSummary,
        ReportGeneratorReportType.Html
    }});
});

RunTarget(target);