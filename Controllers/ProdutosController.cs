using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;
using Dapper;
using dapper_crud.Models;

namespace dapper_crud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly string _connectionString;

        public ProdutosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new ArgumentNullException(nameof(configuration));
        }

        // GET api/produtos
        [HttpGet]
        public async Task<IActionResult> GetProdutos()
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();
            var produtos = await connection.QueryAsync<Produto>("SELECT * FROM Produtos");
            return Ok(produtos);
        }

        // GET api/produtos/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduto(int id)
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();
            var produto = await connection.QueryFirstOrDefaultAsync<Produto>("SELECT * FROM Produtos WHERE Id = @Id", new { Id = id });

            if (produto == null)
            {
                return NotFound();
            }

            return Ok(produto);
        }

        // POST api/produtos
        [HttpPost]
        public async Task<IActionResult> CreateProduto([FromBody] Produto produto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            var query = "INSERT INTO Produtos (Nome, Preco) VALUES (@Nome, @Preco); SELECT last_insert_rowid()";
            var id = await connection.ExecuteScalarAsync<int>(query, new { produto.Nome, produto.Preco });

            produto.Id = id;

            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);
        }

        // PUT api/produtos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduto(int id, [FromBody] Produto produto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.ExecuteAsync(
                "UPDATE Produtos SET Nome = @Nome, Preco = @Preco WHERE Id = @Id",
                new { produto.Nome, produto.Preco, Id = id });

            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE api/produtos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.ExecuteAsync(
                "DELETE FROM Produtos WHERE Id = @Id",
                new { Id = id });

            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
