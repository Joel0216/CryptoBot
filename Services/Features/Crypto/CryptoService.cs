using System.Collections.Generic;
using System.Linq;
using CryptoBot.Data;
using CryptoBot.Domain.Entities;
// If 'Palabra' is not defined, define it in the correct namespace:
namespace CryptoBot.Domain.Entities
{
    public class Palabra
    {
        public int Id { get; set; }
        public string Texto { get; set; } = string.Empty;
        public int Desplazamiento { get; set; }
        public string Resultado { get; set; } = string.Empty;
        public bool EsCifrado { get; set; }
    }
}

namespace CryptoBot.Services.Features.Crypto
{
    public class CryptoService
    {
        private readonly AppDbContext _context;

        public CryptoService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Palabra> GetAll() => _context.Palabras.ToList();

        public Palabra? GetById(int id) => _context.Palabras.Find(id);

        public Palabra Add(Palabra palabra)
        {
            palabra.Resultado = Cesar(palabra.Texto, palabra.Desplazamiento, true);
            palabra.EsCifrado = true;

            _context.Palabras.Add(palabra);
            _context.SaveChanges();
            return palabra;
        }

        public bool Update(int id, Palabra nueva)
        {
            var original = _context.Palabras.Find(id);
            if (original is null) return false;

            original.Texto = nueva.Texto;
            original.Desplazamiento = nueva.Desplazamiento;
            original.Resultado = Cesar(nueva.Texto, nueva.Desplazamiento, true);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var palabra = _context.Palabras.Find(id);
            if (palabra is null) return false;
            _context.Palabras.Remove(palabra);
            _context.SaveChanges();
            return true;
        }

        private string Cesar(string texto, int desplazamiento, bool cifrar)
        {
            if (!cifrar) desplazamiento = -desplazamiento;

            return new string(texto.Select(c =>
            {
                if (!char.IsLetter(c)) return c;
                char baseChar = char.IsUpper(c) ? 'A' : 'a';
                return (char)((((c - baseChar + desplazamiento + 26) % 26) + baseChar));
            }).ToArray());
        }

        public string Desencriptar(string texto, int desplazamiento)
        {
            return Cesar(texto, desplazamiento, cifrar: false);
        }
    }
}
