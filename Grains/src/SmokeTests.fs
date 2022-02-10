namespace Grains

open System
open System.Net.Http
open System.Threading.Tasks
open Orleans

type ISmokeTests =
    inherit IGrainWithGuidKey
    abstract member SmokeTest1 : unit -> Task<bool>
    abstract member SmokeTest2 : unit -> Task<bool>

type SmokeTests() =
    inherit Grain()

    interface ISmokeTests with
        member this.SmokeTest1() : Task<bool> =
            task {
                do! Task.Delay(2000)
                return true
            }

        member this.SmokeTest2() : Task<bool> =
            let query = "SandboxEAtestxml" + Guid.NewGuid().ToString().Replace("-", "") + "@emailage.com"
            
//            let queryString = 

            let client = new HttpClient()
            client.BaseAddress <- Uri("https://sandbox.emailage.com/EmailAgeValidator/")
            
            // var result = await fixture.Client.QueryAsyncXml<ClassicApiResponse>(HttpMethod.Post, query);
            
            task {
                let! qwe = client.PostAsync(query, null)
                return false
            }

// 2022-02-09 PJ:
// --------------
// observations:
// * [ ] fixture has only two used properties: Client (of type ClassicApiClient) and SlaSeconds (of type int)
// * [ ] ClassicApiClient only has one constructor that takes environment variables:
//      * [ ] BaseUrl
//      * [ ] UserEmail
//      * [ ] ConsumerKey
// * [ ] ClassicApiClient
// tasks

//[Fact]
//public async Task Sandbox_XML_Post()
//{
//    var query = "SandboxEAtestxml" + Guid.NewGuid().ToString().Replace("-", "") + "@emailage.com";
//
//    var result = await fixture.Client.QueryAsyncXml<ClassicApiResponse>(HttpMethod.Post, query);
//
//    result?.responseStatus?.Should().NotBeNull();
//    result?.responseStatus?.status?.Should().Be("success");
//    result?.query.Results?[0].Email.Should().NotBeNull();
//    result?.query.ResponseCount.Should().BeGreaterOrEqualTo(1);
//    result?.query.Results[0].EAScore.Should().NotBeNullOrWhiteSpace();
//    result?.query.Results[0].DomainExists.Should().NotBeNullOrWhiteSpace();
//    int.TryParse(result?.query.Results[0].TotalHits, out int TotalHits).Should().BeTrue();
//    TotalHits.Should().BeGreaterOrEqualTo(1);
//}