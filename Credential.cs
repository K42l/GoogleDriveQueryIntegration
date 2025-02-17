using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Drive.Query.Integration;
using Google.Drive.Query.Integration.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Google.Drive.Backup.Util
{
    public static class Credential
    {
        public static IServiceCollection AddIntegration(this IServiceCollection services, string applicationName) 
        {
            GoogleCredential credential = GoogleCredential.GetApplicationDefault()
                    .CreateScoped(DriveService.Scope.Drive);

            var dService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });

            return services.AddSingleton<IIntegration, Integration>(x =>
            {
                return new Integration(dService);
            });
        }
    }
}
