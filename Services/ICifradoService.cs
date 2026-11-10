namespace ApiCatalogo_Galactico.Services
{
    public interface ICifradoService
    {
        string Cifrar(string textoPlano);
        string Descifrar(string textoCifrado);
    }
}
