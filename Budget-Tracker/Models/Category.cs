using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Budget_Tracker.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Column(TypeName ="nvarchar(50)")]
        public string Title { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string Icon { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Type { get; set; }

        [NotMapped]
        public string TitleWithIcon { get { return this.Title + " " + this.Icon; } }

    }
}
