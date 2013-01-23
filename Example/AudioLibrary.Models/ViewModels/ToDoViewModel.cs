using System.ComponentModel.DataAnnotations;
using AudioLibrary.Models.Contract;
using ServiceStack.Common.Extensions;

namespace AudioLibrary.Models.ViewModels
{
    public class ToDoViewModel : IViewModel
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Task { get; set; }
        
        public bool Completed { get; set; }

        public IEntity ConvertToEntity()
        {
            return this.TranslateTo<ToDo>();
        }
    }
}
