using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace int3306
{
    [Table("student")]
    public class Student
    {
        [Key]
        [Required]
        [JsonProperty("id")]
        [Column("id")]
        public int? ID { get; set; }

        [Required]
        [JsonProperty("name")]
        [Column("name")]
        public string Name { get; set; } = null!;
        
        [Required]
        [JsonProperty("class")]
        [Column("class")]
        public string Class { get; set; } = null!;
        
        [Required]
        [JsonProperty("origin")]
        [Column("origin")]
        public string Origin { get; set; } = null!;
    }
}