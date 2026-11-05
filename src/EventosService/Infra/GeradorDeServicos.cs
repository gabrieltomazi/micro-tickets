namespace EventosService.Infra
{
    // utilitário para carregar o contexto e serviços quando necessário
    public static class GeradorDeServicos
    {
        public static ServiceProvider? ServiceProvider { get; set; }

        public static DataContext? CarregarContexto()
        {
            return ServiceProvider?.GetService<DataContext>();
        }
    }
}
