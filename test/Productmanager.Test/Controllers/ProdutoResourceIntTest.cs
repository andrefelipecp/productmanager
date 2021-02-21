
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
    public class ProdutoResourceIntTest
    {
        public ProdutoResourceIntTest()
        {
            _factory = new AppWebApplicationFactory<TestStartup>().WithMockUser();
            _client = _factory.CreateClient();

            _produtoRepository = _factory.GetRequiredService<IProdutoRepository>();


            InitTest();
        }

        private const string DefaultNome = "AAAAAAAAAA";
        private const string UpdatedNome = "BBBBBBBBBB";

        private static readonly int? DefaultQuantidade = 1;
        private static readonly int? UpdatedQuantidade = 2;

        private static readonly double? DefaultValor = 1D;
        private static readonly double? UpdatedValor = 2D;

        private readonly AppWebApplicationFactory<TestStartup> _factory;
        private readonly HttpClient _client;
        private readonly IProdutoRepository _produtoRepository;

        private Produto _produto;


        private Produto CreateEntity()
        {
            return new Produto
            {
                Nome = DefaultNome,
                Quantidade = DefaultQuantidade,
                Valor = DefaultValor
            };
        }

        private void InitTest()
        {
            _produto = CreateEntity();
        }

        [Fact]
        public async Task CreateProduto()
        {
            var databaseSizeBeforeCreate = await _produtoRepository.CountAsync();

            // Create the Produto
            var response = await _client.PostAsync("/api/produtos", TestUtil.ToJsonContent(_produto));
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Validate the Produto in the database
            var produtoList = await _produtoRepository.GetAllAsync();
            produtoList.Count().Should().Be(databaseSizeBeforeCreate + 1);
            var testProduto = produtoList.Last();
            testProduto.Nome.Should().Be(DefaultNome);
            testProduto.Quantidade.Should().Be(DefaultQuantidade);
            testProduto.Valor.Should().Be(DefaultValor);
        }

        [Fact]
        public async Task CreateProdutoWithExistingId()
        {
            var databaseSizeBeforeCreate = await _produtoRepository.CountAsync();
            databaseSizeBeforeCreate.Should().Be(0);
            // Create the Produto with an existing ID
            _produto.Id = 1L;

            // An entity with an existing ID cannot be created, so this API call must fail
            var response = await _client.PostAsync("/api/produtos", TestUtil.ToJsonContent(_produto));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Produto in the database
            var produtoList = await _produtoRepository.GetAllAsync();
            produtoList.Count().Should().Be(databaseSizeBeforeCreate);
        }

        [Fact]
        public async Task GetAllProdutos()
        {
            // Initialize the database
            await _produtoRepository.CreateOrUpdateAsync(_produto);
            await _produtoRepository.SaveChangesAsync();

            // Get all the produtoList
            var response = await _client.GetAsync("/api/produtos?sort=id,desc");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.[*].id").Should().Contain(_produto.Id);
            json.SelectTokens("$.[*].nome").Should().Contain(DefaultNome);
            json.SelectTokens("$.[*].quantidade").Should().Contain(DefaultQuantidade);
            json.SelectTokens("$.[*].valor").Should().Contain(DefaultValor);
        }

        [Fact]
        public async Task GetProduto()
        {
            // Initialize the database
            await _produtoRepository.CreateOrUpdateAsync(_produto);
            await _produtoRepository.SaveChangesAsync();

            // Get the produto
            var response = await _client.GetAsync($"/api/produtos/{_produto.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.id").Should().Contain(_produto.Id);
            json.SelectTokens("$.nome").Should().Contain(DefaultNome);
            json.SelectTokens("$.quantidade").Should().Contain(DefaultQuantidade);
            json.SelectTokens("$.valor").Should().Contain(DefaultValor);
        }

        [Fact]
        public async Task GetNonExistingProduto()
        {
            var response = await _client.GetAsync("/api/produtos/" + long.MaxValue);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateProduto()
        {
            // Initialize the database
            await _produtoRepository.CreateOrUpdateAsync(_produto);
            await _produtoRepository.SaveChangesAsync();
            var databaseSizeBeforeUpdate = await _produtoRepository.CountAsync();

            // Update the produto
            var updatedProduto = await _produtoRepository.QueryHelper().GetOneAsync(it => it.Id == _produto.Id);
            // Disconnect from session so that the updates on updatedProduto are not directly saved in db
            //TODO detach
            updatedProduto.Nome = UpdatedNome;
            updatedProduto.Quantidade = UpdatedQuantidade;
            updatedProduto.Valor = UpdatedValor;

            var response = await _client.PutAsync("/api/produtos", TestUtil.ToJsonContent(updatedProduto));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the Produto in the database
            var produtoList = await _produtoRepository.GetAllAsync();
            produtoList.Count().Should().Be(databaseSizeBeforeUpdate);
            var testProduto = produtoList.Last();
            testProduto.Nome.Should().Be(UpdatedNome);
            testProduto.Quantidade.Should().Be(UpdatedQuantidade);
            testProduto.Valor.Should().Be(UpdatedValor);
        }

        [Fact]
        public async Task UpdateNonExistingProduto()
        {
            var databaseSizeBeforeUpdate = await _produtoRepository.CountAsync();

            // If the entity doesn't have an ID, it will throw BadRequestAlertException
            var response = await _client.PutAsync("/api/produtos", TestUtil.ToJsonContent(_produto));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Produto in the database
            var produtoList = await _produtoRepository.GetAllAsync();
            produtoList.Count().Should().Be(databaseSizeBeforeUpdate);
        }

        [Fact]
        public async Task DeleteProduto()
        {
            // Initialize the database
            await _produtoRepository.CreateOrUpdateAsync(_produto);
            await _produtoRepository.SaveChangesAsync();
            var databaseSizeBeforeDelete = await _produtoRepository.CountAsync();

            var response = await _client.DeleteAsync($"/api/produtos/{_produto.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the database is empty
            var produtoList = await _produtoRepository.GetAllAsync();
            produtoList.Count().Should().Be(databaseSizeBeforeDelete - 1);
        }

        [Fact]
        public void EqualsVerifier()
        {
            TestUtil.EqualsVerifier(typeof(Produto));
            var produto1 = new Produto
            {
                Id = 1L
            };
            var produto2 = new Produto
            {
                Id = produto1.Id
            };
            produto1.Should().Be(produto2);
            produto2.Id = 2L;
            produto1.Should().NotBe(produto2);
            produto1.Id = 0;
            produto1.Should().NotBe(produto2);
        }
    }
}
