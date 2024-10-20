using DotNetEnv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Config
{
    public class EnvSetup
    {
        private static readonly string envFilePath = Path.GetFullPath("../.env");

        public static void EnsureEnvFileExists()
        {
            if (!File.Exists(envFilePath))
            {
                File.WriteAllText(envFilePath, $"JWT_SIGNING_KEY={Guid.NewGuid()}\nJWT_ISSUER=https://localhost:5001\nJWT_AUDIENCE=https://localhost:5002");
            }

            Env.Load(envFilePath);
        }
    }
}
