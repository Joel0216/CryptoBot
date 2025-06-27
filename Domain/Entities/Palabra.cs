public class Palabra
{
	public int Id { get; set; } // autoincremental (en DB)
	public required string Texto { get; set; }
	public required int Desplazamiento { get; set; }
	public string Resultado { get; set; } = string.Empty;
	public bool EsCifrado { get; set; } = true;
}
