namespace IngressosService.Infra
{
    public static class GeradorDeServicos
    {
        public static ServiceProvider ServiceProvider { get; set; } = null!;

        public static DataContext CarregarContexto()
        {
            return ServiceProvider.GetRequiredService<DataContext>();
        }

        public static IConfiguration CarregarConfiguracao()
        {
            return ServiceProvider.GetRequiredService<IConfiguration>();
        }

        public static IHttpClientFactory CarregarHttpClientFactory()
        {
            return ServiceProvider.GetRequiredService<IHttpClientFactory>();
        }
    }
}
