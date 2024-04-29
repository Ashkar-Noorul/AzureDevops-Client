namespace AzureDevOpsClientServices
{
    public interface IAzureDevOpsClient
    {
        Task<int> CreateFeature(string projectName, string featureTitle);
        Task CreateStory(string projectName, string storyTitle, string description, int storyPoints, string[]acceptanceCriteria, int?featureId);
    }
}
