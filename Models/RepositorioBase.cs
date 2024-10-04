namespace PenalozaFernandezInmobiliario.Models
{
    public abstract class RepositorioBase
    {
        protected readonly IConfiguration configuration;
        protected readonly string ConnectionString;

        protected RepositorioBase(IConfiguration configuration)
        {
            this.configuration = configuration;
            ConnectionString = configuration["ConnectionString:DefaultConnection"];
        }
    }
}
