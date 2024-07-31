namespace dapper_crud.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Produto
    {
        public int Id { get; set; }

        [Required]
        public string? Nome { get; set; }

        [Required]
        public decimal? Preco { get; set; }
    }
}
