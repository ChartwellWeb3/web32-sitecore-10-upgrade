using Chartwell.Foundation.CustomAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Chartwell.Foundation.Models
{
  public class ReviewsRatingViewModel
  {
    public PropertyModel Property { get; set; }

    public List<ReviewsRatingsModel> Reviews { get; set; }

    public List<RoleModel> RoleList { get; set; }

    public CaptchaModel Captcha { get; set; }

    public ReviewsRatingsModel Review { get; set; }

    //[Required(ErrorMessage = "Please select the correct fruit.")]
    [LocalizedRequired(ErrorMessage = "ReviewCaptchaError")]
    //[Remote("ValidateCaptcha", "ReviewsRatings", ErrorMessage = "Wrong Captcha. Please try again")]
    public List<string> CaptchaImagesList { get; set; }

    [EmailAddress]
    [LocalizedRequired(ErrorMessage = "ReviewsEmailError")]
    public string Email { get; set; }

    [LocalizedDisplayName("ReviewsFirstName")]
    [LocalizedRequired(ErrorMessage = "ReviewsFirstNameError")]

    public string FirstName { get; set; }

    [LocalizedDisplayName("ReviewsLasttName")]
    [LocalizedRequired(ErrorMessage = "ReviewsLastNameError")]

    public string LastName { get; set; }

    [LocalizedDisplayName("ReviewsRole")]
    public string RoleIdentification { get; set; }

    [LocalizedDisplayName("ReviewsCommentTitle")]
    public string Comments { get; set; }

    [LocalizedDisplayName("ReviewsContentTitle")]
    [LocalizedRequired(ErrorMessage = "ReviewsRatingsError")]
    public string Ratings { get; set; }

  }
}