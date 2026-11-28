namespace ClientesService.Infra
{
    // provedor estático de serviços seguindo o padrão do professor
    public static class GeradorDeServicos
    {
        public static ServiceProvider ServiceProvider { get; set; } = null!;

        public static DataContext CarregarContexto()
        {
            return ServiceProvider.GetRequiredService<DataContext>();
        }
    }
}
