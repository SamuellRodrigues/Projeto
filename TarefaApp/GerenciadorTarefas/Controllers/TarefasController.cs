using GerenciadorTarefas.Data;
using GerenciadorTarefas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorTarefas.Controllers
{
    public class TarefasController : Controller
    {
        private readonly AppDbContext _context;

        public TarefasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Tarefas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tarefas.ToListAsync());
        }

        // GET: Tarefas/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: Tarefas/CreateTest
        public IActionResult CreateTest()
        {
            return View();
        }

        // POST: Tarefas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tarefa tarefa)
        {
            try
            {
                System.Console.WriteLine("=== INÍCIO DA CRIAÇÃO DE TAREFA ===");
                
                // Verificar se o objeto tarefa foi recebido
                if (tarefa == null)
                {
                    System.Console.WriteLine("ERRO: Objeto tarefa é nulo");
                    return View(new Tarefa());
                }
                
                // Log dos dados recebidos
                System.Console.WriteLine($"Dados recebidos - Titulo: '{tarefa.Titulo}', Descricao: '{tarefa.Descricao}', Concluida: {tarefa.Concluida}");
                
                // Remover validação do ModelState temporariamente para debug
                ModelState.Remove("Id");
                ModelState.Remove("DataCriacao");
                
                // Verificar se o modelo está válido
                if (ModelState.IsValid)
                {
                    System.Console.WriteLine("ModelState é válido. Criando tarefa...");
                    
                    // Garantir que a data seja definida
                    tarefa.DataCriacao = DateTime.Now;
                    
                    // Adicionar ao contexto
                    _context.Add(tarefa);
                    
                    // Salvar no banco
                    await _context.SaveChangesAsync();
                    
                    System.Console.WriteLine("Tarefa criada com sucesso!");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Se há erros de validação, mostrar todos
                    System.Console.WriteLine("ModelState NÃO é válido. Erros encontrados:");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        System.Console.WriteLine($"Erro: {error.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"EXCEÇÃO ao criar tarefa: {ex.Message}");
                System.Console.WriteLine($"StackTrace: {ex.StackTrace}");
                ModelState.AddModelError("", "Erro interno do servidor. Tente novamente.");
            }
            
            System.Console.WriteLine("=== FIM DA CRIAÇÃO (COM ERRO) ===");
            return View(tarefa ?? new Tarefa());
        }

        // POST: Tarefas/CreateTest - Teste simplificado
        [HttpPost]
        public async Task<IActionResult> CreateTest(string titulo, string descricao)
        {
            System.Console.WriteLine($"=== TESTE SIMPLES ===");
            System.Console.WriteLine($"Titulo recebido: '{titulo}'");
            System.Console.WriteLine($"Descricao recebida: '{descricao}'");
            
            if (!string.IsNullOrEmpty(titulo))
            {
                var tarefa = new Tarefa
                {
                    Titulo = titulo,
                    Descricao = descricao ?? "",
                    DataCriacao = DateTime.Now,
                    Concluida = false
                };
                
                _context.Add(tarefa);
                await _context.SaveChangesAsync();
                System.Console.WriteLine("Tarefa criada com sucesso no teste!");
                return RedirectToAction(nameof(Index));
            }
            
            System.Console.WriteLine("Falha no teste - título vazio");
            return View();
        }

        // GET: Tarefas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarefa = await _context.Tarefas.FindAsync(id);
            if (tarefa == null)
            {
                return NotFound();
            }
            return View(tarefa);
        }

        // POST: Tarefas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descricao,DataCriacao,Concluida")] Tarefa tarefa)
        {
            if (id != tarefa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarefa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TarefaExists(tarefa.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tarefa);
        }

        // GET: Tarefas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarefa = await _context.Tarefas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        // POST: Tarefas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);
            if (tarefa != null)
            {
                _context.Tarefas.Remove(tarefa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TarefaExists(int id)
        {
            return _context.Tarefas.Any(e => e.Id == id);
        }
    }
}