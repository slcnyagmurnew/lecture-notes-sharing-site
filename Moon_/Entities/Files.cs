using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Moon.Entities
{
    [Table("Files")]
    public class Files
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string DocumentId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string FileType { get; set; }
        [MaxLength]
        public byte[] DataFiles { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ownerId { get; set; }
        [Required(ErrorMessage = "Please enter title for document."), MaxLength(100)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please select category for document.")]
        public string Category { get; set; }
        [Required(ErrorMessage = "Please select course code for document.")]
        public string CourseCode { get; set; }
        [Required(ErrorMessage = "Please enter lecturer for document.")]
        public string Lecturer { get; set; }
        public int Likes { get; set; }
    }
}
