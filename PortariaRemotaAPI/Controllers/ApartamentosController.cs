using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortariaRemotaAPI.Models;
using PortariaRemotaAPI.Models.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortariaRemotaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApartamentosController : ControllerBase
    {
        private readonly KiperContext _context;

        public ApartamentosController(KiperContext context)
        {
            _context = context;
        }

        // GET: api/Apartamentos
        [HttpGet]
        public IEnumerable<Apartamento> GetApartamentos()
        {
            return _context.Apartamentos
                .OrderBy(apto => apto.Numero)
                .OrderBy(apto => apto.Bloco);
        }

        // GET: api/Apartamentos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetApartamento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var apartamento = await _context.Apartamentos.FindAsync(id);

            if (apartamento == null)
            {
                return NotFound(new { Message = "Apartamento não encontrado", Error = true });
            }

            return Ok(apartamento);
        }

        // PUT: api/Apartamentos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApartamento([FromRoute] int id, [FromBody] Apartamento apartamento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != apartamento.ApartamentoId)
            {
                return BadRequest();
            }

            //Verifica se os dados do apartamento atualizado já não se encontra na base.
            var oldApto = _context.Apartamentos.FirstOrDefault(apto => apto.Bloco.Trim().Equals(apartamento.Bloco.Trim()) && apto.Numero == apartamento.Numero);
            if(oldApto != null)
            {
                return NotFound(new { Message = "Não foi possível atualizar o Apartamento pois o Bloco e Número informados já se encontram cadastrado.", Error = true });
            }

            var entity = await _context.Apartamentos.FindAsync(id);
            _context.Entry(entity).State = EntityState.Detached;
            _context.Entry(apartamento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApartamentoExists(id))
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

        // POST: api/Apartamentos
        //[HttpPost]
        //public async Task<IActionResult> PostApartamento([FromBody] Apartamento apartamento)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    _context.Apartamentos.Add(apartamento);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetApartamento", new { id = apartamento.ApartamentoId }, apartamento);
        //}

        // DELETE: api/Apartamentos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApartamento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var apartamento = await _context.Apartamentos.FindAsync(id);
            if (apartamento == null)
            {
                return NotFound(new { Message = "Erro ao excluir o Apartamento e seus moradores.", Error = true });
            }

            //Abre transação para garantir toda a fita de exclusão.
            await _context.Database.BeginTransactionAsync();
            try
            {
                IEnumerable<Morador> list = await _context.Moradores.Include(x => x.Apartamento).Where(mrd => mrd.Apartamento.ApartamentoId == apartamento.ApartamentoId).ToListAsync<Morador>();
                if (list != null)
                    _context.Moradores.RemoveRange(list);

                _context.Apartamentos.Remove(apartamento);
                await _context.SaveChangesAsync();
                _context.Database.CommitTransaction();

                return Ok();
            }
            catch
            {
                _context.Database.RollbackTransaction();
                return NotFound(new { Message = "Erro ao excluir o Apartamento e seus moradores.", Error = true });
            }
        }

        private bool ApartamentoExists(int id)
        {
            return _context.Apartamentos.Any(e => e.ApartamentoId == id);
        }
    }
}