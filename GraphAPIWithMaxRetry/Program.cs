using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GraphAPIWithMaxRetry;

namespace GraphAPIWithMaxRetry
{

    public class Program
    {

        public class SPItem
        {
            public string Id;
            public string Title;  
            public string Person;
            public string choiceDropdown;
            public string ChoiceRadioButton;
            public string ChoiceCheckBox;
            public string MMSCol;
            public string YesNo;
            public string link;
            public string DateOnly; 
        }
        static async Task Main(string[] args)
        {
            var spItems = new List<SPItem>();
            string clientId = AppConfig.AADClientId;
            string tenantId = AppConfig.TenantId;
            X509Certificate2 ClientCertificate = new X509Certificate2(AppConfig.ClientSigningCertificatePath, "");

            // Initialize the Confidential Client Application with certificate
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
            .Create(clientId)
            .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
            .WithCertificate(ClientCertificate)
            .Build();

            var scopes1 = new[] { "https://graph.microsoft.com/.default" };
            var authResult1 = await confidentialClientApplication.AcquireTokenForClient(scopes1).ExecuteAsync();
            var MaxRetry = 3;
            var authProvider = new DelegateAuthenticationProvider(async (requestMessage) =>
            {
                var scopes = new[] { "https://graph.microsoft.com/.default" };
                var authResult = await confidentialClientApplication.AcquireTokenForClient(scopes).ExecuteAsync();
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.AccessToken);
            });
            var selectedList = "787288c7-077e-4d75-9174-a26ff91164d3";
            var siteInformationModelSiteGuid = "cb18ccad-efcd-4d41-b3f4-94213340e636";
            List<Task> tasks = new List<Task>();
            var graphClient = new GraphServiceClient(authProvider);
            try
            { 
                var listItemsRequest = graphClient.Sites[siteInformationModelSiteGuid]
                        .Lists[selectedList].Items
                        .Request()
                        .Expand($"fields($select={GetallColumns()})");
                
                tasks.Add(Task.Run((Func<Task>)(async () =>
                {
                    try
                    {
                        var listItemsResponse = await listItemsRequest.WithMaxRetry(MaxRetry).GetAsync();
                        // Filter the collection using LINQ 
                        while (listItemsResponse != null && listItemsResponse.Count > 0)
                        {
                            var filteredItems = listItemsResponse.Where(items => items.Id != null).ToList();

                            foreach (var listItem in filteredItems)
                            {
                                FieldValueSet listItemFields = listItem.Fields;
                                string Title = GetValueByFieldName(listItemFields, FieldsName.Title.ToString());
                                string Person = GetValueByFieldName(listItemFields, FieldsName.Person.ToString());
                                string choiceDropdown = GetValueByFieldName(listItemFields, FieldsName.choiceDropdown.ToString());
                                string ChoiceRadioButton = GetValueByFieldName(listItemFields, FieldsName.ChoiceRadioButton.ToString());
                                string ChoiceCheckBox = GetValueByFieldName(listItemFields, FieldsName.ChoiceCheckBox.ToString());
                                string MMSCol = GetValueByFieldName(listItemFields, FieldsName.MMSCol.ToString());
                                string YesNo = GetValueByFieldName(listItemFields, FieldsName.YesNo.ToString());
                                string link = GetValueByFieldName(listItemFields, FieldsName.link.ToString());
                                string DateOnly = GetValueByFieldName(listItemFields, FieldsName.DateOnly.ToString());
                                spItems.Add(new SPItem()
                                {
                                    Id = $"{listItem.Id}",
                                    Title = Title,
                                    Person = Person,
                                    ChoiceCheckBox = ChoiceCheckBox,
                                    ChoiceRadioButton = ChoiceRadioButton,
                                    choiceDropdown = choiceDropdown,
                                    DateOnly = DateOnly,
                                    link = link,
                                    MMSCol = MMSCol,
                                    YesNo = YesNo 
                                });
                            }
                            if (listItemsResponse.NextPageRequest != null)
                            {
                                listItemsResponse = await listItemsResponse.NextPageRequest
                                                                        .WithMaxRetry(MaxRetry)
                                                                        .GetAsync();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    string GetValueByFieldName(FieldValueSet _listItemFields, string field)
                    {
                        return _listItemFields.AdditionalData.TryGetValue(field, out object additionalUser) ? additionalUser.ToString() : null;
                    }
                    
                })));
                // Wait for all tasks to complete
                await Task.WhenAll(tasks);
                if (spItems.Count > 0) { Console.WriteLine($"Total Number of Item found {spItems.Count}"); } else { Console.WriteLine("no item found"); };
                foreach (var item in spItems)
                {
                    Console.WriteLine($"ID: {item.Id}, Title: {item.Title}, Person: {item.Person}, ChoiceCheckBox: {item.ChoiceCheckBox}, ChoiceRadioButton: {item.ChoiceRadioButton}, choiceDropdown : {item.choiceDropdown}, DateOnly : {item.DateOnly}, mms : {item.MMSCol}");
                }
                Console.Read();
                string GetallColumns()
                {
                    List<string> columns = new List<string>() { FieldsName.YesNo.ToString(), FieldsName.ChoiceCheckBox.ToString(), FieldsName.MMSCol.ToString(), FieldsName.DateOnly.ToString(), FieldsName.choiceDropdown.ToString() };
                    return string.Join(",", columns.ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
