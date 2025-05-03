using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetshopPeterson.Data; 
using PetshopPeterson.Models;
using PetshopPeterson.DTOs;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace PetshopPeterson.Controllers
{
    [ApiController]
    [Route("api/")]
    public class ServicoriaController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _environment;

        public ServicoriaController(AppDbContext appDbContext, IWebHostEnvironment environment)
        {
            _appDbContext = appDbContext;
            _environment = environment;
        }

        [HttpGet("buscar-todas-servicos")]
        public async Task<IActionResult> GetServicos()
        {
            var servicos = await _appDbContext.Servico.ToListAsync();
            return Ok(servicos);
        }

        [HttpGet("buscar-servico/{id}")]
        public async Task<IActionResult> GetServico(int id)
        {
            var servico = await _appDbContext.Servico.FindAsync(id);
            if (servico == null)
                return NotFound("Serviço não encontrado.");
            return Ok(servico);
        }

        [HttpPost("criar-servico")]
        public async Task<IActionResult> AddServico([FromForm] ServicoUploadDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Descricao) || dto.Imagem == null || dto.Valor <= 0)
                return BadRequest("Preencha todos os campos corretamente.");

            var pastaDestino = Path.Combine(_environment.WebRootPath, "imagens");
            if (!Directory.Exists(pastaDestino))
                Directory.CreateDirectory(pastaDestino);

            var nomeArquivo = Guid.NewGuid() + Path.GetExtension(dto.Imagem.FileName);
            var caminhoCompleto = Path.Combine(pastaDestino, nomeArquivo);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await dto.Imagem.CopyToAsync(stream);
            }

            var urlImagem = $"{Request.Scheme}://{Request.Host}/imagens/{nomeArquivo}";

            var novoServico = new Servico
            {
                Descricao = dto.Descricao,
                Valor = dto.Valor,
                Imagem = urlImagem
            };

            _appDbContext.Servico.Add(novoServico);
            await _appDbContext.SaveChangesAsync();

            return StatusCode(201, novoServico);
        }

        [HttpPut("alterar-servico/{id}")]
        public async Task<IActionResult> UpdateServico(int id, Servico servico)
        {
            if (servico == null || servico.Id != id)
                return BadRequest("Dados inválidos.");

            var existente = await _appDbContext.Servico.FindAsync(id);
            if (existente == null)
                return NotFound("Serviço não encontrado.");

            existente.Descricao = servico.Descricao;
            existente.Valor = servico.Valor;

            _appDbContext.Servico.Update(existente);
            await _appDbContext.SaveChangesAsync();

            return Ok(existente);
        }

        [HttpDelete("apagar-servico/{id}")]
        public async Task<IActionResult> DeleteServico(int id)
        {
            var servico = await _appDbContext.Servico.FindAsync(id);
            if (servico == null)
                return NotFound("Serviço não encontrado.");

            _appDbContext.Servico.Remove(servico);
            await _appDbContext.SaveChangesAsync();

            return Ok("Serviço removido com sucesso.");
        }

        [HttpDelete("apagar-todas-servicos")]
        public async Task<IActionResult> DeleteAllServico()
        {
            var servicos = await _appDbContext.Servico.ToListAsync();
            int count = 0;

            foreach (var servico in servicos)
            {
                if (string.IsNullOrEmpty(servico.Descricao) || servico.Valor <= 0)
                {
                    _appDbContext.Servico.Remove(servico);
                    count++;
                }
            }

            await _appDbContext.SaveChangesAsync();
            return Ok(count > 0
                ? $"{count} serviços removidos com sucesso!"
                : "Nenhum serviço foi removido.");
        }

        [HttpGet("buscar-todos-agendamentos")]
        public async Task<IActionResult> GetAgendamentos()
        {
            var agendamentos = await _appDbContext.Agendamento
                .Include(a => a.AgendamentoServico)
                    .ThenInclude(asv => asv.Servico)
                .Include(a => a.Tutor)
                .ToListAsync();

            if (agendamentos.Count == 0)
                return NotFound("Nenhum agendamento encontrado.");

            return Ok(agendamentos);
        }

        [HttpPost("criar-agendamento")]
        public async Task<IActionResult> CriarAgendamento(AgendamentoDTO dto)
        {
            if (dto == null)
                return BadRequest("Dados inválidos!");

            var agendamento = new Agendamento
            {
                TutorId = dto.TutorId,
                Data = DateTime.Now,
                AgendamentoServico = dto.AgendamentoServico.Select(a => new AgendamentoServico
                {
                    ServicoId = a.ServicoId,
                    Quantidade = a.Quantidade
                }).ToList()
            };

            _appDbContext.Agendamento.Add(agendamento);
            await _appDbContext.SaveChangesAsync();

            return StatusCode(201, agendamento);
        }

    // DTO usado para upload de imagem
    public class ServicoUploadDTO
    {
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public IFormFile Imagem { get; set; } = null!;
    }
}
