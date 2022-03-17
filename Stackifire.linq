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

async Task Main()
{

	//Runs Task in using Hangfire background task and creates 10 student clones
	await api
	.AppendPathSegment("api/Recruitments/RecruitClonesWithBackgroundJob")
	.PostJsonAsync(10);

	//Runs without background processing and creates 10 student clones
	await api
	.AppendPathSegment("api/Recruitments/RecruitClones")
	.PostJsonAsync(10);

	await Task.Delay(TimeSpan.FromSeconds(10));
	//This will produce 20 students, 10 created from background task and 10 created without background task
	await DumpClonesAsync();
	
	//Removes all Student clones
	await api
	.AppendPathSegment("api/Recruitments/RemoveClonesWithBackgroundJob")
	.PostJsonAsync(10);

	await Task.Delay(TimeSpan.FromSeconds(10));
	await DumpClonesAsync();

}


private async Task DumpClonesAsync()
{
	var retrieveResponse = await api
	.AppendPathSegment("api/Recruitments")
	.WithTimeout(TimeSpan.FromMinutes(8))
	.GetJsonListAsync();

	((object)retrieveResponse).Dump();
}