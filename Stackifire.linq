<Query Kind="Program">
  <NuGetReference>CsvHelper</NuGetReference>
  <NuGetReference>Flurl</NuGetReference>
  <NuGetReference>Flurl.Http</NuGetReference>
  <NuGetReference>System.IdentityModel.Tokens.Jwt</NuGetReference>
  <Namespace>Flurl</Namespace>
  <Namespace>Flurl.Http</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.IdentityModel.Tokens.Jwt</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>CsvHelper</Namespace>
</Query>

const string api = "https://devbob-aminoapi-dev.azurewebsites.net";
//const string api = "https://localhost:44398";

async Task Main()
{

	//Runs Task in using Hangfire background task and creates 10 department clones
	await api
	.AppendPathSegment("api/Recruitments/ClonesDepartmentsWithBackgroundJob")
	.PostJsonAsync(10);

	//Runs without background processing and creates 10 student clones
	await api
	.AppendPathSegment("api/Recruitments/RecruitStudentClones")
	.PostJsonAsync(10);
	
	await Task.Delay(TimeSpan.FromSeconds(10));
	//This will produce 10 students clones and 10 department clones created without background task
	await DumpStudentClones();
	await DumpDeparmentClones();
	
	//Removes all Student & department clones
	await api
	.AppendPathSegment("api/Recruitments/RemoveDepartmentClonesWithBackgroundJob")
	.PostJsonAsync(10);

	await api
	.AppendPathSegment("api/Recruitments/RemoveStudentClones")
	.PostJsonAsync(10);

	await Task.Delay(TimeSpan.FromSeconds(10));
	await DumpStudentClones();
	await DumpDeparmentClones();

}


private async Task DumpStudentClones()
{
	var retrieveResponse = await api
	.AppendPathSegment("api/Recruitments/GetStudentClones")
	.WithTimeout(TimeSpan.FromMinutes(8))
	.GetJsonListAsync();

	((object)retrieveResponse).Dump();
}


private async Task DumpDeparmentClones()
{
	var retrieveResponse = await api
	.AppendPathSegment("api/Recruitments/GetClonedDepartments")
	.WithTimeout(TimeSpan.FromMinutes(8))
	.GetJsonListAsync();

	((object)retrieveResponse).Dump();
}