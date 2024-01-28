﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace UserStore.Core.Models
{
    public class User
    {
        public Guid Id { get; }
        public string? Еmail { get; }
        public string? Password { get; }
        public string? Token { get; }

        private User(Guid id, string email, string password, string token)
        {
            this.Id = id;
            this.Еmail = email;
            this.Password = password;
            this.Token = token;
            //if(string.IsNullOrEmpty(Token))
            //{
            //    Console.WriteLine($"old - {Token}");
            //    this.Token = GenerateJwtToken(password, "9f6a1d7e5b3c8a4d9f6a1d7e5b3c8a4d");
            //    Console.WriteLine($"new - {Token}");
            //}
            //else
            //{
            //    Console.WriteLine("token был ранее");
            //}
        }
        private User(string token)
        {
            this.Token = token;
        }
        private User(string email, string password)
        {
            this.Еmail = email;
            this.Password = password;
        }
        private User(string email, string password, string token)
        {
            this.Еmail = email;
            this.Password = password;
            this.Token = token;
        }
        // get user
        public static (User User, string Error) Create(Guid id, string email, string password, string token)
        {
            var error = string.Empty;

            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                error = "Email or Password can not be empty";
            }


            User user = new(id, email, password, token);
            //user.GenerateToken("9f6a1d7e5b3c8a4d9f6a1d7e5b3c8a4d");

            return (user, error);
        }
        // create
        public static (User User, string Error) Create(string email, string password)
        {
            var error = string.Empty;

            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                error = "Email or Password can not be empty";
            }

            User user = new(email, password);
            //user.GenerateToken("9f6a1d7e5b3c8a4d9f6a1d7e5b3c8a4d");

            return (user, error);
        }
        // login
        public static (User User, string Error) Create(string email, string password, string token)
        {
            var error = string.Empty;

            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                error = "Email or Password can not be empty";
            }
            //if (!ValidateToken(token, "9f6a1d7e5b3c8a4d9f6a1d7e5b3c8a4d"))
            //{
            //    error = "Invalid token";
            //}

            User user = new(email, password, token);
            //user.GenerateToken("9f6a1d7e5b3c8a4d9f6a1d7e5b3c8a4d");

            return (user, error);
        }
        // login 2
        public static (User User, string Error) Create(string token)
        {
            var error = string.Empty;

            //if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            //{
            //    error = "Email or Password can not be empty";
            //}
            //if (!ValidateToken(token, "9f6a1d7e5b3c8a4d9f6a1d7e5b3c8a4d"))
            //{
            //    error = "Invalid token";
            //}

            User user = new(token);
            //user.GenerateToken("9f6a1d7e5b3c8a4d9f6a1d7e5b3c8a4d");

            return (user, error);
        }
        //private static string? GetUserTokenFromDatabase(Guid userId)
        //{
        //    // Здесь вы должны получить токен пользователя из вашей базы данных или другого хранилища
        //    // Верните токен, если он существует, или null, если токен не найден
        //    // Пример:
        //    return _context.Users.SingleOrDefault(u => u.Id == userId)?.Token;
        //    //return null;
        //}


        public static string GenerateToken(Guid id, string secretKey)
        {
            Console.WriteLine("id for token " + id);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, id.ToString())
                // Дополнительные утверждения (claims) могут быть добавлены здесь
                }),
                Expires = DateTime.UtcNow.AddDays(7), // Установите желаемое время истечения срока действия токена
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            Console.WriteLine("new token " + token);

            return tokenHandler.WriteToken(token);
        }
        public static bool ValidateToken(string token, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero // Установите желаемую погрешность времени, если необходимо
            };

            try
            {
                SecurityToken validatedToken;
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return true; // Токен валиден
            }
            catch (Exception)
            {
                return false; // Токен недействителен
            }
        }
        public static string ExtractIdFromToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                Console.WriteLine("token " + token);
                Console.WriteLine("jwtToken " + jwtToken);

                string stringId = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

                return stringId;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
