
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using .Infrastructure.Data;
using .Domain;
using .Domain.Repositories.Interfaces;
using .Test.Setup;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Xunit;

namespace .Test.Controllers
{
    public class CategoriaResourceIntTest
    {
        public CategoriaResourceIntTest()
        {
            _factory = new AppWebApplicationFactory<TestStartup>().WithMockUser();
            _client = _factory.CreateClient();

            _categoriaRepository = _factory.GetRequiredService<ICategoriaRepository>();


            InitTest();
        }

        private const string DefaultNome = "AAAAAAAAAA";
        private const string UpdatedNome = "BBBBBBBBBB";

        private readonly AppWebApplicationFactory<TestStartup> _factory;
        private readonly HttpClient _client;
        private readonly ICategoriaRepository _categoriaRepository;

        private Categoria _categoria;


        private Categoria CreateEntity()
        {
            return new Categoria
            {
                Nome = DefaultNome
            };
        }

        private void InitTest()
        {
            _categoria = CreateEntity();
        }

        [Fact]
        public async Task CreateCategoria()
        {
            var databaseSizeBeforeCreate = await _categoriaRepository.CountAsync();

            // Create the Categoria
            var response = await _client.PostAsync("/api/categorias", TestUtil.ToJsonContent(_categoria));
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Validate the Categoria in the database
            var categoriaList = await _categoriaRepository.GetAllAsync();
            categoriaList.Count().Should().Be(databaseSizeBeforeCreate + 1);
            var testCategoria = categoriaList.Last();
            testCategoria.Nome.Should().Be(DefaultNome);
        }

        [Fact]
        public async Task CreateCategoriaWithExistingId()
        {
            var databaseSizeBeforeCreate = await _categoriaRepository.CountAsync();
            databaseSizeBeforeCreate.Should().Be(0);
            // Create the Categoria with an existing ID
            _categoria.Id = 1L;

            // An entity with an existing ID cannot be created, so this API call must fail
            var response = await _client.PostAsync("/api/categorias", TestUtil.ToJsonContent(_categoria));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Categoria in the database
            var categoriaList = await _categoriaRepository.GetAllAsync();
            categoriaList.Count().Should().Be(databaseSizeBeforeCreate);
        }

        [Fact]
        public async Task GetAllCategorias()
        {
            // Initialize the database
            await _categoriaRepository.CreateOrUpdateAsync(_categoria);
            await _categoriaRepository.SaveChangesAsync();

            // Get all the categoriaList
            var response = await _client.GetAsync("/api/categorias?sort=id,desc");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.[*].id").Should().Contain(_categoria.Id);
            json.SelectTokens("$.[*].nome").Should().Contain(DefaultNome);
        }

        [Fact]
        public async Task GetCategoria()
        {
            // Initialize the database
            await _categoriaRepository.CreateOrUpdateAsync(_categoria);
            await _categoriaRepository.SaveChangesAsync();

            // Get the categoria
            var response = await _client.GetAsync($"/api/categorias/{_categoria.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.id").Should().Contain(_categoria.Id);
            json.SelectTokens("$.nome").Should().Contain(DefaultNome);
        }

        [Fact]
        public async Task GetNonExistingCategoria()
        {
            var response = await _client.GetAsync("/api/categorias/" + long.MaxValue);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateCategoria()
        {
            // Initialize the database
            await _categoriaRepository.CreateOrUpdateAsync(_categoria);
            await _categoriaRepository.SaveChangesAsync();
            var databaseSizeBeforeUpdate = await _categoriaRepository.CountAsync();

            // Update the categoria
            var updatedCategoria = await _categoriaRepository.QueryHelper().GetOneAsync(it => it.Id == _categoria.Id);
            // Disconnect from session so that the updates on updatedCategoria are not directly saved in db
            //TODO detach
            updatedCategoria.Nome = UpdatedNome;

            var response = await _client.PutAsync("/api/categorias", TestUtil.ToJsonContent(updatedCategoria));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the Categoria in the database
            var categoriaList = await _categoriaRepository.GetAllAsync();
            categoriaList.Count().Should().Be(databaseSizeBeforeUpdate);
            var testCategoria = categoriaList.Last();
            testCategoria.Nome.Should().Be(UpdatedNome);
        }

        [Fact]
        public async Task UpdateNonExistingCategoria()
        {
            var databaseSizeBeforeUpdate = await _categoriaRepository.CountAsync();

            // If the entity doesn't have an ID, it will throw BadRequestAlertException
            var response = await _client.PutAsync("/api/categorias", TestUtil.ToJsonContent(_categoria));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Categoria in the database
            var categoriaList = await _categoriaRepository.GetAllAsync();
            categoriaList.Count().Should().Be(databaseSizeBeforeUpdate);
        }

        [Fact]
        public async Task DeleteCategoria()
        {
            // Initialize the database
            await _categoriaRepository.CreateOrUpdateAsync(_categoria);
            await _categoriaRepository.SaveChangesAsync();
            var databaseSizeBeforeDelete = await _categoriaRepository.CountAsync();

            var response = await _client.DeleteAsync($"/api/categorias/{_categoria.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the database is empty
            var categoriaList = await _categoriaRepository.GetAllAsync();
            categoriaList.Count().Should().Be(databaseSizeBeforeDelete - 1);
        }

        [Fact]
        public void EqualsVerifier()
        {
            TestUtil.EqualsVerifier(typeof(Categoria));
            var categoria1 = new Categoria
            {
                Id = 1L
            };
            var categoria2 = new Categoria
            {
                Id = categoria1.Id
            };
            categoria1.Should().Be(categoria2);
            categoria2.Id = 2L;
            categoria1.Should().NotBe(categoria2);
            categoria1.Id = 0;
            categoria1.Should().NotBe(categoria2);
        }
    }
}
