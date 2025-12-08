using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Courses.Application.DTOs
{
    public class CreateLessonRequest
    {
        [Required]
        public string title { get; set; }
        [Required]
        public string contentUrl { get; set; } 
        [Required]
        public int orderIndex { get; set; }
        [Required]
        public Guid moduleId { get; set; }
        [Required]
        public bool isPreview { get; set; }
        [Required]
        public int lessonType { get; set; }
        [RequiredIfVideoLesson("lessonType", 0)]
        public double duration { get; set; }

    }

    public class RequiredIfVideoLessonAttribute : ValidationAttribute
    {
        private readonly string _lessonTypePropertyName;
        private readonly int _videoLessonTypeValue;
        public RequiredIfVideoLessonAttribute(string lessonTypePropertyName, int videoLessonTypeValue)
        {
            _lessonTypePropertyName = lessonTypePropertyName;
            _videoLessonTypeValue = videoLessonTypeValue;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var lessonTypeProperty = validationContext.ObjectType.GetProperty(_lessonTypePropertyName);
            if (lessonTypeProperty == null)
            {
                return new ValidationResult($"Unknown property: {_lessonTypePropertyName}");
            }
            var lessonTypeValue = lessonTypeProperty.GetValue(validationContext.ObjectInstance, null);
            if (lessonTypeValue is int lessonTypeInt && lessonTypeInt == _videoLessonTypeValue)
            {
                // For Video lessons, duration must be provided and > 0
                if (value == null || (value is double d && d <= 0))
                {
                    return new ValidationResult("Duration is required for Video lessons and must be greater than 0.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
