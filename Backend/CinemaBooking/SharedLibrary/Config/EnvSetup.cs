﻿using DotNetEnv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Config
{
    public class EnvSetup
    {
        private static readonly string envFilePath = Path.GetFullPath("../../../.env");

        public static void EnsureEnvFileExists()
        {
            if (!File.Exists(envFilePath))
            {
                var jwtSigningKey = Guid.NewGuid();
                var jwtIssuer = "https://localhost:5001";
                var jwtAudience = "https://localhost:5002";
                var userServiceUrl = "https://localhost:5001";
                var filmServiceUrl = "https://localhost:5003";
                var screeningServiceUrl = "https://localhost:5004";
                var seatReservationServiceUrl = "https://localhost:5005";

                var envContent = $"JWT_SIGNING_KEY={jwtSigningKey}\n" +
                    $"JWT_ISSUER={jwtIssuer}\n" +
                    $"JWT_AUDIENCE={jwtAudience}\n" +
                    $"USER_SERVICE_URL={userServiceUrl}\n" +
                    $"FILM_SERVICE_URL={filmServiceUrl}\n" +
                    $"SCREENING_SERVICE_URL={screeningServiceUrl}\n" +
                    $"SEATRESERVATION_SERVICE_URL={seatReservationServiceUrl}";
                
                File.WriteAllText(envFilePath, envContent);
            }

            Env.Load(envFilePath);
        }
    }
}
