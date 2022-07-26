using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialChat.Domain
{
    public class Messages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string UserFrom { get; set; }
        [Required]
        public string UserTo { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
    }
}
