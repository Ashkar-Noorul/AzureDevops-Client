using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureDevOpsClientServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzureDevOpsController : ControllerBase
    {
        private readonly IAzureDevOpsClient _azureDevOpsClient;

        public AzureDevOpsController(IAzureDevOpsClient azureDevOpsClient)
        {
            _azureDevOpsClient = azureDevOpsClient;
        }
        [HttpPost("create-feature")]
        public async Task<IActionResult> CreateFeatureAsync(string projectName, string featureTitle)
        {
            try
            {
                var featureId = await _azureDevOpsClient.CreateFeature(projectName, featureTitle);
                return Ok(new { FeatureId = featureId });
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "An error occurred while creating the feature.");
            }
        }

        [HttpPost("create-story")]
        public async Task<IActionResult> CreateStoryAsync(Models.Story story)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    await _azureDevOpsClient.CreateStory(story.ProjectName, story.StoryTitle, story.Description, story.StoryPoints, story.AcceptanceCriteria, story.FeatureId);
                    return Ok("User story created successfully.");
                }
                else
                {
                    return BadRequest(ModelState);
                }
                
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "An error occurred while creating the user story.");
            }
        }
    }
}
