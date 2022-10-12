using System.ComponentModel.DataAnnotations;

namespace WebAssemblyBlazorApp1
{
    public class ColumnUserInput
    {
        [Required]
        public List<InfoSchemaColumns>? ColumnList { get; set; }
        [Required]
        public String? TargetSchema { get; set; }
    }
}
