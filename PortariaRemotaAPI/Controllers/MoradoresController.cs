using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortariaRemotaAPI.Models;
using PortariaRemotaAPI.Models.DataContext;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortariaRemotaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoradoresController : ControllerBase
    {
        private readonly KiperContext _context;

        public MoradoresController(KiperContext context)
        {
            _context = context;
        }

        // GET: api/Moradores
        [HttpGet]
        public IEnumerable<Morador> GetMoradores()
        {
            return _context.Moradores.Include(apto => apto.Apartamento)
                .OrderBy(mrd => mrd.DataNascimento)
                .OrderBy(apto => apto.Apartamento.Numero)
                .OrderBy(apto => apto.Apartamento.Bloco);
        }

        // GET: api/Moradores/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMorador([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var morador = await _context.Moradores.Include(apto => apto.Apartamento).FirstOrDefaultAsync(mrd => mrd.MoradorId.Equals(id));

            if (morador == null)
            {
                return NotFound(new { Message = "Morador não encontrado", Error = true });
            }

            return Ok(morador);
        }

        // PUT: api/Moradores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMorador([FromRoute] int id, [FromBody] Morador morador)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != morador.MoradorId)
            {
                return BadRequest();
            }

            var oldMorador = await _context.Moradores.Include(x => x.Apartamento).FirstOrDefaultAsync(mrd => 
                mrd.Apartamento.ApartamentoId == morador.Apartamento.ApartamentoId &&
                mrd.CPF.Equals(morador.CPF) && mrd.DataNascimento.Date.Equals(morador.DataNascimento.Date));
            if(oldMorador != null)
            {
                return NotFound(new { Message = "Não foi possível atualizar o Morador pois o CPF, Data de Nascimento e Apartamento informados já se encontram cadastrado para outro Morador.", Error = true });
            }

            var entityApto = await _context.Apartamentos.FindAsync(morador.Apartamento.ApartamentoId);
            _context.Entry(entityApto).State = EntityState.Detached;
            var entityMrd = await _context.Moradores.FindAsync(id);
            _context.Entry(entityMrd).State = EntityState.Detached;

            _context.Entry(morador).State = EntityState.Modified;
            _context.Entry(morador.Apartamento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MoradorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Moradores
        [HttpPost]
        public async Task<IActionResult> PostMorador([FromBody] Morador morador)
        {
            if (!ModelState.IsValid)
                return NotFound(new { Message = ModelState.First().Value, Error = true });

            await _context.Database.BeginTransactionAsync();

            try
            {
                var apartamento = await _context.Apartamentos.FirstOrDefaultAsync<Apartamento>(apto => 
                    apto.Bloco.Trim().ToUpper().Equals(morador.Apartamento.Bloco.Trim().ToUpper()) &&
                    apto.Numero.Equals(morador.Apartamento.Numero));

                if (apartamento is null)
                {
                    _context.Apartamentos.Add(morador.Apartamento);
                    _context.SaveChanges();
                }
                else
                    morador.Apartamento = apartamento;

                var oldMorador = await _context.Moradores.Include(x => x.Apartamento).FirstOrDefaultAsync<Morador>(mrd =>
                    mrd.Apartamento.ApartamentoId == morador.Apartamento.ApartamentoId &&
                    mrd.CPF.Trim().Equals(morador.CPF.Trim()) && 
                    mrd.DataNascimento.Date.Equals(morador.DataNascimento.Date));

                //Se morador existe retorna erro.
                if(oldMorador != null)
                {
                    _context.Database.RollbackTransaction();
                    return NotFound(new { Message = $"Erro ao tentar incluir o Morador {morador.Nome}. Já existe um morador com mesma Data de Nascimento e CPF.", Error = true });
                }

                _context.Moradores.Add(morador);
                await _context.SaveChangesAsync();
                _context.Database.CommitTransaction();
            }
            catch
            {
                _context.Database.RollbackTransaction();
                return NotFound(new { Message = $"Erro ao tentar incluir o Morador {morador.Nome}", Error = true });
            }

            return CreatedAtAction("GetMorador", new { id = morador.MoradorId }, morador);
        }

        // DELETE: api/Moradores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMorador([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var morador = await _context.Moradores.FindAsync(id);
            if (morador == null)
            {
                return NotFound(new { Message = "Erro ao excluir o Morador.", Error = true });
            }

            _context.Moradores.Remove(morador);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool MoradorExists(int id)
        {
            return _context.Moradores.Any(e => e.MoradorId == id);
        }
    }
}