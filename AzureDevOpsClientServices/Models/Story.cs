using System.ComponentModel.DataAnnotations;

namespace AzureDevOpsClientServices.Models
{
    public class Story
    {
        [Required(ErrorMessage = "ProjectName is required")]
        public string ProjectName { get; set; }

        [Required(ErrorMessage = "StoryTitle is required")]
        public string StoryTitle { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "StoryPoints must be greater than zero")]
        public int StoryPoints { get; set; }

        [MinLength(1, ErrorMessage = "At least one acceptance criteria is required")]
        public string[] AcceptanceCriteria { get; set; }

        
        public int? FeatureId { get; set; }

    }
}
