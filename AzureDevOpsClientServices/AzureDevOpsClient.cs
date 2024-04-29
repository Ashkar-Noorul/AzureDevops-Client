using Microsoft.Extensions.Options;
//provides classes and functions for interacting with the core azure devops services, like projects, teams and identities;
using Microsoft.TeamFoundation.Core.WebApi;
//Offers classes and methods specifically designed for working with work items in Azure DevOps. 
//This includes querying, creating, updating, and managing work items.
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
//Contains model classes representing work items, work item fields, and other related entities within Azure DevOps work item tracking.
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
//Provides general-purpose classes used across various Azure DevOps services. It includes functionalities for authentication, exception handling, and data serialization.
using Microsoft.VisualStudio.Services.Common;
//Contains core classes and functionalities for interacting with Azure DevOps services through the Web API. This includes establishing connections, managing credentials, and handling responses.
using Microsoft.VisualStudio.Services.WebApi;
//Offers classes and methods for working with JSON Patch documents. These documents allow you to define specific changes to be made to Azure DevOps resources (like work items) in a structured format.
using Microsoft.VisualStudio.Services.WebApi.Patch;
//Specifically focused on creating JSON Patch documents for modifying Azure DevOps resources. It provides helper classes and methods to simplify the process of building these patch documents.
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Threading.Tasks;
using System;


namespace AzureDevOpsClientServices
{
    public class AzureDevOpsClient:IAzureDevOpsClient
    {
        private readonly AzureDevOpsOptions _options;
        public AzureDevOpsClient(IOptions<AzureDevOpsOptions> options) {
            _options = options.Value;
        }
        public async Task<int> CreateFeature(string projectName, string featureTitle)
        {
            try
            {
                using (var connection = new VssConnection(new Uri(_options.OrganizationUrl), new VssBasicCredential(string.Empty, _options.PersonalAccessToken)))
                {
                    var workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

                    JsonPatchDocument feature = new JsonPatchDocument
                    {
                        new JsonPatchOperation()
                        {
                            Operation = Operation.Add,
                            Path = "/fields/System.Title",
                            Value = featureTitle
                        },
                        new JsonPatchOperation()
                        {
                            Operation = Operation.Add,
                            Path = "/fields/System.Description",
                            Value = "This is a sample feature created via Azure DevOps SDK."
                        }
                    };

                    WorkItem featureWorkItem = await workItemTrackingClient.CreateWorkItemAsync(feature, projectName, "Feature");

                    Console.WriteLine("Feature created successfully.");
                    return (int)featureWorkItem.Id;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating feature: {ex.Message}");
                return 0;
            }
        }
        public async Task CreateStory(string projectName, string storyTitle, string description, int storyPoints, string[]acceptanceCriteria, int? featureId = null)
        {
            try
            {
                using (var connection = new VssConnection(new Uri(_options.OrganizationUrl), new VssBasicCredential(string.Empty, _options.PersonalAccessToken)))
                {
                    var workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

                    JsonPatchDocument story = new JsonPatchDocument
                    {
                        new JsonPatchOperation()
                        {
                            Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
                            Path = "/fields/System.Title",
                            Value = storyTitle
                        },
                        new JsonPatchOperation()
                        {
                            Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
                            Path = "/fields/System.Description",
                            Value = description
                        }
                    };

                    AddAcceptanceCriteria(story, acceptanceCriteria);
                    AddStoryPoints(story, storyPoints);


                    if (featureId.HasValue && featureId != 0)
                    {
                        story.Add(new JsonPatchOperation
                        {
                            Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
                            Path = "/relations/-",
                            Value = new WorkItemRelation
                            {
                                Rel = "System.LinkTypes.Dependency-forward",
                                Url = $"{_options.OrganizationUrl}/{projectName}/_apis/wit/workItems/{featureId}",
                                Attributes = new Dictionary<string, object>
                                {
                                    { "comment", "Related feature" }
                                }
                            }
                        });
                    }


                    await workItemTrackingClient.CreateWorkItemAsync(story, projectName, "User Story");
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error creating user story: {ex.Message}");
            }
        }

        private void AddAcceptanceCriteria(JsonPatchDocument story, string[] acceptanceCriteria)
        {
            if (acceptanceCriteria != null && acceptanceCriteria.Length > 0)
            {
                foreach (var criteria in acceptanceCriteria)
                {
                    story.Add(new JsonPatchOperation
                    {
                        Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
                        Path = "/fields/Microsoft.VSTS.Common.AcceptanceCriteria",
                        Value = criteria
                    });
                }
            }
        }

        private void AddStoryPoints(JsonPatchDocument story, int storyPoints)
        {
            if (storyPoints > 0)
            {
                story.Add(new JsonPatchOperation
                {
                    Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
                    Path = "/fields/Microsoft.VSTS.Scheduling.StoryPoints",
                    Value = storyPoints
                });
            }
        }
    }
}
