using System.ComponentModel.DataAnnotations;

namespace Assignment2.Models
{
    public class Advertisement
    {
        public int AdvertisementId { get; set; }
        public string BrokerageId { get; set; }


        [Required]
        [StringLength(50)]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Url")]
        public string Url { get; set; }

    }
}
