using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Idams.Core.Model.Requests
{
    public class BasePagedRequest
    {
        [Required]
        [Range(1, 2000, ErrorMessage = "Please enter a {0} value bigger than 0")]
        [DefaultValue(1)]
        public int Page { get; set; } = 1;

        [Required]
        [Range(1, 1000, ErrorMessage = "Please enter a {0} value bigger than 0")]
        [DefaultValue(10)]
        public int Size { get; set; } = 10;

        [DefaultValue("id asc")]
        public string Sort { get; set; } = "id asc";
    }
}
